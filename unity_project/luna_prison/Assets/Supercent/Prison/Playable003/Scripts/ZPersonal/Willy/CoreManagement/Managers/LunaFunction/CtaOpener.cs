using System.Collections;
using UnityEngine;
using Luna.Unity;
using Supercent.Util;

namespace Supercent.PrisonLife.Playable003
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
            Playable.InstallFullGame();
            _isCalled = true;
        }
    }
}
