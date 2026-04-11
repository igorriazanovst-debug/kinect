using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using EduMotion.UI;
using EduMotion.Games;

namespace EduMotion.Editor
{
    public class SetupTrafficScene
    {
        [MenuItem("EduMotion/Setup Traffic Scene")]
        public static void Run()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Game_Traffic.unity", OpenSceneMode.Single);

            // Canvas
            var canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                var cgo = new GameObject("Canvas");
                var c = cgo.AddComponent<Canvas>();
                c.renderMode = RenderMode.ScreenSpaceOverlay;
                cgo.AddComponent<CanvasScaler>();
                cgo.AddComponent<GraphicRaycaster>();
                canvas = cgo;
            }

            // Score text
            var scoreGO = GetOrCreate(canvas, "ScoreText");
            var scoreText = GetOrAdd<Text>(scoreGO);
            scoreText.text = "Score: 0";
            scoreText.fontSize = 28;
            scoreText.color = Color.white;
            scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            SetAnchors(scoreGO, new Vector2(0f,0.9f), new Vector2(0.5f,1f));

            // Light image
            var lightGO = GetOrCreate(canvas, "LightImage");
            var lightImg = GetOrAdd<Image>(lightGO);
            lightImg.color = Color.red;
            SetAnchors(lightGO, new Vector2(0.35f,0.45f), new Vector2(0.65f,0.85f));

            // Instruction text
            var instrGO = GetOrCreate(canvas, "InstructionText");
            var instrText = GetOrAdd<Text>(instrGO);
            instrText.text = "Watch the light!";
            instrText.fontSize = 32;
            instrText.color = Color.white;
            instrText.alignment = TextAnchor.MiddleCenter;
            instrText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            SetAnchors(instrGO, new Vector2(0.05f,0.28f), new Vector2(0.95f,0.43f));

            // Feedback text
            var fbGO = GetOrCreate(canvas, "FeedbackText");
            var fbText = GetOrAdd<Text>(fbGO);
            fbText.text = "";
            fbText.fontSize = 48;
            fbText.fontStyle = FontStyle.Bold;
            fbText.color = Color.green;
            fbText.alignment = TextAnchor.MiddleCenter;
            fbText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            SetAnchors(fbGO, new Vector2(0.05f,0.1f), new Vector2(0.95f,0.27f));

            // Back button
            var backGO = GetOrCreate(canvas, "BackButton");
            GetOrAdd<Image>(backGO).color = new Color(0.3f,0.3f,0.3f);
            GetOrAdd<Button>(backGO);
            SetAnchors(backGO, new Vector2(0f,0.9f), new Vector2(0.2f,1f));
            var backLabelGO = GetOrCreate(backGO, "Label");
            var backLabel = GetOrAdd<Text>(backLabelGO);
            backLabel.text = "<- Menu";
            backLabel.fontSize = 20;
            backLabel.color = Color.white;
            backLabel.alignment = TextAnchor.MiddleCenter;
            backLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            SetAnchors(backLabelGO, Vector2.zero, Vector2.one);

            // TrafficLightGame + TrafficLightView
            var gameGO = GetOrCreate(null, "TrafficLightGame");
            var game = GetOrAdd<TrafficLightGame>(gameGO);
            var view = GetOrAdd<TrafficLightView>(gameGO);

            var soV = new SerializedObject(view);
            soV.FindProperty("_lightImage").objectReferenceValue       = lightImg;
            soV.FindProperty("_instructionText").objectReferenceValue  = instrText;
            soV.FindProperty("_feedbackText").objectReferenceValue     = fbText;
            soV.ApplyModifiedProperties();

            var soG = new SerializedObject(game);
            soG.FindProperty("_view").objectReferenceValue = view;
            soG.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(scene);
            Debug.Log("[SetupTrafficScene] Done.");
        }

        private static GameObject GetOrCreate(GameObject parent, string name)
        {
            var existing = GameObject.Find(name);
            if (existing != null) return existing;
            var go = new GameObject(name);
            if (parent != null) go.transform.SetParent(parent.transform, false);
            go.AddComponent<RectTransform>();
            return go;
        }

        private static T GetOrAdd<T>(GameObject go) where T : Component
        {
            var c = go.GetComponent<T>();
            return c != null ? c : go.AddComponent<T>();
        }

        private static void SetAnchors(GameObject go, Vector2 min, Vector2 max)
        {
            var rt = go.GetComponent<RectTransform>();
            if (rt == null) rt = go.AddComponent<RectTransform>();
            rt.anchorMin = min; rt.anchorMax = max;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        }
    }
}
