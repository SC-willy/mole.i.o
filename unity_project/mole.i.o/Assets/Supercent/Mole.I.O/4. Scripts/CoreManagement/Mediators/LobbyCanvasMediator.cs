
using System.Collections;
using System.Text;
using Supercent.MoleIO.Playable001;
using Supercent.Util;
using TMPro;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class LobbyCanvasMediator : InitManagedBehaviorBase
    {
        [SerializeField] Canvas _canvas;
        [SerializeField] ScreenInputController _screenInputHandler;
        StringBuilder _stringBuilder = new StringBuilder();
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