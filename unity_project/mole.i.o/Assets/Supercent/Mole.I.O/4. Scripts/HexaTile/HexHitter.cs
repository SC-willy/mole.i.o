using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class HexHitter : MonoBehaviour
    {
        public LayerMask tileLayer;
        public WaveController waveController;
        public Color playerColor = Color.red;
        public int maxWaveRange = 3; // 최대 웨이브 퍼지는 거리 설정
        [SerializeField] bool _isPlayer;

        public void AddRange(int range = 1)
        {
            maxWaveRange += range;
        }
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayer))
                {
                    HexTile tile = hit.collider.GetComponent<HexTile>();
                    if (tile != null)
                    {
                        waveController.SpreadWave(tile.hexCoords, playerColor, maxWaveRange, _isPlayer);
                    }
                }
            }
        }
    }
}