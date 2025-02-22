using System.Collections;
using UnityEngine;


namespace Supercent.Slime.Playable004
{
    public class SceneToImage : MonoBehaviour
    {
        public string screenshotPath = "Assets/Screenshots/";
        WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                StartCoroutine(CoTakeScreenshotCoroutine());
            }
        }

        public void DoScreenShot()
        {
            StartCoroutine(CoTakeScreenshotCoroutine());
        }

        IEnumerator CoTakeScreenshotCoroutine()
        {
            yield return frameEnd;

            int width = Screen.width;
            int height = Screen.height;

            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture.Apply();

            Color[] pixels = texture.GetPixels();
            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i] == Color.black)
                {
                    pixels[i] = Color.clear;
                }
            }
            texture.SetPixels(pixels);
            texture.Apply();

            byte[] bytes = texture.EncodeToPNG();
            string fileName = "screenshot_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
            string fullPath = screenshotPath + fileName;
            System.IO.File.WriteAllBytes(fullPath, bytes);
            Debug.Log("Screenshot saved to: " + fullPath);
        }
    }
}