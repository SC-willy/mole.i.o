using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class TileData
    {
        public Vector2Int hexCoords;   // 핵사타일 좌표 (큐브 좌표도 가능)
        public Color ownerColor;       // 현재 타일의 소유자 색깔
        public Renderer renderer;
        public float waveStartTime;    // 파동이 시작된 시간
        public bool isActive;          // 활성화 여부

        public void SetTileData(Vector2Int coords, Color color)
        {
            hexCoords = coords;
            ownerColor = color;
            waveStartTime = -1f;  // 초기에는 파동 없음
            isActive = false;
        }
    }
}
