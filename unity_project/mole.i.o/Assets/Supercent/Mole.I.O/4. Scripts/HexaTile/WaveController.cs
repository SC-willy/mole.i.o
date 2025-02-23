using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class WaveController : MonoBehaviour
    {
        public HexGrid hexGrid;  // 핵사타일 관리 객체

        public void SpreadWave(Vector2Int startTile, Color newColor)
        {
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

            queue.Enqueue(startTile);
            visited.Add(startTile);

            float startTime = Time.time;

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();

                if (hexGrid.tileDict.TryGetValue(current, out TileData tile))
                {
                    tile.waveStartTime = startTime;
                    tile.ownerColor = newColor;
                    hexGrid.tileDict[current] = tile;
                }

                foreach (Vector2Int neighbor in hexGrid.GetHexNeighbors(current))
                {
                    if (!visited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }
        }
    }
}