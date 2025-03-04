using System;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class HexHammer : MonoBehaviour
    {
        public event Action OnHit;

        public int PlayerCode => _playerCode;
        HexGrid _mapInfo;
        [SerializeField] Transform _hitTr;
        [SerializeField] Color playerColor = Color.red;
        [SerializeField] float _hitDuration = 1f;
        [SerializeField] int _maxWaveRange = 3; // 최대 웨이브 퍼지는 거리 설정
        [SerializeField] int _playerCode = 1;

        float _lastHitTime = 1;
        bool _isUpdate = false;

        public void ActiveAttack(bool on) => _isUpdate = on;
        public void ReduceHitDuration(float timeValue) => _hitDuration -= timeValue;
        public void SetPlayerCode(int code) => _playerCode = code;
        public void SetMapInfo(HexGrid map) => _mapInfo = map;
        public void SetRange(int range)
        {
            if (range >= 1)
                _maxWaveRange = range;
            else
                _maxWaveRange = 1;
        }
        void Update()
        {
            if (!_isUpdate)
                return;

            if (_lastHitTime + _hitDuration > Time.time)
                return;

            TryHit();
        }

        private void TryHit()
        {
            TileData tile = _mapInfo.GetTileDataByPos(_hitTr.position);

            if (tile == null)
                return;
            if (tile.Owner == _playerCode)
                return;

            OnHit?.Invoke();
            _lastHitTime = Time.time;
        }

        public void HitTile(ITileXpGetter hitter)
        {
            TileData tile = _mapInfo.GetTileDataByPos(_hitTr.position);

            if (tile == null)
                return;
            _mapInfo.SpreadWave(hitter, tile.HexCoords, playerColor, _maxWaveRange, _playerCode);
        }
    }
}