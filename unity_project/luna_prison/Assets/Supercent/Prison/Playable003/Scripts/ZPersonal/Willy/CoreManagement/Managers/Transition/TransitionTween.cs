using System;
using UnityEngine;


namespace Supercent.PrisonLife.Playable003
{
    public abstract class TransitionTween : ScriptableObject, IStartable, IUpdateable
    {
        public Action<Transform, TransitionTween> OnOverwriteUpdate;
        public float InversedDuration => _inversedDuration;
        public float Duration => _duration;
        public bool Loop => _loop;
        [SerializeField] protected float _duration;
        [SerializeField] protected int _poolCount;
        [SerializeField] protected bool _loop = false;
        [SerializeField] protected bool _unstopable = false;
        protected float _inversedDuration = 0;


        public abstract void StartSetup();
        public abstract void StartTransition(Transform target, Transform start, Transform end, Action doneCallback = null);
        public void StartTransition(Transform target, Transform end, Action doneCallback = null) => StartTransition(target, target, end, doneCallback);
        public void UpdateManualy() => UpdateManualy(Time.deltaTime);
        public abstract void UpdateManualy(float deltaTime);
        public abstract void UpdateTransition(TransitionToken token);
        public abstract void UpdateLinkTransition(TransitionToken token);
        public abstract void StopTransition(Transform target);
    }

}