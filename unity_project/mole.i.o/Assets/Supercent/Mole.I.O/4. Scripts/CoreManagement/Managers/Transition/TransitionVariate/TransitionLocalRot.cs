using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [CreateAssetMenu(fileName = "TransitionLocalRot", menuName = "Transition/LocalRot")]
    public class TransitionLocalRot : TransitionGenericBase<TransTokenStartTrRot>
    {
        [SerializeField] Quaternion _targetRot = Quaternion.identity;
        [SerializeField] AnimationCurve _curve;
        public override void _UpdateTransition(TransTokenStartTrRot token)
        {
            token.Target.rotation = Quaternion.Lerp(token.StartRot, _targetRot, _curve.Evaluate(token.Time));
        }
    }
}