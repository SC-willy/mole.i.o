using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class HexTile : MonoBehaviour
    {
        public Vector2Int hexCoords;  // 타일의 핵사 좌표
        [SerializeField] TileData _tileData;     // 타일의 데이터 (비 MonoBehaviour)
        public TileData TileData => _tileData;

        public void InitTile(Vector2Int coords, Color ownerColor)
        {
            hexCoords = coords;
            _tileData.SetTileData(coords, ownerColor); // 타일 데이터 생성
        }
    }
}