using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EduMotion.Editor
{
    public class FixGestureThresholds
    {
        [MenuItem("EduMotion/Fix Gesture Thresholds In All Scenes")]
        public static void Run()
        {
            string[] scenes = { "MainMenu", "Game_Traffic", "Game_Crossing", "Game_Ecology" };
            foreach (var s in scenes) Fix(s);
            Debug.Log("[FixGestureThresholds] Done.");
        }

        private static void Fix(string sceneName)
        {
            var scene = EditorSceneManager.OpenScene(
                "Assets/Scenes/" + sceneName + ".unity", OpenSceneMode.Single);

            var gm = GameObject.FindObjectOfType<EduMotion.Gestures.GestureManager>();
            if (gm == null) { Debug.Log("No GestureManager in " + sceneName); return; }

            var so = new SerializedObject(gm);
            so.FindProperty("_stepThreshold").floatValue  = 0.55f;
            so.FindProperty("_turnThreshold").floatValue  = 0.25f;
            so.FindProperty("_raiseThreshold").floatValue = 0.10f;
            so.FindProperty("_stopArmSpan").floatValue    = 0.80f;
            so.FindProperty("_holdDuration").floatValue   = 0.50f;
            so.FindProperty("_cooldown").floatValue       = 1.00f;
            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(gm);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[FixGestureThresholds] Fixed: " + sceneName);
        }
    }
}
