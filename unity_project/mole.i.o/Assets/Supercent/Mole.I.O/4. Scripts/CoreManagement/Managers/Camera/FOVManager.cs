
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class FovManager : MonoBehaviour
    {
        [SerializeField] Camera _mainCamera;
        [SerializeField] Camera[] _cameras;
        [Header("Must Bigger then OriginFOV")]
        [SerializeField] float _wideFovDecreasedStrength = 40;

        [Range(0f, 300f)]
        [SerializeField] float _originalFieldOfView = 90;
        float _wideFieldOfView;
        float _defaultFieldOfView;
        public float DefaultFieldOfView { get { return _defaultFieldOfView; } }
        float _currentAspectRatio;
        private void Awake()
        {
            CalculateCamView();
        }
        public void CalculateCamView()
        {
            _currentAspectRatio = Screen.width - Screen.height;
            _wideFieldOfView = _originalFieldOfView - _wideFovDecreasedStrength;
            _defaultFieldOfView = _currentAspectRatio > 0 ? _wideFieldOfView : _originalFieldOfView;

            // Set Camera
            for (int i = 0; i < _cameras.Length; i++)
            {
                _cameras[i].fieldOfView = _defaultFieldOfView;
            }
            _mainCamera.fieldOfView = _defaultFieldOfView;
        }

        private void Update()
        {
            if (_currentAspectRatio != Screen.width - Screen.height)
            {
                CalculateCamView();
            }
        }
    }

}
