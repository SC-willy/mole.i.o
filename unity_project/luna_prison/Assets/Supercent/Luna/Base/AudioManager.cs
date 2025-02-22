using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using UnityRandom = UnityEngine.Random;

namespace Supercent.Base
{
    public static class AudioManager
    {
        static UpdateListener rootAudio = null;

        static AudioSource oneShotSource = null;
        public static Transform TargetListener = null;

        public static bool IgnorePlayCall = false;
        static Transform rootManaged = null;
        static readonly List<AudioSource> managed = new List<AudioSource>(8);

        static Transform rootManual = null;
        static readonly Dictionary<string, AudioInfo> manual = new Dictionary<string, AudioInfo>();



        public static void Initialize(Transform parent, int managedCapacity = 8)
        {
            var root = parent == null
                     ? AppUtil.GetAppObejct().transform
                     : parent;

            if (AppUtil.GetOrAddComponent(parent, "Audio", out rootAudio))
            {
                rootAudio.lateUpdate -= LateUpdate;
                rootAudio.lateUpdate += LateUpdate;
            }
            if (AppUtil.GetOrAddComponent(rootAudio, "OneShot", out oneShotSource))
                oneShotSource.playOnAwake = false;

            var listener = UnityObject.FindObjectOfType<AudioListener>();
            if (listener != null)
                TargetListener = listener.transform;


            // Release and new
            foreach (IAudioInfo audio in manual.Values)
                audio.Release();
            managed.Clear();
            manual.Clear();

            if (managedCapacity < 8)
                managedCapacity = 8;
            managed.Capacity = managedCapacity;

            if (rootManaged != null) UnityObject.Destroy(rootManaged);
            if (rootManual != null) UnityObject.Destroy(rootManual);

            rootManaged = AppUtil.NewObject("Manged", rootAudio).transform;
            rootManual = AppUtil.NewObject("Manual", rootAudio).transform;

            for (int index = 0; index < managed.Capacity; ++index)
            {
                if (AppUtil.GetOrAddComponent(rootManaged, $"AUDIO_{index}", out AudioSource source))
                    source.playOnAwake = false;
                managed.Add(source);
            }
        }

        static void LateUpdate()
        {
            foreach (IAudioInfo audio in manual.Values)
                audio.Update();
        }


        public static void PlayOneShot(AudioClip clip, float volume = 1f)
        {
            if (IgnorePlayCall) return;
            if (clip == null) return;
            if (oneShotSource == null) return;

            oneShotSource.PlayOneShot(clip, volume);
        }

        #region Managed
        public static void Play(AudioClip clip, float volume = 1f) => Play(clip, volume, 1f, 0f, Vector3.zero, Quaternion.identity);
        public static void Play(AudioClip clip, float volume, float pitch, float offset = 0f) => Play(clip, volume, pitch, offset, Vector3.zero, Quaternion.identity);
        public static void Play(AudioClip clip, float volume, float pitch, float offset, Vector3 position, Quaternion quaternion)
        {
            if (IgnorePlayCall) return;
            if (clip == null) return;

            var source = GetSource();
            if (source == null) return;

            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.transform.position = position;
            source.transform.rotation = quaternion;

            if (offset == 0f)
                source.Play();
            else if (offset < 0f)
                source.PlayDelayed(Mathf.Abs(offset));
            else
                source.PlayScheduled(offset);
        }
        static AudioSource GetSource()
        {
            float secOld = 0f;
            AudioSource oldPlay = null;
            for (int index = 0, cnt = managed.Count; index < cnt; ++index)
            {
                var source = managed[index];
                if (source == null)
                {
                    Debug.LogError($"{nameof(AudioManager)} : {nameof(managed)}[{index}] is null");
                    continue;
                }

                if (source.isPlaying)
                {
                    var secCur = source.time;
                    if (oldPlay == null || secOld < secCur)
                    {
                        secOld = secCur;
                        oldPlay = source;
                    }
                    continue;
                }

                return source;
            }

            if (oldPlay != null)
                oldPlay.Stop();
            return oldPlay;
        }

        public static void Mute(bool value)
        {
            for (int index = 0, cnt = managed.Count; index < cnt; ++index)
            {
                var source = managed[index];
                if (source != null)
                    source.mute = value;
            }
        }

        public static void Stop(AudioClip clip)
        {
            if (clip == null)
                return;

            for (int index = 0, cnt = managed.Count; index < cnt; ++index)
            {
                var source = managed[index];
                if (source != null && source.clip == clip)
                    source.Stop();
            }
        }
        public static void StopAll()
        {
            for (int index = 0, cnt = managed.Count; index < cnt; ++index)
            {
                var source = managed[index];
                if (source != null)
                    source.Stop();
            }
        }
        #endregion// Managed


        #region Manual
        public static AudioInfo GetManual(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            name = name.ToUpper();
            var hasManual = manual.TryGetValue(name, out var audio) && audio != null;
            if (!hasManual)
            {
                if (AppUtil.GetOrAddComponent(rootManual, name, out AudioSource source))
                {
                    source.playOnAwake = false;
                    manual[name] = audio = new AudioInfo();
                    (audio as IAudioInfo).SetSource(source);
                }
            }
            return audio;
        }

        public static void BatchManuals(Action<AudioInfo> callback)
        {
            if (callback == null)
                return;

            foreach (var audio in manual.Values)
            {
                if (audio != null)
                    callback(audio);
            }
        }
        #endregion// Manual


        interface IAudioInfo
        {
            void SetSource(AudioSource src);
            void Release();
            void Update();
        }

        interface IAudioModule
        {
            void SetInfo(AudioInfo info);
            void Release();
            void Update();
        }

        public enum MoveType
        {
            Forward = 0,
            Reverse,
            Random,
        }

        public sealed class AudioInfo : IAudioInfo
        {
            public string Name { private set; get; } = string.Empty;
            public AudioSource Source => source; AudioSource source = null;
            public bool IsValid => source != null;
            public Transform Target = null;

            public readonly TrackModule Track = new TrackModule();
            public readonly FxModule Fx = new FxModule();


            void IAudioInfo.SetSource(AudioSource src)
            {
                (Track as IAudioModule).SetInfo(this);
                (Fx as IAudioModule).SetInfo(this);

                Name = src.name;
                source = src;
            }
            void IAudioInfo.Release()
            {
                (Track as IAudioModule).Release();
                (Fx as IAudioModule).Release();

                source = null;
                Target = null;
            }

            void IAudioInfo.Update()
            {
                if (source == null)
                    AppUtil.GetOrAddComponent(rootManual, Name, out source);
#if UNITY_EDITOR
                else
                {
                    if (source.transform.parent != rootManual)
                        Debug.LogError($"{nameof(AudioInfo)} : Do not modify the parent of the AudioSource");
                }
#endif// UNITY_EDITOR

                if (!source.enabled) return;
                if (!source.gameObject.activeInHierarchy) return;

                if (Target != null)
                {
                    source.transform.position = Target.position;
                    source.transform.rotation = Target.rotation;
                }

                if (Track.Enabled) (Track as IAudioModule).Update();
                if (Fx.Enabled) (Fx as IAudioModule).Update();
            }
        }

        public sealed class TrackModule : IAudioModule
        {
            AudioInfo THIS = null;
            bool enabled = false;
            public bool Enabled
            {
                set
                {
                    enabled = value;
                    pendingInterval = true;
                }
                get => enabled;
            }

            public readonly List<AudioClip> Clips = new List<AudioClip>();
            public int Id { private set; get; } = 0;
            public MoveType Move = MoveType.Forward;
            public float Interval = 0f;
            bool pendingInterval = false;


            void IAudioModule.SetInfo(AudioInfo info)
            {
                THIS = info;
            }
            void IAudioModule.Release()
            {
                THIS = null;

                Id = 0;
                Enabled = false;
                Clips.Clear();
            }

            void IAudioModule.Update()
            {
                if (Clips.Count < 1) return;

                var source = THIS.Source;
                if (!source.isPlaying)
                {
                    if (pendingInterval)
                        SetTrack(Id);

                    var clip = source.clip;
                    if (clip != null
                     && source.time < clip.length)
                        Play();
                    else
                    {
                        var id = GetNextId(Id);
                        if (SetTrack(id))
                            Play();
                    }
                }
            }

            void Play()
            {
                if (pendingInterval)
                {
                    pendingInterval = false;
                    THIS.Source.Play();
                }
                else
                {
                    THIS.Source.PlayDelayed(Interval);
                }
            }


            public bool SetTrack(int id)
            {
                var cnt = Clips.Count;
                Id = id < 1 ? 0
                   : id < cnt ? id
                   : cnt - 1;

                if (THIS.Source == null)
                    return false;

                var clip = cnt < 1 ? null : Clips[Id];
                THIS.Source.clip = clip;
                return clip != null;
            }

            public int GetNextId(int id)
            {
                var cnt = Clips.Count;
                if (cnt < 2) return 0;

                switch (Move)
                {
                default:
                case MoveType.Forward:
                    ++id;
                    return id < 1 ? 0
                         : id < cnt ? id
                         : 0;

                case MoveType.Reverse:
                    --id;
                    return id < 0 ? cnt - 1
                         : id < cnt ? id
                         : cnt - 1;

                case MoveType.Random:
                    if (cnt < 3)
                        return UnityRandom.Range(0, cnt);

                    var last = cnt - 1;
                    var newId = UnityRandom.Range(0, last);
                    return newId == id ? last : newId;
                }
            }
        }

        public sealed class FxModule : IAudioModule
        {
            AudioInfo THIS = null;
            bool enabled = false;
            public bool Enabled
            {
                set
                {
                    enabled = value;
                }
                get => enabled;
            }

            public float MinVolume = 1f;
            public float MaxVolume = 1f;

            float minDistance = 0f;
            float minSqrDistance = 0f;
            public float MinDistance
            {
                set
                {
                    minDistance = value;
                    minSqrDistance = value * value;
                }
                get => minDistance;
            }

            float maxDistance = 1f;
            float maxSqrDistance = 1f;
            public float MaxDistance
            {
                set
                {
                    maxDistance = value;
                    maxSqrDistance = value * value;
                }
                get => maxDistance;
            }


            void IAudioModule.SetInfo(AudioInfo info)
            {
                THIS = info;
            }
            void IAudioModule.Release()
            {
                THIS = null;
            }

            void IAudioModule.Update()
            {
                var source = THIS.Source;

                var listener = TargetListener;
                if (listener == null)
                    source.volume = MinVolume;
                else
                {
                    var distance = listener.position - source.transform.position;
                    var sqrDistance = Vector3.SqrMagnitude(distance);

                    var ratio = sqrDistance < minSqrDistance ? 0f
                              : maxSqrDistance < sqrDistance ? 1f
                              : (maxSqrDistance - minSqrDistance) / sqrDistance - minSqrDistance;
                    var volume = Mathf.LerpUnclamped(MinVolume, MaxVolume, ratio);
                    source.volume = volume;
                }
            }
        }
    }
}