using System;
using Supercent.Util;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class PooledObject : MonoBehaviour, IPoolObject
    {
        public event Action OnPoolAction;
        public IPoolBase OwnPool { get; set; }
        public Transform Parent { get { return _parnet; } set { _parnet = value; } }

        Transform _parnet;
        Vector3 _originPos;
        Vector3 _originScale;
        Quaternion _originRot;

        protected virtual void Awake()
        {
            _originPos = transform.position;
            _originRot = transform.rotation;
            _originScale = transform.localScale;
        }
        public virtual void OnGenerate()
        {
            transform.parent = Parent;
        }

        public virtual void OnGet()
        {
            gameObject.SetActive(true);
        }

        public void OnReturn()
        {
            gameObject.SetActive(false);
            transform.parent = Parent;
            transform.position = _originPos;
            transform.rotation = _originRot;
            transform.localScale = _originScale;
        }

        public void OnTerminate() { }

        public virtual void ReturnSelf()
        {
            if (OwnPool != null)
                OwnPool.Return(this);
            else
                Destroy(gameObject);
        }
        public void InvokePoolAction() => OnPoolAction?.Invoke();
        public void ReleasePoolAction() => OnPoolAction = null;
    }
}