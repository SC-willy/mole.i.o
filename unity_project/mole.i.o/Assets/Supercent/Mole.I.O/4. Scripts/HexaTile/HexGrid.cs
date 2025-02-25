using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class HexGrid : MonoBehaviour
    {
        public GameObject hexTilePrefab; // 3D 핵사타일 프리팹
        public int gridWidth = 10;
        public int gridHeight = 10;
        public float _tileWidth = 1.7f;
        public float _tileHeight = 1.7f;

        public Dictionary<Vector2Int, TileData> tileDict = new Dictionary<Vector2Int, TileData>();

        void Start()
        {
            GenerateHexGrid();
        }

        void GenerateHexGrid()
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector2Int hexCoords = new Vector2Int(x, y);
                    GameObject hexTile = Instantiate(hexTilePrefab, HexToWorldPosition(hexCoords), Quaternion.identity, transform);
                    hexTile.name = $"Tile {x}, {y}";

                    HexTile tileScript = hexTile.GetComponent<HexTile>();
                    if (tileScript != null)
                    {
                        tileScript.InitTile(hexCoords, Color.white);
                        tileDict.Add(hexCoords, tileScript.TileData);
                    }
                }
            }
        }

        public TileData GetTileData(Vector2Int coords)
        {
            return tileDict.ContainsKey(coords) ? tileDict[coords] : null;
        }

        public TileData GetTileDataByPos(Vector3 position)
        {
            Vector2Int hexCoords = WorldToHexCoords(position);

            if (tileDict.TryGetValue(hexCoords, out TileData tileData))
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
                if (tileDict.ContainsKey(neighbor))
                    neighbors.Add(neighbor);
            }

            return neighbors;
        }
    }
}