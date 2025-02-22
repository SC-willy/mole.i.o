using System;
using UnityEngine;
namespace Supercent.PrisonLife.Playable003
{
    public class MonoStacker : MonoBehaviour
    {
        [SerializeField] protected Stacker _stacker;
        public Stacker Stacker => _stacker;

        public void SetOnStack(Action action, bool _isRegist)
        {
            if (_isRegist)
                _stacker.Data.OnStack += action;
            else
                _stacker.Data.OnStack -= action;
        }

        public void SetOnRelease(Action action, bool _isRegist)
        {
            if (_isRegist)
                _stacker.Data.OnRelease += action;
            else
                _stacker.Data.OnRelease -= action;
        }
    }
}