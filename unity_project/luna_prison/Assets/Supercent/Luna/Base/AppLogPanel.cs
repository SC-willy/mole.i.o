using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Supercent.Base
{
    public static class AppLogPanel
    {
        const int MaxCount = 256;

        static readonly StringBuilder sb = new StringBuilder();
        static readonly Queue<(int, string)> logs = new Queue<(int, string)>(MaxCount);
        static readonly List<TMP_Text> texts = new List<TMP_Text>(MaxCount);

        static Canvas canvas = null;
        static GraphicRaycaster raycaster = null;

        static Button btnLog = null;
        static Button btnCopy = null;
        static Button btnTop = null;
        static Button btnBottom = null;
        static ScrollRect scroll = null;

        public static string TimeFormat = "mm:ss";

        static bool visible = true;
        public static bool Visible
        {
            set
            {
                visible = value;
                if (canvas != null)
                    canvas.enabled = value;
            }
            get => canvas == null ? visible : visible = canvas.enabled;
        }



        public static void Initialize(Transform parent)
        {
            Reset();

            Application.logMessageReceived -= LogMessageReceived;
            Application.logMessageReceived += LogMessageReceived;

            var root = parent == null
                     ? AppUtil.GetAppObejct().transform
                     : parent;
            SetUI(root);

            if (canvas != null)
                canvas.enabled = true;
            if (btnLog != null)
            {
                btnLog.gameObject.SetActive(true);
                btnLog.onClick.RemoveAllListeners();
                btnLog.onClick.AddListener(() =>
                {
                    if (scroll == null) return;

                    bool isOn = !scroll.gameObject.activeSelf;
                    scroll.gameObject.SetActive(isOn);
                    if (isOn)
                    {
                        ViewUpdate();
                        if (btnLog != null
                         && btnLog.image != null)
                            btnLog.image.color = Color.white;
                        if (scroll != null)
                            scroll.verticalNormalizedPosition = 0;
                    }
                });
            }
            if (btnCopy != null)
            {
                btnCopy.onClick.RemoveAllListeners();
                btnCopy.onClick.AddListener(() =>
                {
                    sb.Clear();
                    {
                        foreach (var log in logs)
                            sb.AppendLine(log.Item2);
                        GUIUtility.systemCopyBuffer = sb.ToString();
                    }
                    sb.Clear();
                });
            }
            if (btnTop != null)
            {
                btnTop.onClick.RemoveAllListeners();
                btnTop.onClick.AddListener(() =>
                {
                    if (scroll != null)
                        scroll.verticalNormalizedPosition = 1f;
                });
            }
            if (btnBottom != null)
            {
                btnBottom.onClick.RemoveAllListeners();
                btnBottom.onClick.AddListener(() =>
                {
                    if (scroll != null)
                        scroll.verticalNormalizedPosition = 0f;
                });
            }
        }

        static void Reset()
        {
            logs.Clear();
            ViewUpdate();
        }

        static void SetUI(Transform parent)
        {
            var layerUI = LayerMask.NameToLayer("UI");

            #region Canvas
            if (AppUtil.GetOrAddComponent<RectTransform>(parent, "PNL_LOG", out var rectLog))
                rectLog.SetFullStretch();

            if (AppUtil.GetOrAddComponent(rectLog, out canvas))
            {
                canvas.enabled = visible;
                canvas.gameObject.layer = layerUI;
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.pixelPerfect = false;
                canvas.overrideSorting = true;
                canvas.sortingOrder = short.MaxValue;
                canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
            }

            if (AppUtil.GetOrAddComponent(canvas, out raycaster))
            {
                raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
            }
            #endregion// Canvas


            #region Scroll
            if (AppUtil.GetOrAddComponent(canvas, "Scroll", out RectTransform formScroll))
            {
                formScroll.gameObject.SetActive(false);
                formScroll.gameObject.layer = layerUI;
                formScroll.SetFullStretch();
            }
            if (AppUtil.GetOrAddComponent(formScroll, out scroll))
            {
                scroll.horizontal = true;
                scroll.vertical = true;
                scroll.movementType = ScrollRect.MovementType.Clamped;
            }
            if (AppUtil.GetOrAddComponent(formScroll, out RawImage rawImage))
            {
                rawImage.color = new Color(0, 0, 0, 0.35f);
                rawImage.raycastTarget = true;
                rawImage.maskable = true;
            }
            AppUtil.GetOrAddComponent<RectMask2D>(formScroll, out _);

            if (AppUtil.GetOrAddComponent(formScroll, "Viewport", out RectTransform formViewport))
            {
                if (scroll != null)
                    scroll.viewport = formViewport;
                formViewport.gameObject.layer = layerUI;
                formViewport.SetFullStretch();
                formViewport.sizeDelta = new Vector2(-10, -10);
            }

            // Copy button
            btnCopy = MakeSamllButton(formScroll, "BtnCopy", "C");
            if (btnCopy != null)
            {
                var rect = btnCopy.targetGraphic.rectTransform;
                rect.SetAnchor(AppUtil.AnchorType.LeftTop);
                rect.SetPivot(AppUtil.PivotType.LeftTop);
                rect.anchoredPosition = new Vector2(5, -5);
            }

            // Top button
            btnTop = MakeSamllButton(formScroll, "BtnTop", "T");
            if (btnTop != null)
            {
                var rect = btnTop.targetGraphic.rectTransform;
                rect.SetAnchor(AppUtil.AnchorType.RightTop);
                rect.SetPivot(AppUtil.PivotType.RightTop);
                rect.anchoredPosition = new Vector2(-5, -5);
            }

            // Bottom button
            btnBottom = MakeSamllButton(formScroll, "BtnBotton", "B");
            if (btnBottom != null)
            {
                var rect = btnBottom.targetGraphic.rectTransform;
                rect.SetAnchor(AppUtil.AnchorType.RightTop);
                rect.SetPivot(AppUtil.PivotType.RightTop);
                rect.anchoredPosition = new Vector2(-5 - rect.sizeDelta.x - 5, -5);
            }

            // Content
            if (AppUtil.GetOrAddComponent(formViewport, "Content", out RectTransform formContent))
            {
                if (scroll != null)
                    scroll.content = formContent;
                formContent.gameObject.layer = layerUI;
                formContent.SetAnchor(AppUtil.AnchorType.BottomStretch);
                formContent.SetPivot(AppUtil.PivotType.LeftBottom);
            }

            if (AppUtil.GetOrAddComponent(formContent, out VerticalLayoutGroup layout))
            {
                layout.padding.top = 100;
                layout.spacing = 20f;
            }

            for (int index = texts.Count - 1; -1 < index; --index)
            {
                var text = texts[index];
                if (text == null)
                    texts.RemoveAt(index);
            }
            for (int index = texts.Count; index < MaxCount; ++index)
            {
                var objText = AppUtil.NewObject("Text", formContent);
                objText.SetActive(false);
                var text = objText.AddComponent<TextMeshProUGUI>();
                texts.Add(text);

                SetTextCommonOptions(text);
                text.fontSize = 42;
                text.lineSpacing = 1.1f;
                text.alignment = TextAlignmentOptions.TopLeft;
            }
            if (AppUtil.GetOrAddComponent(formContent, out ContentSizeFitter sizeFitter))
            {
                sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            #endregion// Scroll


            #region BtnLog
            if (AppUtil.GetOrAddComponent(canvas, "BtnLog", out RectTransform formBtnLog))
            {
                formBtnLog.gameObject.layer = layerUI;
                formBtnLog.SetAnchor(AppUtil.AnchorType.CenterTop);
                formBtnLog.SetPivot(AppUtil.PivotType.CenterTop);
                formBtnLog.anchoredPosition = new Vector2(0, -5);
                formBtnLog.sizeDelta = new Vector2(120, 80);
            }
            if (AppUtil.GetOrAddComponent(formBtnLog, out Image imgBtnLog))
                imgBtnLog.raycastTarget = true;
            if (AppUtil.GetOrAddComponent(formBtnLog, out btnLog))
            {
                btnLog.interactable = true;
                btnLog.transition = Selectable.Transition.ColorTint;
                btnLog.targetGraphic = imgBtnLog;
            }

            if (AppUtil.GetOrAddComponent(formBtnLog, "Text", out RectTransform formTextLog))
            {
                formTextLog.gameObject.layer = layerUI;
                formTextLog.SetFullStretch();
            }
            if (AppUtil.GetOrAddComponent(formTextLog, out TextMeshProUGUI textBtnLog))
            {
                SetTextCommonOptions(textBtnLog);
                textBtnLog.text = "Log";
                textBtnLog.fontSize = 48;
                textBtnLog.alignment = TextAlignmentOptions.Center;
                textBtnLog.color = Color.black;
            }
            #endregion// BtnLog
        }

        static Button MakeSamllButton(Transform parent, string name, string initial)
        {
            var layer = LayerMask.NameToLayer("UI");

            if (AppUtil.GetOrAddComponent(parent, name, out RectTransform formBtn))
            {
                formBtn.gameObject.layer = layer;
                formBtn.sizeDelta = new Vector2(80, 80);
            }
            if (AppUtil.GetOrAddComponent(formBtn, out Image imgBtn))
                imgBtn.raycastTarget = true;
            if (AppUtil.GetOrAddComponent(formBtn, out Button btn))
            {
                btn.interactable = true;
                btn.transition = Selectable.Transition.ColorTint;
                btn.targetGraphic = imgBtn;
            }

            if (AppUtil.GetOrAddComponent(formBtn, "Text", out RectTransform formText))
            {
                formText.gameObject.layer = layer;
                formText.SetFullStretch();
            }
            if (AppUtil.GetOrAddComponent(formText, out TextMeshProUGUI textBtn))
            {
                SetTextCommonOptions(textBtn);
                textBtn.text = initial;
                textBtn.fontSize = 48;
                textBtn.alignment = TextAlignmentOptions.Center;
                textBtn.color = Color.black;
            }

            return btn;
        }

        static void SetTextCommonOptions(TMP_Text text)
        {
            text.font = TMP_Settings.defaultFontAsset;
            text.fontStyle = FontStyles.Bold;
            text.richText = false;

            text.enableWordWrapping = true;
            text.overflowMode = TextOverflowModes.Overflow;
            text.raycastTarget = false;
            text.maskable = true;
        }


        

        static void LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            var code = type == LogType.Warning ? 1
                     : type == LogType.Error ? 2
                     : type == LogType.Exception ? 3
                     : type == LogType.Assert ? 4
                     : 0;
            LogMessageReceived($"{condition}{Environment.NewLine}{stackTrace}", code);
        }
        static void LogMessageReceived(string message, int type)
        {
            if (MaxCount <= logs.Count)
                logs.Dequeue();
            var tag = type == 1 ? 'W'
                    : type == 2 ? 'E'
                    : type == 3 ? 'X'
                    : type == 4 ? 'A'
                    : 'L';
            logs.Enqueue((type, $"[{tag} {DateTime.Now.ToString(TimeFormat)}] {message}"));

            switch (type)
            {
            case 2:
            case 3:
            case 4:
                if (btnLog != null
                 && btnLog.image != null)
                    btnLog.image.color = Color.red;
                break;
            }

            ViewUpdate();
        }

        static void ViewUpdate()
        {
            if (scroll == null
             || !scroll.gameObject.activeInHierarchy)
                return;

            int index = 0;
            foreach (var log in logs)
            {
                if (MaxCount <= index)
                    break;

                var text = texts[index];
                if (text == null)
                    continue;

                text.color = log.Item1 == 1 ? Color.yellow
                           : log.Item1 == 2 ? Color.red
                           : log.Item1 == 3 ? Color.red
                           : log.Item1 == 4 ? Color.red
                           : Color.green;
                text.text = log.Item2;
                text.gameObject.SetActive(true);

                ++index;
            }

            for (int cnt = texts.Count; index < cnt; ++index)
            {
                var text = texts[index];
                if (text == null)
                    continue;

                text.gameObject.SetActive(false);
            }
        }
    }
}