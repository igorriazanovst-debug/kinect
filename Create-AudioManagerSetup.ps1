$projectPath = "C:\Temp\2026\kinect\kinect\My project"
$editorPath = "$projectPath\Assets\Editor"

if (-not (Test-Path $editorPath)) { New-Item -ItemType Directory -Path $editorPath -Force | Out-Null }

$script = @'
using UnityEditor;
using UnityEngine;
using EduMotion.Games;

namespace EduMotion.Editor
{
    public class AudioManagerSetup
    {
        [MenuItem("EduMotion/Setup AudioManager")]
        public static void Setup()
        {
            var go = GameObject.Find("Bootstrap");
            if (go == null)
            {
                Debug.LogError("Bootstrap GameObject not found in scene!");
                return;
            }

            var am = go.GetComponent<AudioManager>();
            if (am == null)
            {
                Debug.LogError("AudioManager component not found on Bootstrap!");
                return;
            }

            // Voice AudioSource
            var voiceSources = go.GetComponents<AudioSource>();
            AudioSource voiceSource = null;
            AudioSource sfxSource   = null;

            if (voiceSources.Length == 0)
            {
                voiceSource = go.AddComponent<AudioSource>();
                sfxSource   = go.AddComponent<AudioSource>();
            }
            else if (voiceSources.Length == 1)
            {
                voiceSource = voiceSources[0];
                sfxSource   = go.AddComponent<AudioSource>();
            }
            else
            {
                voiceSource = voiceSources[0];
                sfxSource   = voiceSources[1];
            }

            voiceSource.playOnAwake = false;
            sfxSource.playOnAwake   = false;

            var so = new SerializedObject(am);
            so.FindProperty("_voiceSource").objectReferenceValue = voiceSource;
            so.FindProperty("_sfxSource").objectReferenceValue   = sfxSource;
            so.ApplyModifiedProperties();

            EditorUtility.SetDirty(go);
            Debug.Log("AudioManager setup complete: VoiceSource + SFXSource assigned.");
        }
    }
}
'@

Set-Content -Path "$editorPath\AudioManagerSetup.cs" -Value $script -Encoding UTF8
Write-Host "AudioManagerSetup.cs created: $editorPath\AudioManagerSetup.cs"
