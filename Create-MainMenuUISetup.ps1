$projectPath = "C:\Temp\2026\kinect\kinect\My project"
$editorPath = "$projectPath\Assets\Editor"

if (-not (Test-Path $editorPath)) { New-Item -ItemType Directory -Path $editorPath -Force | Out-Null }

$script = @'
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using EduMotion.UI;

namespace EduMotion.Editor
{
    public class MainMenuUISetup
    {
        [MenuItem("EduMotion/Setup MainMenu UI")]
        public static void Setup()
        {
            // Открываем сцену MainMenu
            var scenePath = "Assets/Scenes/MainMenu.unity";
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // Canvas
            var canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGO.AddComponent<GraphicRaycaster>();

            // Background
            var bg = CreateImage(canvasGO, "Background", new Color(0.1f, 0.15f, 0.25f));
            SetRect(bg, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            // Title
            var title = CreateText(canvasGO, "Title", "EduMotion Kids", 48, FontStyle.Bold);
            SetRect(title, new Vector2(0.1f, 0.75f), new Vector2(0.9f, 0.95f), Vector2.zero, Vector2.zero);

            // Kinect status panel
            var statusPanel = new GameObject("KinectStatusPanel");
            statusPanel.transform.SetParent(canvasGO.transform, false);
            var statusImg = statusPanel.AddComponent<Image>();
            statusImg.color = new Color(0, 0, 0, 0.5f);
            SetRect(statusPanel, new Vector2(0.3f, 0.65f), new Vector2(0.7f, 0.75f), Vector2.zero, Vector2.zero);

            var statusIcon = CreateImage(statusPanel, "KinectIcon", Color.red);
            SetRect(statusIcon, new Vector2(0.05f, 0.1f), new Vector2(0.15f, 0.9f), Vector2.zero, Vector2.zero);

            var statusText = CreateText(statusPanel, "KinectStatusText", "Kinect: не найден", 20, FontStyle.Normal);
            SetRect(statusText, new Vector2(0.18f, 0.05f), new Vector2(0.95f, 0.95f), Vector2.zero, Vector2.zero);

            // Buttons
            var trafficBtn  = CreateButton(canvasGO, "TrafficButton",  "🚦 Светофор",    new Vector2(0.1f, 0.42f), new Vector2(0.9f, 0.58f));
            var crossingBtn = CreateButton(canvasGO, "CrossingButton", "🚸 Переход",     new Vector2(0.1f, 0.24f), new Vector2(0.9f, 0.40f));
            var ecologyBtn  = CreateButton(canvasGO, "EcologyButton",  "♻️ Экология",   new Vector2(0.1f, 0.06f), new Vector2(0.9f, 0.22f));

            // MainMenuUI component
            var menuGO = new GameObject("MainMenuUI");
            menuGO.transform.SetParent(canvasGO.transform, false);
            var menuUI = menuGO.AddComponent<MainMenuUI>();

            var so = new SerializedObject(menuUI);
            so.FindProperty("_trafficButton").objectReferenceValue  = trafficBtn.GetComponent<Button>();
            so.FindProperty("_crossingButton").objectReferenceValue = crossingBtn.GetComponent<Button>();
            so.FindProperty("_ecologyButton").objectReferenceValue  = ecologyBtn.GetComponent<Button>();
            so.FindProperty("_kinectStatusText").objectReferenceValue = statusText.GetComponent<Text>();
            so.FindProperty("_kinectStatusIcon").objectReferenceValue = statusIcon.GetComponent<Image>();
            so.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(scene);
            Debug.Log("MainMenu UI setup complete.");
        }

        private static GameObject CreateImage(GameObject parent, string name, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            return go;
        }

        private static GameObject CreateText(GameObject parent, string name, string text, int size, FontStyle style)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var t = go.AddComponent<Text>();
            t.text      = text;
            t.fontSize  = size;
            t.fontStyle = style;
            t.color     = Color.white;
            t.alignment = TextAnchor.MiddleCenter;
            t.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return go;
        }

        private static GameObject CreateButton(GameObject parent, string name, string label, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var img = go.AddComponent<Image>();
            img.color = new Color(0.2f, 0.5f, 0.8f);
            var btn = go.AddComponent<Button>();
            var colors = btn.colors;
            colors.highlightedColor = new Color(0.3f, 0.6f, 0.9f);
            colors.pressedColor     = new Color(0.1f, 0.3f, 0.6f);
            colors.disabledColor    = new Color(0.3f, 0.3f, 0.3f);
            btn.colors = colors;
            SetRect(go, anchorMin, anchorMax, Vector2.zero, Vector2.zero);

            var textGO = CreateText(go, "Label", label, 28, FontStyle.Bold);
            SetRect(textGO, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return go;
        }

        private static void SetRect(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var rt = go.GetComponent<RectTransform>();
            if (rt == null) rt = go.AddComponent<RectTransform>();
            rt.anchorMin  = anchorMin;
            rt.anchorMax  = anchorMax;
            rt.offsetMin  = offsetMin;
            rt.offsetMax  = offsetMax;
        }
    }
}
'@

Set-Content -Path "$editorPath\MainMenuUISetup.cs" -Value $script -Encoding UTF8
Write-Host "MainMenuUISetup.cs created: $editorPath\MainMenuUISetup.cs"
