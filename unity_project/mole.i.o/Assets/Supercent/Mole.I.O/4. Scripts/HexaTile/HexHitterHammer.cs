using System;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class HexHitterHammer : MonoBehaviour
    {
        public event Action OnHit;
        [SerializeField] HexGrid _hexGrid;
        [SerializeField] WaveController _waveController;
        [SerializeField] Transform _hitTr;
        [SerializeField] Color playerColor = Color.red;
        [SerializeField] float _hitDuration = 1f;
        [SerializeField] int _maxWaveRange = 3; // 최대 웨이브 퍼지는 거리 설정
        [SerializeField] bool _isPlayer = false;

        float _lastHitTime = 1;


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
            TileData tile = _hexGrid.GetTileDataByPos(_hitTr.position);

            if (tile == null)
                return;
            if (tile.xp == 0)
                return;

            OnHit?.Invoke();

        }

        public void HitTile()
        {
            TileData tile = _hexGrid.GetTileDataByPos(_hitTr.position);
            _waveController.SpreadWave(tile.hexCoords, playerColor, _maxWaveRange, _isPlayer);
            _lastHitTime = Time.time;
        }
    }
}