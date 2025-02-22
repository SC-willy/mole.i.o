using System.Collections;
using UnityEngine;
using Supercent.Util;

namespace Supercent.MoleIO.InGame
{
    public class CtaOpener : MonoBehaviour
    {
        [Header("Timer Info")]
        [SerializeField] bool _isTimerdCTA = false;
        [SerializeField] float _delay = 3f;
        bool _isCalled = false;
        private void Start()
        {
            if (_isTimerdCTA)
                StartCoroutine(CoCheckPress());
        }

        private IEnumerator CoCheckPress()
        {
            yield return CoroutineUtil.WaitForSeconds(_delay);

            if (!_isCalled)
                OpenCTAButton();
        }
        public void OpenCTAButton()
        {
            _isCalled = true;
        }
    }
}
