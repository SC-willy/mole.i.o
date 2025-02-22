using UnityEngine;
using System;

namespace Supercent.PrisonLife.Playable003
{
    [CreateAssetMenu(fileName = "TransitionFixedPosition", menuName = "Transition/FixedPosition")]
    public class TransitionFixedPos : TransitionGenericBase<TransTokenVector3>
    {
        [SerializeField] AnimationCurve _curve;
        public override void _UpdateTransition(TransTokenVector3 token)
        {
            token.Target.position = Vector3.Lerp(token.StartPos, token.EndPos, _curve.Evaluate(token.Time));
        }

        public void StartTransition(Transform target, Vector3 start, Vector3 end, Action doneCallback = null)
        {
            OnOverwriteUpdate?.Invoke(target, this);
            TransTokenVector3 token = GetToken();
            token.Initialize(this, target, target, target, doneCallback);
            token.StartPos = start;
            token.EndPos = end;
            _activeTweens.Add(token);

            if (!_transitionData.ContainsKey(target))
            {
                _transitionData.Add(target, token);
            }
            else
            {
                _transitionData[target] = token;
            }
        }
    }
}