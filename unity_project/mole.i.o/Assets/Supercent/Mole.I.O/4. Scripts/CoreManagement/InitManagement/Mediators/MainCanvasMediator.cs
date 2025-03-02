
using Supercent.MoleIO.Playable001;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class MainCanvasMediator : InitManagedBehaviorBase
    {
        [SerializeField] Canvas _canvas;
        [SerializeField] ScreenInputController _screenInputHandler;
        [SerializeField] LeaderBoard _leaderBoard;
        public void RegistLeaderBoardPlayer(UnitBattleController unit) => _leaderBoard.RegistUnit(unit);
        protected override void _Init()
        {
            _screenInputHandler.SetCanvas(_canvas);
        }
        protected override void _Release()
        {
        }

#if UNITY_EDITOR
        protected override void OnBindSerializedField()
        {
            _canvas = GetComponentInChildren<Canvas>(true);
            _screenInputHandler = GetComponentInChildren<ScreenInputController>(true);
        }
#endif // UNITY_EDITOR
    }
}