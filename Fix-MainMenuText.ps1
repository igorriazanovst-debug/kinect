$projectPath = "C:\Temp\2026\kinect\kinect\My project"
$editorPath  = "$projectPath\Assets\Editor"

$script = @'
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace EduMotion.Editor
{
    public class FixMainMenuText
    {
        [MenuItem("EduMotion/Fix MainMenu Text")]
        public static void Fix()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity", OpenSceneMode.Single);

            SetText("TrafficButton/Label",  "\u0421\u0432\u0435\u0442\u043e\u0444\u043e\u0440");
            SetText("CrossingButton/Label", "\u041f\u0435\u0440\u0435\u0445\u043e\u0434");
            SetText("EcologyButton/Label",  "\u042d\u043a\u043e\u043b\u043e\u0433\u0438\u044f");
            SetText("Title",                "EduMotion Kids");
            SetText("KinectStatusPanel/KinectStatusText", "Kinect: \u043d\u0435 \u043d\u0430\u0439\u0434\u0435\u043d");

            EditorSceneManager.SaveScene(scene);
            Debug.Log("MainMenu text fixed.");
        }

        private static void SetText(string path, string value)
        {
            var go = GameObject.Find("Canvas/" + path);
            if (go == null) { Debug.LogWarning("Not found: " + path); return; }
            var t = go.GetComponent<Text>();
            if (t == null) { Debug.LogWarning("No Text on: " + path); return; }
            t.text = value;
            EditorUtility.SetDirty(go);
        }
    }
}
'@

Set-Content -Path "$editorPath\FixMainMenuText.cs" -Value $script -Encoding UTF8
Write-Host "FixMainMenuText.cs created."
