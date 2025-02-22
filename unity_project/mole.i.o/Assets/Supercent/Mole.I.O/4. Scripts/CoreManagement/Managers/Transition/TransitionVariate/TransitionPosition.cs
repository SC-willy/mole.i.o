using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [CreateAssetMenu(fileName = "TransitionTrPosition", menuName = "Transition/Position")]
    public class TransitionTrPosition : TransitionGenericBase<TransitionToken>
    {
        [SerializeField] AnimationCurve _curve;
        public override void _UpdateTransition(TransitionToken token)
        {
            token.Target.position = Vector3.Lerp(token.Start.position, token.End.position, _curve.Evaluate(token.Time));
        }
    }
}