using System;
using Supercent.MoleIO.Management;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class AudioMuteManager : IBindable
    {
        [SerializeField] AudioSource[] _audioSources;
        [SerializeField] private float[] _volumes;

        public void CheckSound() => ActiveSound(PlayerData.IsVolumeOn == 1);
        public void ActiveSound(bool on)
        {
            for (int i = 0; i < _audioSources.Length; i++)
            {
                _audioSources[i].volume = on ? _volumes[i] : 0f;
            }
        }

#if UNITY_EDITOR
        public void Bind(MonoBehaviour mono)
        {
            _audioSources = mono.GetComponentsInChildren<AudioSource>(true);
            _volumes = new float[_audioSources.Length];
            for (int i = 0; i < _volumes.Length; i++)
            {
                _volumes[i] = _audioSources[i].volume;
            }
        }
#endif // UNITY_EDITOR
    }
}