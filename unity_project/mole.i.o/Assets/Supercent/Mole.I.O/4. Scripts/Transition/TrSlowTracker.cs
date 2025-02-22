using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class TrSlowTracker : MonoBehaviour
    {
        [SerializeField] Transform _targetTr;
        [SerializeField] Transform _originTransform;
        [SerializeField] float _traceSpeed = 1f;

        Vector3 _prevPos;
        private void OnEnable()
        {
            _prevPos = _originTransform.position;
        }
        private void Update()
        {
            _targetTr.position = Vector3.Lerp(_prevPos, _originTransform.position, _traceSpeed * Time.deltaTime);
            _prevPos = _targetTr.position;
        }
    }
}