using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [CreateAssetMenu(fileName = "TransitionPositionTrace", menuName = "Transition/PositionTrace")]
    public class TransitionPositionTrace : TransitionGenericBase<TransTokenStartTrPos>
    {
        [SerializeField] AnimationCurve _curve;
        public override void _UpdateTransition(TransTokenStartTrPos token)
        {
            token.Target.position = Vector3.Lerp(token.Start.position, token.End.position, _curve.Evaluate(token.Time));
        }
    }
}