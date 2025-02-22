using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;

namespace Supercent.Util.Editor
{
    public class Menu : ScriptableObject
    {
        [MenuItem("Supercent/Util/Clear PlayerPrefab")]
        static void Clear_PlayerPrefab()
        {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Supercent/Util/Delete persistentDataPath")]
        static void Delete_PersistentDataPath()
        {
            Directory.Delete(Application.persistentDataPath, true);
        }

        [MenuItem("Supercent/Util/Screen Capture")]
        static void CurrentScreenCapture01() => ScreenCaptureJob(1);
        [MenuItem("Supercent/Util/Screen Capture x4 &C")]
        static void CurrentScreenCapture04() => ScreenCaptureJob(4);

        static void ScreenCaptureJob(int superSize)
        {
            var directory   = new DirectoryInfo($"{Application.dataPath}/../../");;
            var filename    = $"{directory.FullName}ScreenCapture_{DateTime.Now:yyyyMMdd_HHmmss}.png";

            ScreenCapture.CaptureScreenshot(filename, superSize);
            Debug.Log($"Screen Capture : {filename}");
        }

        [MenuItem("Supercent/Util/Camera Capture")]
        static void CurrentCameraCapture01() => CameraCaptureJob(1f);
        [MenuItem("Supercent/Util/Camera Capture x4 #&C")]
        static void CurrentCameraCapture04() => CameraCaptureJob(4f);

        static void CameraCaptureJob(float ratio)
        {
            if (ratio <= 0)
            {
                Debug.LogAssertion($"{nameof(CameraCaptureJob)} : {nameof(ratio)}({ratio}) <= 0");
                return;
            }

            var cam = Camera.main;
            if (cam == null)
            {
                Debug.Log("$Camera Capture : Not found main camera");
                return;
            }
            if (!cam.enabled)
            {
                Debug.Log("$Camera Capture : Disabled main camera");
                return;
            }

            var directory = new DirectoryInfo($"{Application.dataPath}/../../"); ;
            var filename = $"{directory.FullName}CameraCapture_{DateTime.Now:yyyyMMdd_HHmmss}.png";

            byte[] binPng = null;
            var rtex = RenderTexture.GetTemporary((int)(Screen.width * ratio),
                                                  (int)(Screen.height * ratio),
                                                  32,
                                                  RenderTextureFormat.ARGB32);
            {
                var rtexOld = cam.targetTexture;
                cam.targetTexture = rtex;
                cam.Render();
                cam.targetTexture = rtexOld;

                rtexOld = RenderTexture.active;
                RenderTexture.active = rtex;
                var tex = new Texture2D(rtex.width, rtex.height, TextureFormat.RGBA32, false, true);
                tex.ReadPixels(new Rect(0f, 0f, rtex.width, rtex.height), 0, 0, false);
                tex.Apply(false, false);
                RenderTexture.active = rtexOld;

                binPng = tex.EncodeToPNG();
            }
            RenderTexture.ReleaseTemporary(rtex);

            File.WriteAllBytes(filename, binPng);
            Debug.Log($"Camera Capture : {filename}");
        }

        [MenuItem("Supercent/Util/Print Resolution Info &R")]
        static void Print_ResolutionInfo()
        {
            Debug.Log($"[Resolution Info]\n" +
                      $"Screen Area : (x:0.00, y:0.00, width:{Screen.width:0.00}, height:{Screen.height:0.00})\n" +
                      $"Safe Area : {Screen.safeArea}\n" +
                      $"DPI : {Screen.dpi}");
        }
    }
}