using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    [CreateAssetMenu(fileName = "TransitionScatterGet", menuName = "Transition/ScatterGet")]
    public class TransitionScatterGet : TransitionGenericBase<TransTokenScatter>
    {
        [SerializeField] float _range;
        [Range(1f, 0f)]
        [SerializeField] float _curveTime;
        [SerializeField] AnimationCurve _curveStart;
        [SerializeField] AnimationCurve _curveEnd;
        [SerializeField] AnimationCurve _curveForwardRot;
        [SerializeField] Vector3 _getPos;
        float _inversedCurveTime;
        float _inversedCurveBackTime;
        public override void _UpdateTransition(TransTokenScatter token)
        {
            if (token.Time < _curveTime)
            {
                token.Target.position = Vector3.Lerp(token.StartPos, token.ScatterPos, _curveStart.Evaluate(token.Time * _inversedCurveTime));
            }
            else
            {
                token.Target.position = Vector3.Lerp(token.ScatterPos, token.End.position + _getPos, _curveEnd.Evaluate((token.Time - _curveTime) * _inversedCurveBackTime));
            }
            token.Target.forward = Vector3.Lerp(token.OriginForward, token.EndForward, _curveForwardRot.Evaluate(token.Time));
        }
        public override void StartTransition(Transform target, Transform start, Transform end, Action doneCallback = null)
        {
            OnOverwriteUpdate?.Invoke(target, this);
            TransTokenScatter token = GetToken();
            token.Initialize(this, target, start, end, doneCallback);
            token.SetScatterPos(_range);
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

        public override void StartSetup()
        {
            base.StartSetup();
            _inversedCurveTime = 1 / _curveTime;
            _inversedCurveBackTime = 1 / (1 - _curveTime);
        }
    }
}