using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [CreateAssetMenu(fileName = "TransitionJump", menuName = "Transition/Jump")]
    public class TransitionJump : TransitionGenericBase<TransTokenJump>
    {
        [SerializeField] float _jumpHeight = 3f;
        [SerializeField] protected AnimationCurve _jumpHeightCurve;
        [SerializeField] protected AnimationCurve _jumpMoveCurve;
        [Range(0, 1)][SerializeField] protected float _jumpMaxHeightTime = 0.5f;

        Vector3 stackPos;
        public override void _UpdateTransition(TransTokenJump token)
        {
            stackPos = token.End.position + token.Gap;
            stackPos.y = token.Time < _jumpMaxHeightTime ? Mathf.Lerp(token.StartY, token.MaxY + _jumpHeight, _jumpHeightCurve.Evaluate(token.Time)) : Mathf.Lerp(token.BottomY, token.MaxY + _jumpHeight, _jumpHeightCurve.Evaluate(token.Time));
            token.Target.position = Vector3.Lerp(token.StartPos, stackPos, _jumpMoveCurve.Evaluate(token.Time));
        }
        public void StartTransition(Transform target, Transform start, Transform end, Vector3 gap, Action doneCallback = null)
        {
            base.StartTransition(target, start, end, doneCallback);
            _transitionData[target].Gap = gap;
        }
        public void StartTransition(Transform target, Transform end, Vector3 gap, Action doneCallback = null)
        {
            StartTransition(target, target, end, doneCallback);
        }
    }
}