using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class DynamicLineSelector : MonoBehaviour
    {
        [SerializeField] CustomerCheckOutLine _singleLinePassArea;
        [SerializeField] PathBase _pathCurve;
        [SerializeField] PathBase _pathPoint;
        [LunaPlaygroundField("Waterslide Line SquareShape")]
        [SerializeField] bool _pathIsSquare = false;

        private void Awake()
        {
            _singleLinePassArea.SetPath(_pathIsSquare ? _pathPoint : _pathCurve);
        }
    }
}

