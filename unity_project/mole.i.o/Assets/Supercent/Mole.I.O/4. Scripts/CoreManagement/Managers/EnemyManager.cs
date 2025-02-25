using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class EnemyManager : InitManagedBehaviorBase
    {
        [SerializeField] EnemyController[] _enemies;
        protected override void _Init()
        {
            for (int i = 0; i < _enemies.Length; i++)
            {
                _enemies[i].OnHit += StartHitAction;
            }
        }

        private void StartHitAction(EnemyController target)
        {
            target.gameObject.SetActive(false);
        }

        protected override void _Release()
        {
        }

#if UNITY_EDITOR

        protected override void OnBindSerializedField()
        {
            _enemies = GetComponentsInChildren<EnemyController>();
        }
#endif //UNITY_EDITOR
    }
}