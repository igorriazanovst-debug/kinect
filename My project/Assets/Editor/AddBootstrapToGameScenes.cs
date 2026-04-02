using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using EduMotion.Kinect;
using EduMotion.Gestures;
using EduMotion.Games;
using EduMotion.Core;

namespace EduMotion.Editor
{
    public class AddBootstrapToGameScenes
    {
        [MenuItem("EduMotion/Add Bootstrap To Game Scenes")]
        public static void Run()
        {
            string[] scenes = { "Game_Traffic", "Game_Crossing", "Game_Ecology" };
            foreach (var s in scenes)
                AddBootstrap(s);
            Debug.Log("[Bootstrap] Done.");
        }

        private static void AddBootstrap(string sceneName)
        {
            var scene = EditorSceneManager.OpenScene(
                "Assets/Scenes/" + sceneName + ".unity", OpenSceneMode.Single);

            var existing = GameObject.Find("Bootstrap");
            if (existing != null)
            {
                Debug.Log("[Bootstrap] Already exists in " + sceneName);
                EditorSceneManager.SaveScene(scene);
                return;
            }

            var go = new GameObject("Bootstrap");

            go.AddComponent<EduMotion.Kinect.KinectManager>();
            go.AddComponent<EduMotion.Gestures.GestureManager>();
            go.AddComponent<EduMotion.Games.GameManager>();
            go.AddComponent<EduMotion.Games.RewardSystem>();
            go.AddComponent<EduMotion.Games.AudioManager>();

            var audioSources = new UnityEngine.AudioSource[2];
            audioSources[0] = go.AddComponent<UnityEngine.AudioSource>();
            audioSources[1] = go.AddComponent<UnityEngine.AudioSource>();
            audioSources[0].playOnAwake = false;
            audioSources[1].playOnAwake = false;

            var am = go.GetComponent<EduMotion.Games.AudioManager>();
            var soAm = new SerializedObject(am);
            soAm.FindProperty("_voiceSource").objectReferenceValue = audioSources[0];
            soAm.FindProperty("_sfxSource").objectReferenceValue   = audioSources[1];
            soAm.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(scene);
            Debug.Log("[Bootstrap] Added to " + sceneName);
        }
    }
}
