using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class HexTile : MonoBehaviour
    {
        [SerializeField] TileData _tileData;
        public TileData TileData => _tileData;

        public void InitTile(Vector2Int coords, Color ownerColor)
        {
            _tileData.SetTileData(coords, ownerColor); // 타일 데이터 생성
        }
    }
}