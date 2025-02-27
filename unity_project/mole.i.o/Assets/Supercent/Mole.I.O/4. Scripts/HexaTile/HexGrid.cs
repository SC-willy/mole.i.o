using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class HexGrid : IStartable, IUpdateable, IBindable
    {
        [Header("Tile Map")]
        [SerializeField] MonoBehaviour _coroutineOwner;
        [SerializeField] GameObject _hexTilePrefab; // 3D 핵사타일 프리팹
        [SerializeField] Transform _mapParents;
        [SerializeField] Color _emptyTileColor;
        [SerializeField] Color _hitColor;
        [SerializeField] int _gridWidth = 10;
        [SerializeField] int _gridHeight = 10;
        [SerializeField] float _tileWidth = 1.7f;
        [SerializeField] float _tileHeight = 1.7f;
        [SerializeField] float _waveDelay = 0.05f;

        Dictionary<Vector2Int, TileData> _tileDict = new Dictionary<Vector2Int, TileData>();
        Dictionary<Vector2Int, MaterialPropertyBlock> tileProperties = new Dictionary<Vector2Int, MaterialPropertyBlock>();

        public void StartSetup()
        {
            GenerateHexGrid();
        }

        public void UpdateManualy(float dt)
        {
            foreach (var tile in tileProperties)
            {
                TileData tileObj = GetTileData(tile.Key);
                if (tileObj == null) continue;

                MaterialPropertyBlock props = tile.Value;
                props.SetFloat("_GlobalTime", Time.time);
                tileObj.Renderer.SetPropertyBlock(props);
            }
        }

        #region TileMap
        private void GenerateHexGrid()
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    Vector2Int hexCoords = new Vector2Int(x, y);
                    GameObject hexTile = GameObject.Instantiate(_hexTilePrefab, HexToWorldPosition(hexCoords), Quaternion.identity, _mapParents);
                    hexTile.name = $"Tile {x}, {y}";

                    HexTile tileScript = hexTile.GetComponent<HexTile>();
                    if (tileScript != null)
                    {
                        tileScript.InitTile(hexCoords, Color.white);
                        _tileDict.Add(hexCoords, tileScript.TileData);
                    }
                }
            }
        }

        public TileData GetTileData(Vector2Int coords) => _tileDict.ContainsKey(coords) ? _tileDict[coords] : null;

        public TileData GetTileDataByPos(Vector3 position)
        {
            Vector2Int hexCoords = WorldToHexCoords(position);

            if (_tileDict.TryGetValue(hexCoords, out TileData tileData))
            {
                return tileData;
            }
            return null;
        }

        public Vector2Int WorldToHexCoords(Vector3 worldPosition)
        {
            float x = worldPosition.x / _tileWidth;
            float y = worldPosition.z / (_tileHeight * 0.85f);

            // 반올림하여 가장 가까운 타일 좌표로 변환
            int hexX = Mathf.RoundToInt(x);
            int hexY = Mathf.RoundToInt(y);

            // 짝수/홀수 행 보정 (오프셋 좌표계 적용)
            float xOffset = (hexY % 2 == 0) ? 0f : 0.5f;
            hexX = Mathf.RoundToInt(x - xOffset);

            return new Vector2Int(hexX, hexY);
        }

        Vector3 HexToWorldPosition(Vector2Int hexCoords)
        {
            float xOffset = hexCoords.y % 2 == 0 ? 0f : _tileWidth * 0.5f;
            float x = hexCoords.x * _tileWidth + xOffset;
            float y = hexCoords.y * _tileHeight * 0.85f;
            return new Vector3(x, 0, y);
        }
        public List<Vector2Int> GetHexNeighbors(Vector2Int hexCoords)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();

            // 홀수 줄과 짝수 줄에 따라 다름
            Vector2Int[] evenOffsets = {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(-1, 1), new Vector2Int(-1, -1)
        };

            Vector2Int[] oddOffsets = {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, 1), new Vector2Int(1, -1)
        };

            Vector2Int[] offsets = (hexCoords.y % 2 == 0) ? evenOffsets : oddOffsets;

            foreach (Vector2Int offset in offsets)
            {
                Vector2Int neighbor = hexCoords + offset;
                if (_tileDict.ContainsKey(neighbor))
                    neighbors.Add(neighbor);
            }

            return neighbors;
        }
        #endregion


        #region WaveControl
        public void SpreadWave(ITileXpGetter hitter, Vector2Int startTile, Color playerColor, int maxRange, int userCode = TileData.PLAYER_CODE)
        {
            _coroutineOwner.StartCoroutine(WavePropagation(hitter, startTile, playerColor, maxRange, userCode));
        }

        IEnumerator WavePropagation(ITileXpGetter hitter, Vector2Int startTile, Color playerColor, int maxRange, int userCode = TileData.PLAYER_CODE)
        {
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            Dictionary<Vector2Int, int> distanceMap = new Dictionary<Vector2Int, int>(); // 거리 추적
            queue.Enqueue(startTile);
            distanceMap[startTile] = 0;

            while (queue.Count > 0)
            {
                int waveSize = queue.Count;

                for (int i = 0; i < waveSize; i++)
                {
                    Vector2Int current = queue.Dequeue();
                    TileData curTile = GetTileData(current);
                    int currentDistance = distanceMap[current];

                    if (currentDistance > maxRange) continue; // 최대 거리 초과 시 무시

                    if (curTile.Owner == TileData.EMPTY_CODE)
                    {
                        hitter.GetXp(curTile.Xp);
                        curTile.Xp = 0;
                    }

                    if (curTile.Owner != userCode && curTile.Owner != TileData.EMPTY_CODE)
                    {
                        curTile.Owner = TileData.EMPTY_CODE;
                        curTile.Xp = 1;
                        StartWave(current, Time.time, _emptyTileColor);
                    }
                    else
                    {
                        curTile.Owner = userCode;
                        StartWave(current, Time.time, playerColor);
                    }


                    foreach (Vector2Int neighbor in GetHexNeighbors(current))
                    {
                        if (!distanceMap.ContainsKey(neighbor))
                        {
                            queue.Enqueue(neighbor);
                            distanceMap[neighbor] = currentDistance + 1;
                        }
                    }
                }
                yield return new WaitForSeconds(_waveDelay);

            }
        }
        #endregion


        #region ShaderControl
        public void StartWave(Vector2Int tilePos, float waveStartTime, Color newColor)
        {
            if (!tileProperties.ContainsKey(tilePos))
            {
                tileProperties[tilePos] = new MaterialPropertyBlock();
            }

            TileData tileObj = GetTileData(tilePos);
            if (tileObj == null) return;

            MaterialPropertyBlock props = tileProperties[tilePos];
            props.SetFloat("_WaveStartTime", waveStartTime);
            props.SetColor("_TargetColor", newColor);
            props.SetColor("_BaseColor", _hitColor);
            tileObj.Renderer.SetPropertyBlock(props);
        }

        public void Bind(MonoBehaviour mono)
        {
            _coroutineOwner = mono;
        }

        #endregion
    }
}
