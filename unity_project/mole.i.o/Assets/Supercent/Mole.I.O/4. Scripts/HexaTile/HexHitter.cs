using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class HexHitter : MonoBehaviour
    {
        public LayerMask tileLayer;  // 핵사타일이 있는 레이어
        public WaveController waveController; // 파도 전파를 담당하는 스크립트
        public Color _color = Color.white;

        void Update()
        {
            if (Input.GetMouseButtonDown(0)) // 터치 또는 마우스 클릭 감지
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayer))
                {
                    TileData tile = hit.collider.GetComponent<TileData>(); // 타일 스크립트 가져오기

                    if (tile != null)
                    {
                        Debug.Log("망치 친 타일: " + tile.hexCoords);
                        waveController.SpreadWave(tile.hexCoords, _color); // 웨이브 시작!
                    }
                }
            }
        }
    }
}