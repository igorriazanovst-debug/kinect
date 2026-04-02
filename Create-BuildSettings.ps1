$projectPath = "C:\Temp\2026\kinect\kinect\My project"
$editorPath = "$projectPath\Assets\Editor"

if (-not (Test-Path $editorPath)) { New-Item -ItemType Directory -Path $editorPath -Force | Out-Null }

$buildSettingsScript = @'
using UnityEditor;
using UnityEngine;

namespace EduMotion.Editor
{
    public class BuildSettingsSetup
    {
        [MenuItem("EduMotion/Setup Build Settings")]
        public static void SetupBuildSettings()
        {
            var scenes = new[]
            {
                "Assets/Scenes/MainMenu.unity",
                "Assets/Scenes/Game_Traffic.unity",
                "Assets/Scenes/Game_Crossing.unity",
                "Assets/Scenes/Game_Ecology.unity",
            };

            var editorScenes = new EditorBuildSettingsScene[scenes.Length];
            for (int i = 0; i < scenes.Length; i++)
            {
                editorScenes[i] = new EditorBuildSettingsScene(scenes[i], true);
            }

            EditorBuildSettings.scenes = editorScenes;
            Debug.Log("Build Settings updated: " + scenes.Length + " scenes added.");
        }
    }
}
'@

Set-Content -Path "$editorPath\BuildSettingsSetup.cs" -Value $buildSettingsScript -Encoding UTF8
Write-Host "BuildSettingsSetup.cs created: $editorPath\BuildSettingsSetup.cs"
