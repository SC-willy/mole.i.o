using System.Collections;
using Supercent.Util;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public abstract class InitCheckObject : MonoBehaviour, IInitable
    {
        protected bool _isInit = false;
        public bool IsInit => _isInit;

        public void Init()
        {
            _isInit = true;
            _Init();
        }
        protected abstract void _Init();
        public void Release()
        {
            _isInit = false;
            _Release();
        }
        protected abstract void _Release();

        protected virtual void OnDestroy()
        {
            Release();
        }

#if UNITY_EDITOR
        const float CHECK_DELAY = 0.1f;
        protected virtual void Start()
        {
            StartCoroutine(CoCheckInit());
        }

        IEnumerator CoCheckInit()
        {
            yield return CoroutineUtil.WaitForSeconds(CHECK_DELAY);
            if (_isInit)
                yield break;
            Debug.LogWarning("InitializableObject Not inited! : " + gameObject.name);
        }
#endif
    }

    public interface IInitable
    {
        void Init();
        void Release();
    }
}
