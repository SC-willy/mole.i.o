using System;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class HexHammer : MonoBehaviour
    {
        public event Action OnHit;
        HexGrid _mapInfo;
        [SerializeField] Transform _hitTr;
        [SerializeField] Color playerColor = Color.red;
        [SerializeField] float _hitDuration = 1f;
        [SerializeField] int _maxWaveRange = 3; // 최대 웨이브 퍼지는 거리 설정
        [SerializeField] bool _isPlayer = false;

        float _lastHitTime = 1;

        public void SetMapInfo(HexGrid map) => _mapInfo = map;
        public void AddRange(int range = 1)
        {
            _maxWaveRange += range;
        }
        void Update()
        {
            if (_lastHitTime + _hitDuration > Time.time)
                return;

            TryHit();
        }

        private void TryHit()
        {
            TileData tile = _mapInfo.GetTileDataByPos(_hitTr.position);

            if (tile == null)
                return;
            if (tile.Xp == 0)
                return;

            OnHit?.Invoke();
            _lastHitTime = Time.time;
        }

        public void HitTile()
        {
            TileData tile = _mapInfo.GetTileDataByPos(_hitTr.position);
            _mapInfo.SpreadWave(tile.HexCoords, playerColor, _maxWaveRange, _isPlayer);
        }
    }
}