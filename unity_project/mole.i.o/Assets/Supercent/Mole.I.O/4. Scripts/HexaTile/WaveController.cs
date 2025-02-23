using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class WaveController : MonoBehaviour
    {
        public HexShaderController shaderController;
        public HexGrid hexGrid;
        public float waveDelay = 0.05f;

        public void SpreadWave(Vector2Int startTile, Color playerColor, int maxRange)
        {
            StartCoroutine(WavePropagation(startTile, playerColor, maxRange));
        }

        IEnumerator WavePropagation(Vector2Int startTile, Color playerColor, int maxRange)
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
                    int currentDistance = distanceMap[current];

                    if (currentDistance > maxRange) continue; // 최대 거리 초과 시 무시

                    shaderController.StartWave(current, Time.time, playerColor);

                    foreach (Vector2Int neighbor in hexGrid.GetHexNeighbors(current))
                    {
                        if (!distanceMap.ContainsKey(neighbor))
                        {
                            queue.Enqueue(neighbor);
                            distanceMap[neighbor] = currentDistance + 1;
                        }
                    }
                }

                yield return new WaitForSeconds(waveDelay);
            }
        }
    }
}
