using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using EduMotion.UI;
using EduMotion.Games;

namespace EduMotion.Editor
{
    public class GameScenesSetup
    {
        [MenuItem("EduMotion/Setup Game Scenes")]
        public static void Setup()
        {
            SetupTraffic();
            SetupCrossing();
            SetupEcology();
            Debug.Log("All game scenes setup complete.");
        }

        // ----------------------------------------------------------------
        private static void SetupTraffic()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Game_Traffic.unity", OpenSceneMode.Single);

            var canvasGO = CreateCanvas();

            // Score
            var scoreText = CreateText(canvasGO, "ScoreText", "РћС‡РєРё: 0", 28, new Vector2(0f, 0.9f), new Vector2(0.4f, 1f));

            // Lights panel
            var panel = CreatePanel(canvasGO, "LightsPanel", new Color(0.1f, 0.1f, 0.1f, 0.8f),
                new Vector2(0.3f, 0.45f), new Vector2(0.7f, 0.9f));

            var redImg    = CreateImage(panel, "RedLight",    Color.red,    new Vector2(0.1f, 0.66f), new Vector2(0.9f, 0.98f));
            var yellowImg = CreateImage(panel, "YellowLight", Color.yellow, new Vector2(0.1f, 0.34f), new Vector2(0.9f, 0.66f));
            var greenImg  = CreateImage(panel, "GreenLight",  Color.green,  new Vector2(0.1f, 0.02f), new Vector2(0.9f, 0.34f));

            // Instruction
            var instrText = CreateText(canvasGO, "InstructionText", "РЎРјРѕС‚СЂРё РЅР° СЃРІРµС‚РѕС„РѕСЂ!", 32, new Vector2(0.1f, 0.3f), new Vector2(0.9f, 0.44f));

            // Feedback
            var feedbackText = CreateText(canvasGO, "FeedbackText", "", 40, new Vector2(0.1f, 0.15f), new Vector2(0.9f, 0.3f));

            // Back button
            var backBtn = CreateButton(canvasGO, "BackButton", "в†ђ РњРµРЅСЋ", new Vector2(0f, 0.9f), new Vector2(0.2f, 1f));
            AddSceneLoader(backBtn, "MainMenu");

            // Game GameObject
            var gameGO = new GameObject("TrafficLightGame");
            var game   = gameGO.AddComponent<TrafficLightGame>();
            var view   = gameGO.AddComponent<TrafficLightView>();

            var soV = new SerializedObject(view);
            soV.FindProperty("_lightImage").objectReferenceValue    = redImg.GetComponent<Image>();
            soV.FindProperty("_feedbackOk").objectReferenceValue    = null;
            soV.FindProperty("_feedbackFail").objectReferenceValue  = null;
            soV.ApplyModifiedProperties();

            var soG = new SerializedObject(game);
            soG.FindProperty("_view").objectReferenceValue = view;
            soG.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(scene);
        }

        // ----------------------------------------------------------------
        private static void SetupCrossing()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Game_Crossing.unity", OpenSceneMode.Single);

            var canvasGO = CreateCanvas();

            CreateText(canvasGO, "ScoreText",       "РћС‡РєРё: 0",              28, new Vector2(0f,   0.9f), new Vector2(0.4f, 1f));
            CreateText(canvasGO, "InstructionText", "РЁР°РіРЅРё РІРїРµСЂС‘Рґ!",        32, new Vector2(0.1f, 0.4f), new Vector2(0.9f, 0.55f));
            var danger = CreateImage(canvasGO, "DangerIndicator", new Color(1f, 0.2f, 0.2f, 0.7f),
                new Vector2(0.2f, 0.55f), new Vector2(0.8f, 0.85f));
            danger.SetActive(false);

            var backBtn = CreateButton(canvasGO, "BackButton", "в†ђ РњРµРЅСЋ", new Vector2(0f, 0.9f), new Vector2(0.2f, 1f));
            AddSceneLoader(backBtn, "MainMenu");

            var gameGO = new GameObject("CrossingGame");
            var game   = gameGO.AddComponent<CrossingGame>();
            var view   = gameGO.AddComponent<CrossingView>();

            var soV = new SerializedObject(view);
            soV.FindProperty("_dangerIndicator").objectReferenceValue = danger;
            soV.ApplyModifiedProperties();

            var soG = new SerializedObject(game);
            soG.FindProperty("_view").objectReferenceValue = view;
            soG.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(scene);
        }

        // ----------------------------------------------------------------
        private static void SetupEcology()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Game_Ecology.unity", OpenSceneMode.Single);

            var canvasGO = CreateCanvas();

            CreateText(canvasGO, "ScoreText",       "РћС‡РєРё: 0",            28, new Vector2(0f,   0.9f), new Vector2(0.4f, 1f));
            CreateText(canvasGO, "InstructionText", "РЎРѕСЂС‚РёСЂСѓР№ РјСѓСЃРѕСЂ!",    32, new Vector2(0.1f, 0.1f), new Vector2(0.9f, 0.25f));

            var trashPanel = CreatePanel(canvasGO, "TrashPanel", new Color(0.15f, 0.15f, 0.15f, 0.8f),
                new Vector2(0.3f, 0.45f), new Vector2(0.7f, 0.85f));
            var trashImg = CreateImage(trashPanel, "TrashImage", Color.white, new Vector2(0.1f, 0.1f), new Vector2(0.9f, 0.9f));

            var backBtn = CreateButton(canvasGO, "BackButton", "в†ђ РњРµРЅСЋ", new Vector2(0f, 0.9f), new Vector2(0.2f, 1f));
            AddSceneLoader(backBtn, "MainMenu");

            var gameGO = new GameObject("EcologyGame");
            var game   = gameGO.AddComponent<EcologyGame>();
            var view   = gameGO.AddComponent<EcologyView>();

            var soV = new SerializedObject(view);
            soV.FindProperty("_trashImage").objectReferenceValue = trashImg.GetComponent<Image>();
            soV.ApplyModifiedProperties();

            var soG = new SerializedObject(game);
            soG.FindProperty("_view").objectReferenceValue = view;
            soG.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(scene);
        }

        // ----------------------------------------------------------------
        private static GameObject CreateCanvas()
        {
            var go = new GameObject("Canvas");
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            go.AddComponent<GraphicRaycaster>();

            var bg = new GameObject("Background");
            bg.transform.SetParent(go.transform, false);
            var img = bg.AddComponent<Image>();
            img.color = new Color(0.08f, 0.12f, 0.2f);
            SetRect(bg, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            return go;
        }

        private static GameObject CreatePanel(GameObject parent, string name, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            SetRect(go, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
            return go;
        }

        private static GameObject CreateImage(GameObject parent, string name, Color color, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            SetRect(go, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
            return go;
        }

        private static GameObject CreateText(GameObject parent, string name, string text, int size, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var t = go.AddComponent<Text>();
            t.text      = text;
            t.fontSize  = size;
            t.color     = Color.white;
            t.alignment = TextAnchor.MiddleCenter;
            t.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            SetRect(go, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
            return go;
        }

        private static GameObject CreateButton(GameObject parent, string name, string label, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            var img = go.AddComponent<Image>();
            img.color = new Color(0.2f, 0.5f, 0.8f);
            go.AddComponent<Button>();
            SetRect(go, anchorMin, anchorMax, Vector2.zero, Vector2.zero);

            var textGO = new GameObject("Label");
            textGO.transform.SetParent(go.transform, false);
            var t = textGO.AddComponent<Text>();
            t.text      = label;
            t.fontSize  = 22;
            t.color     = Color.white;
            t.alignment = TextAnchor.MiddleCenter;
            t.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            SetRect(textGO, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            return go;
        }

        private static void AddSceneLoader(GameObject btnGO, string sceneName)
        {
            var btn = btnGO.GetComponent<Button>();
            btn.onClick.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName));
        }

        private static void SetRect(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var rt = go.GetComponent<RectTransform>();
            if (rt == null) rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
        }
    }
}
