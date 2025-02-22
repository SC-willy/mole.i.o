using Supercent.MoleIO.InGame;
using UnityEngine;

namespace Supercent.MoleIO.Playable03
{
    public class PitchUpAudio : MonoBehaviour
    {
        [SerializeField] AudioSource _audioSource;
        [SerializeField] float _min = 1f;

        [SerializeField] float _max = 2f;

        [SerializeField] float _gap = 0.07f;

        [SerializeField] float _waitTime = 0.1f;
        float _lastUpdateTime;
        bool _isReseted = true;
        bool _isMute = false;

        private void Awake()
        {
            _audioSource.pitch = _min;
        }

        private void Mute(bool isSoundOn) => _isMute = !isSoundOn;

        public void Play()
        {
            if (_isMute)
                return;

            _audioSource.PlayOneShot(_audioSource.clip);
            _audioSource.pitch = Mathf.Min(_gap + _audioSource.pitch, _max);
            _lastUpdateTime = Time.time;
            _isReseted = false;
        }

        public void ResetPitch()
        {
            _audioSource.pitch = _min;
            _isReseted = true;
        }

        private void Update()
        {
            if (_isReseted)
                return;

            if (Time.time < _lastUpdateTime + _waitTime)
                return;

            ResetPitch();
        }
    }
}