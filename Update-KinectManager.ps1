$kinectPath = "C:\Temp\2026\kinect\kinect\My project\Assets\Scripts\Kinect"

$script = @'
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using EduMotion.Core;

namespace EduMotion.Kinect
{
    public enum KinectProviderType { Auto, KinectV1, UDP, Mock }

    public class KinectManager : MonoBehaviour
    {
        public static KinectManager Instance { get; private set; }

        [Header("Provider")]
        [SerializeField] private KinectProviderType _providerType = KinectProviderType.Auto;
        [SerializeField] private int _udpPort = 7777;

        [Header("Kinect V1 Settings")]
        [Tooltip("Индекс сенсора (0 — первый подключённый)")]
        [SerializeField] private int _sensorIndex    = 0;
        [SerializeField, Range(-27, 27)] private int _elevationAngle = 0;

        private IKinectProvider _provider;
        public SkeletonData PrimaryPlayer { get; private set; }
        public bool         IsConnected   { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _provider = CreateProvider();
            bool ok   = _provider.Initialize();
            IsConnected = ok;
            EventBus.Publish(ok ? GameEvents.KinectConnected : GameEvents.KinectDisconnected);
        }

        private IKinectProvider CreateProvider()
        {
            switch (_providerType)
            {
                case KinectProviderType.KinectV1:
                    return new KinectV1Provider(_sensorIndex, _elevationAngle);

                case KinectProviderType.UDP:
                    return new UDPKinectProvider(_udpPort);

                case KinectProviderType.Mock:
                    return new MockKinectProvider();

                case KinectProviderType.Auto:
                default:
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                    Debug.Log("[KinectManager] Auto: trying KinectV1...");
                    var v1 = new KinectV1Provider(_sensorIndex, _elevationAngle);
                    if (v1.Initialize())
                    {
                        IsConnected = true;
                        EventBus.Publish(GameEvents.KinectConnected);
                        return v1;
                    }
                    Debug.Log("[KinectManager] KinectV1 failed, switching to UDP...");
                    return new UDPKinectProvider(_udpPort);
#else
                    Debug.Log("[KinectManager] Auto: non-Windows, using UDP...");
                    return new UDPKinectProvider(_udpPort);
#endif
            }
        }

        private void Update()
        {
            if (!IsConnected)
            {
                // Для UDP — проверяем появление данных
                if (_provider is UDPKinectProvider)
                {
                    var data = _provider.GetPrimarySkeleton();
                    if (data != null && data.IsTracked)
                    {
                        IsConnected = true;
                        EventBus.Publish(GameEvents.KinectConnected);
                    }
                }
                return;
            }

            PrimaryPlayer = _provider.GetPrimarySkeleton();
            if (PrimaryPlayer != null && PrimaryPlayer.IsTracked)
                EventBus.Publish(GameEvents.PlayerDetected, PrimaryPlayer);
            else
                EventBus.Publish(GameEvents.PlayerLost);
        }

        private void OnDestroy()
        {
            _provider?.Shutdown();
        }
    }
}
'@

Set-Content -Path "$kinectPath\KinectManager.cs" -Value $script -Encoding UTF8
Write-Host "KinectManager.cs updated."
