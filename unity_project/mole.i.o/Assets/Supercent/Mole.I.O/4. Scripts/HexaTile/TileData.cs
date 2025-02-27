using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    [Serializable]
    public class TileData
    {
        public const int EMPTY_CODE = 0;
        public const int PLAYER_CODE = -1;
        public int Owner;
        public Vector2Int HexCoords;
        public Color OwnerColor;
        public Renderer Renderer;
        public int Xp
        {
            get { return _xp; }
            set { _xp = _xp < 0 ? 0 : value; }
        }
        int _xp = 1;

        public void SetTileData(Vector2Int coords, Color color)
        {
            HexCoords = coords;
            OwnerColor = color;
        }
    }
}
