using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [CreateAssetMenu(fileName = "TransitionTrPositionHeight", menuName = "Transition/PositionHeight")]
    public class TransitionTrPositionHeight : TransitionGenericBase<TransTokenStartTrPos>
    {
        [SerializeField] AnimationCurve _verticalCurve;
        [SerializeField] AnimationCurve _horizontalCurve;
        Vector3 _curPos;
        public override void _UpdateTransition(TransTokenStartTrPos token)
        {
            _curPos = Vector3.Lerp(token.StartPos, token.End.position, _horizontalCurve.Evaluate(token.Time));
            _curPos.y = Mathf.Lerp(token.StartPos.y, token.End.position.y, _verticalCurve.Evaluate(token.Time));
            token.Target.position = _curPos;
        }
    }
}