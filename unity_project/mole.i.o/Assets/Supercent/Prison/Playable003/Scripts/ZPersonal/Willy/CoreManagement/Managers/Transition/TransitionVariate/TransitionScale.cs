using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    [CreateAssetMenu(fileName = "TransitionTrScale", menuName = "Transition/Scale")]
    public class TransitionTrScale : TransitionGenericBase<TransitionToken>
    {
        [SerializeField] Vector3 _startScale;
        [SerializeField] Vector3 _endScale;
        [SerializeField] AnimationCurve _curve;
        public override void _UpdateTransition(TransitionToken token)
        {
            token.Target.localScale = Vector3.Lerp(_startScale, _endScale, _curve.Evaluate(token.Time));
        }
    }
}