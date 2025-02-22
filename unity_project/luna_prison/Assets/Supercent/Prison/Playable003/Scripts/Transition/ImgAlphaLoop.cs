using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Supercent.PrisonLife.Playable003
{
    public class ImgAlphaLoop : MonoBehaviour
    {
        const float FORWARD = 1f;
        const float BACKWORD = -1f;
        [SerializeField] Image _image;
        [SerializeField] Color _start;
        [SerializeField] Color _end;
        [SerializeField] float _speed = 1f;
        float _lerpValue = 0;
        float _currentDir = FORWARD;
        void Update()
        {
            _lerpValue += Time.deltaTime * _speed * _currentDir;
            if (_lerpValue >= 1f)
                _currentDir = BACKWORD;
            else if (_lerpValue <= 0f)
                _currentDir = FORWARD;

            _image.color = Color.Lerp(_start, _end, Mathf.Clamp01(_lerpValue));
        }
    }

}
