using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class DynamicLineSelector : MonoBehaviour
    {
        [SerializeField] CustomerCheckOutLine _singleLinePassArea;
        [SerializeField] PathBase _pathCurve;
        [SerializeField] PathBase _pathPoint;
        [SerializeField] bool _pathIsSquare = false;

        private void Awake()
        {
            _singleLinePassArea.SetPath(_pathIsSquare ? _pathPoint : _pathCurve);
        }
    }
}

