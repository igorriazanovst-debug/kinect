using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace EduMotion.Editor
{
    public class FixGameScenes
    {
        [MenuItem("EduMotion/Fix Game Scenes (Camera + Text)")]
        public static void Run()
        {
            string[] scenes = { "Game_Traffic", "Game_Crossing", "Game_Ecology" };
            foreach (var s in scenes)
                Fix(s);
            Debug.Log("[FixGameScenes] Done.");
        }

        private static void Fix(string sceneName)
        {
            var scene = EditorSceneManager.OpenScene(
                "Assets/Scenes/" + sceneName + ".unity", OpenSceneMode.Single);

            // --- Camera ---
            var cam = GameObject.FindObjectOfType<Camera>();
            if (cam == null)
            {
                var camGO = new GameObject("Main Camera");
                camGO.tag = "MainCamera";
                cam = camGO.AddComponent<Camera>();
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0.1f, 0.15f, 0.25f);
                cam.orthographic = false;
                camGO.AddComponent<AudioListener>();
                Debug.Log("[FixGameScenes] Camera added to " + sceneName);
            }

            // --- Fix Text encoding ---
            var texts = GameObject.FindObjectsOfType<Text>();
            foreach (var t in texts)
            {
                if (IsGarbled(t.text))
                    t.text = FixText(t.text, sceneName);
                EditorUtility.SetDirty(t);
            }

            EditorSceneManager.SaveScene(scene);
            Debug.Log("[FixGameScenes] Fixed: " + sceneName);
        }

        private static bool IsGarbled(string s)
        {
            foreach (var c in s)
                if (c > 0x00C0 && c < 0x0400) return true;
            return false;
        }

        private static string FixText(string original, string scene)
        {
            var lower = original.ToLower();
            if (lower.Contains("traffic") || scene == "Game_Traffic")
            {
                if (original.Contains("0"))   return "Score: 0";
                if (lower.Contains("back") || lower.Contains("menu")) return "<- Menu";
                return "Watch the traffic light!";
            }
            if (lower.Contains("crossing") || scene == "Game_Crossing")
            {
                if (original.Contains("0"))   return "Score: 0";
                if (lower.Contains("back") || lower.Contains("menu")) return "<- Menu";
                return "Step forward!";
            }
            if (lower.Contains("ecology") || scene == "Game_Ecology")
            {
                if (original.Contains("0"))   return "Score: 0";
                if (lower.Contains("back") || lower.Contains("menu")) return "<- Menu";
                return "Sort the trash!";
            }
            return original;
        }
    }
}
