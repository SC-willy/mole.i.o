using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    [CreateAssetMenu(fileName = "TransitionTrRotation", menuName = "Transition/Rotation")]
    public class TransitionRotation : TransitionGenericBase<TransitionToken>
    {
        [SerializeField] AnimationCurve _curve;
        public override void _UpdateTransition(TransitionToken token)
        {
            token.Target.rotation = Quaternion.Lerp(token.Start.rotation, token.End.rotation, _curve.Evaluate(token.Time));
        }
    }
}