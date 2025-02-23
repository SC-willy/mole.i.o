using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class HexShaderController : MonoBehaviour
    {
        public Material hexMaterial;
        private Dictionary<Vector2Int, MaterialPropertyBlock> tileProperties = new Dictionary<Vector2Int, MaterialPropertyBlock>();
        public HexGrid hexGrid;

        void Update()
        {
            foreach (var tile in tileProperties)
            {
                TileData tileObj = hexGrid.GetTileData(tile.Key);
                if (tileObj == null) continue;

                MaterialPropertyBlock props = tile.Value;
                props.SetFloat("_GlobalTime", Time.time);
                tileObj.renderer.SetPropertyBlock(props);
            }
        }

        public void StartWave(Vector2Int tilePos, float waveStartTime, Color newColor)
        {
            if (!tileProperties.ContainsKey(tilePos))
            {
                tileProperties[tilePos] = new MaterialPropertyBlock();
            }

            TileData tileObj = hexGrid.GetTileData(tilePos);
            if (tileObj == null) return;

            MaterialPropertyBlock props = tileProperties[tilePos];
            props.SetFloat("_WaveStartTime", waveStartTime);
            props.SetColor("_Color", newColor);  // 🔥 타일 색상 적용!
            tileObj.renderer.SetPropertyBlock(props);
        }
    }
}