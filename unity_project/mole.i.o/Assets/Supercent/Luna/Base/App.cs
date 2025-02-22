using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Supercent.Base
{
    public class App : MonoBehaviour
    {
        public static float ScreenAspect => Screen.width / (float)Screen.height;
        public static bool IsLandscape => 1f <= ScreenAspect;

        [LunaPlaygroundField("Info", 0, "App")]
        [SerializeField] bool visibleInfo = false;
        [LunaPlaygroundField("Log", 1, "App")]
        [SerializeField] bool visibleLogPanel = false;

        [LunaPlaygroundField("Time Scale", 10, "App")]
        [Range(0.01f, 10f)]
        [SerializeField] float timeScale = 1f;

        Canvas canvas = null;
        CanvasScaler scaler = null;
        GraphicRaycaster raycaster = null;

        TMP_Text textPlay = null;
        float stampPlaying = 0f;

        TMP_Text textFPS = null;
        long stampTimeFPS = 0;
        int FPS = 0;



        void Awake()
        {
            if (Application.isPlaying)
            {
                stampPlaying = Time.realtimeSinceStartup;

                var isEditor = Application.isEditor;
                SetUI();
                if (isEditor || visibleInfo) SetInfo();
                if (isEditor || visibleLogPanel)
                {
                    AppLogPanel.Initialize(transform);
                    AppLogPanel.Visible = visibleLogPanel;
                }
                Time.timeScale = timeScale;

                AudioManager.Initialize(transform);
            }
        }

        void Update()
        {
            if (visibleInfo)
            {
                if (textPlay != null)
                    textPlay.text = $"Play {(Time.realtimeSinceStartup - stampPlaying):F1}";

                var sec = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
                if (stampTimeFPS == sec)
                    FPS += 1;
                else
                {
                    if (stampTimeFPS + 1 < sec)
                            FPS = 0;

                    if (textFPS != null)
                    {
                        textFPS.text = $"FPS {FPS}";
                        textFPS.color = 29 < FPS ? Color.green
                                      : 9 < FPS ? Color.yellow
                                      : Color.red;
                    }

                    stampTimeFPS = sec;
                    FPS = 1;
                }
            }
        }


        void SetUI()
        {
            var layerUI = LayerMask.NameToLayer("UI");

            if (AppUtil.GetOrAddComponent(transform, out canvas))
            {
                canvas.gameObject.layer = layerUI;
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.pixelPerfect = false;
                canvas.sortingOrder = 32700;
                canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
            }

            if (AppUtil.GetOrAddComponent(canvas, out scaler))
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                scaler.referencePixelsPerUnit = 100;
                scaler.referenceResolution = new Vector2(900f, 1600f);
            }

            if (AppUtil.GetOrAddComponent(canvas, out raycaster))
            {
                raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
            }
        }

        void SetInfo()
        {
            SetTextObject("PLAY", Vector2.zero, out textPlay);
            if (textPlay != null)
                textPlay.color = Color.green;
            SetTextObject("FPS", new Vector2(0f, -40f), out textFPS);
        }

        void SetTextObject(string name, Vector2 offset, out TMP_Text result)
        {
            result = null;
            var layerUI = LayerMask.NameToLayer("UI");

            if (AppUtil.GetOrAddComponent(canvas, name, out RectTransform rect))
            {
                rect.gameObject.SetActive(true);
                rect.gameObject.layer = layerUI;
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0f, 1f);
                rect.anchoredPosition = Vector2.zero;
            }
            if (AppUtil.GetOrAddComponent(rect, out TextMeshProUGUI text))
            {
                result = text;
                text.fontSize = 42;
                text.alignment = TextAlignmentOptions.TopLeft;
                SetTextOption(text);

                if (offset != Vector2.zero)
                    text.rectTransform.anchoredPosition += offset;
            }
            if (AppUtil.GetOrAddComponent(text, out ContentSizeFitter fitter))
            {
                fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }

        void SetTextOption(TMP_Text text)
        {
            text.font = TMP_Settings.defaultFontAsset;
            text.fontStyle = FontStyles.Bold;
            text.richText = false;

            text.enableWordWrapping = true;
            text.overflowMode = TextOverflowModes.Overflow;
            text.raycastTarget = false;
            text.maskable = true;
        }



#if UNITY_EDITOR
        void Reset()
        {
            name = "APP";
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        void OnValidate()
        {
            if (Application.isPlaying)
            {
                if (textFPS != null) textFPS.enabled = visibleInfo;
                if (textPlay != null) textPlay.enabled = visibleInfo;

                AppLogPanel.Visible = visibleLogPanel;
                Time.timeScale = timeScale;
            }
        }
#endif// UNITY_EDITOR
    }
}