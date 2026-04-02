$projectPath = "C:\Temp\2026\kinect\kinect\My project"
$scriptsUI = "$projectPath\Assets\Scripts\UI"
$scenesPath = "$projectPath\Assets\Scenes"

if (-not (Test-Path $scriptsUI)) { New-Item -ItemType Directory -Path $scriptsUI -Force | Out-Null }
if (-not (Test-Path $scenesPath)) { New-Item -ItemType Directory -Path $scenesPath -Force | Out-Null }

# ---- TrafficLightView.cs ----
$trafficView = @'
using UnityEngine;
using UnityEngine.UI;

namespace EduMotion
{
    public class TrafficLightView : MonoBehaviour
    {
        [SerializeField] private Image _redLight;
        [SerializeField] private Image _yellowLight;
        [SerializeField] private Image _greenLight;
        [SerializeField] private Text _instructionText;
        [SerializeField] private Text _scoreText;

        private Color _activeColor = Color.white;
        private Color _inactiveColor = new Color(1f, 1f, 1f, 0.2f);

        public void SetLight(TrafficLightColor color)
        {
            _redLight.color    = color == TrafficLightColor.Red    ? _activeColor : _inactiveColor;
            _yellowLight.color = color == TrafficLightColor.Yellow ? _activeColor : _inactiveColor;
            _greenLight.color  = color == TrafficLightColor.Green  ? _activeColor : _inactiveColor;
        }

        public void SetInstruction(string text) => _instructionText.text = text;
        public void SetScore(int score) => _scoreText.text = "Очки: " + score;
    }
}
'@

# ---- CrossingView.cs ----
$crossingView = @'
using UnityEngine;
using UnityEngine.UI;

namespace EduMotion
{
    public class CrossingView : MonoBehaviour
    {
        [SerializeField] private Text _instructionText;
        [SerializeField] private Text _scoreText;
        [SerializeField] private Slider _progressBar;

        public void SetInstruction(string text) => _instructionText.text = text;
        public void SetScore(int score) => _scoreText.text = "Очки: " + score;
        public void SetProgress(float value) => _progressBar.value = value;
    }
}
'@

# ---- EcologyView.cs ----
$ecologyView = @'
using UnityEngine;
using UnityEngine.UI;

namespace EduMotion
{
    public class EcologyView : MonoBehaviour
    {
        [SerializeField] private Text _instructionText;
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _categoryText;

        public void SetInstruction(string text) => _instructionText.text = text;
        public void SetScore(int score) => _scoreText.text = "Очки: " + score;
        public void SetCategory(string category) => _categoryText.text = category;
    }
}
'@

Set-Content -Path "$scriptsUI\TrafficLightView.cs" -Value $trafficView  -Encoding UTF8
Set-Content -Path "$scriptsUI\CrossingView.cs"     -Value $crossingView -Encoding UTF8
Set-Content -Path "$scriptsUI\EcologyView.cs"      -Value $ecologyView  -Encoding UTF8

Write-Host "View scripts created."

# ---- Минимальные .unity сцены (YAML) ----
$sceneTemplate = @'
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!29 &1
OcclusionCullingSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_OcclusionBakeSettings:
    smallestOccluder: 5
    smallestHole: 0.25
    backfaceThreshold: 100
  m_SceneGUID: 00000000000000000000000000000000
  m_OcclusionCullingData: {fileID: 0}
--- !u!104 &2
RenderSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 9
  m_Fog: 0
  m_FogColor: {r: 0.5, g: 0.5, b: 0.5, a: 1}
  m_FogMode: 3
  m_FogDensity: 0.01
  m_LinearFogStart: 0
  m_LinearFogEnd: 300
  m_AmbientSkyColor: {r: 0.212, g: 0.227, b: 0.259, a: 1}
  m_AmbientEquatorColor: {r: 0.114, g: 0.125, b: 0.133, a: 1}
  m_AmbientGroundColor: {r: 0.047, g: 0.043, b: 0.035, a: 1}
  m_AmbientIntensity: 1
  m_AmbientMode: 0
  m_SubtractiveShadowColor: {r: 0.42, g: 0.478, b: 0.627, a: 1}
  m_SkyboxMaterial: {fileID: 0}
  m_HaloStrength: 0.5
  m_FlareStrength: 1
  m_FlareFadeSpeed: 3
  m_HaloTexture: {fileID: 0}
  m_SpotCookie: {fileID: 0}
  m_DefaultReflectionMode: 0
  m_DefaultReflectionResolution: 128
  m_ReflectionBounces: 1
  m_ReflectionIntensity: 1
  m_CustomReflection: {fileID: 0}
  m_Sun: {fileID: 0}
  m_IndirectSpecularColor: {r: 0, g: 0, b: 0, a: 1}
  m_UseRadianceAmbientProbe: 0
--- !u!157 &3
LightmapSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 12
  m_GIWorkflowMode: 1
  m_GISettings:
    serializedVersion: 2
    m_BounceScale: 1
    m_IndirectOutputScale: 1
    m_AlbedoBoost: 1
    m_EnvironmentLightingMode: 0
    m_EnableBakedLightmaps: 1
    m_EnableRealtimeLightmaps: 1
  m_LightmapEditorSettings:
    serializedVersion: 12
    m_Resolution: 2
    m_BakeResolution: 40
    m_AtlasSize: 1024
    m_AO: 0
    m_AOMaxDistance: 1
    m_CompAOExponent: 1
    m_CompAOExponentDirect: 0
    m_ExtractAmbientOcclusion: 0
    m_Padding: 2
    m_LightmapParameters: {fileID: 0}
    m_LightmapsBakeMode: 1
    m_TextureCompression: 1
    m_FinalGather: 0
    m_FinalGatherFiltering: 1
    m_FinalGatherRayCount: 256
    m_ReflectionCompression: 2
    m_MixedBakeMode: 2
    m_BakeBackend: 1
    m_PVRSampling: 1
    m_PVRDirectSampleCount: 32
    m_PVRSampleCount: 512
    m_PVRBounces: 2
    m_PVREnvironmentSampleCount: 256
    m_PVREnvironmentReferencePointCount: 2048
    m_PVRFilteringMode: 1
    m_PVRDenoiserTypeDirect: 1
    m_PVRDenoiserTypeIndirect: 1
    m_PVRDenoiserTypeAO: 1
    m_PVRFilterTypeDirect: 0
    m_PVRFilterTypeIndirect: 0
    m_PVRFilterTypeAO: 0
    m_PVREnvironmentMIS: 1
    m_PVRCulling: 1
    m_PVRFilteringGaussRadiusDirect: 1
    m_PVRFilteringGaussRadiusIndirect: 5
    m_PVRFilteringGaussRadiusAO: 2
    m_PVRFilteringAtrousPositionSigmaDirect: 0.5
    m_PVRFilteringAtrousPositionSigmaIndirect: 2
    m_PVRFilteringAtrousPositionSigmaAO: 1
    m_ExportTrainingData: 0
    m_TrainingDataDestination: TrainingData
    m_LightProbeSampleCountMultiplier: 4
  m_LightingDataAsset: {fileID: 0}
  m_LightingSettings: {fileID: 0}
--- !u!196 &4
NavMeshSettings:
  serializedVersion: 2
  m_ObjectHideFlags: 0
  m_BuildSettings:
    serializedVersion: 3
    agentTypeID: 0
    agentRadius: 0.5
    agentHeight: 2
    agentSlope: 45
    agentClimb: 0.4
    ledgeDropHeight: 0
    maxJumpAcrossDistance: 0
    minRegionArea: 2
    manualCellSize: 0
    cellSize: 0.16666667
    manualTileSize: 0
    tileSize: 256
    accuratePlacement: 0
    maxJobWorkers: 0
    preserveTilesOutsideBounds: 0
    debug:
      m_Flags: 0
  m_NavMeshData: {fileID: 0}
'@

foreach ($sceneName in @("Game_Traffic", "Game_Crossing", "Game_Ecology")) {
    $scenePath = "$scenesPath\$sceneName.unity"
    if (-not (Test-Path $scenePath)) {
        Set-Content -Path $scenePath -Value $sceneTemplate -Encoding UTF8
        Write-Host "Scene created: $scenePath"
    } else {
        Write-Host "Scene already exists: $scenePath"
    }
}

Write-Host "Done."
