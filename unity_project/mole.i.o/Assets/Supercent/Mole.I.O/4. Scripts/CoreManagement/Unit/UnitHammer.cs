using System;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class UnitHammer : IInitable, ITileXpGetter
    {
        const int ATTACKABLE_COMBO = 5;
        readonly private static int _animBoolHit = Animator.StringToHash("Hit");

        ITileXpGetter _user;
        [SerializeField] HexHammer _hitter;
        [SerializeField] AnimEventContainer _animEvent;
        [SerializeField] Animator _animator = null;
        [SerializeField] Transform _attackTr;
        [SerializeField] LayerMask _mask;
        [SerializeField] GameObject[] _chargeObjs;
        [SerializeField] float _killRange = 3f;
        int _combo = 0;

        public void Init()
        {
            _hitter.OnHit += PlayAttackAnim;
            _animEvent.OnAnimEvent += CheckEnemy;
        }

        public void Release()
        {
            _hitter.OnHit -= PlayAttackAnim;
            _animEvent.OnAnimEvent -= CheckEnemy;
        }
        public void RegistUser(ITileXpGetter user)
        {
            _user = user;
        }

        private void PlayAttackAnim() => _animator.SetTrigger(_animBoolHit);

        private void CheckEnemy()
        {
            _hitter.HitTile(this);

            if (_combo < ATTACKABLE_COMBO)
            {
                _combo++;

                if (_combo == ATTACKABLE_COMBO)
                    ReadyCharge();

                return;
            }

            Collider[] others = Physics.OverlapSphere(_attackTr.position, _killRange, _mask);

            for (int i = 0; i < others.Length; i++)
            {
                var enemy = ColDict.GetData(others[i]);

                if (enemy == null)
                    continue;

                enemy.GetDamage();
                ReleaseCharge();
            }
        }

        private void ReadyCharge()
        {
            for (int i = 0; i < _chargeObjs.Length; i++)
            {
                _chargeObjs[i].SetActive(true);
            }
        }


        private void ReleaseCharge()
        {
            if (_combo <= 0)
                return;

            _combo = 0;
            for (int i = 0; i < _chargeObjs.Length; i++)
            {
                _chargeObjs[i].SetActive(false);
            }
        }

        public void GetXp(int xp)
        {
            _user.GetXp(xp);
        }

        public void AddRange(int range = 0) => _hitter.AddRange(range);
    }
}