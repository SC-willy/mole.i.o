
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class FovManager : MonoBehaviour
    {
        [SerializeField] Camera _mainCamera;
        [SerializeField] Camera[] _cameras;
        [SerializeField] BoxCollider _camColider;
        [Header("Must Bigger then OriginFOV")]
        [SerializeField] float _wideFovDecreasedStrength = 40;


        [LunaPlaygroundField("Player Sight", 0, "Player")]
        [Range(0f, 300f)]
        [SerializeField] float _originalFieldOfView = 90;
        [SerializeField] float _colSizeFactor = 0.001f;
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

            float height = 2f * _colSizeFactor * Mathf.Tan(_mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            _camColider.size = new Vector3
                (
                     height * _mainCamera.aspect,
                     height,
                    _camColider.size.z
                );
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
