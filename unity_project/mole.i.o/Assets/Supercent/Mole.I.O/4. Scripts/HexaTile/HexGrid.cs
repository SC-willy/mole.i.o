using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class HexGrid : MonoBehaviour
    {
        public GameObject hexTilePrefab; // 핵사타일 프리팹
        public int gridWidth = 10;
        public int gridHeight = 10;

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

                    // 타일 생성
                    GameObject hexTile = Instantiate(hexTilePrefab, HexToWorldPosition(hexCoords), Quaternion.identity, transform);
                    hexTile.name = $"Tile {x}, {y}";

                    // TileData 추가 (초기 색은 흰색)
                    TileData newTileData = new TileData(hexCoords, Color.white);
                    tileDict.Add(hexCoords, newTileData);

                    // 타일에 좌표 정보 저장
                    TileData tileScript = hexTile.GetComponent<TileData>();
                    if (tileScript != null)
                    {
                        tileScript.hexCoords = hexCoords;
                    }
                }
            }
        }

        Vector3 HexToWorldPosition(Vector2Int hexCoords)
        {
            float xOffset = hexCoords.y % 2 == 0 ? 0f : 0.5f; // 홀수 줄 오프셋
            float x = hexCoords.x + xOffset;
            float y = hexCoords.y * 0.85f; // 높이 조절
            return new Vector3(x, 0, y);
        }

        public List<Vector2Int> GetHexNeighbors(Vector2Int hexCoords)
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();

            // 홀수 줄과 짝수 줄에 따라 다름
            Vector2Int[] evenOffsets = {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, -1), new Vector2Int(-1, -1)
        };

            Vector2Int[] oddOffsets = {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, 1), new Vector2Int(-1, 1)
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