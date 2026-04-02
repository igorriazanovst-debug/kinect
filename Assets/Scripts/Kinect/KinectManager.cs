using System;
using System.Runtime.InteropServices;
using UnityEngine;
using EduMotion.Core;

namespace EduMotion.Kinect
{
    /// <summary>
    /// Обёртка над Kinect SDK 1.8 (Windows).
    /// На Linux подставляется заглушка MockKinectProvider.
    /// </summary>
    public class KinectManager : MonoBehaviour
    {
        public static KinectManager Instance { get; private set; }

        [Header("Settings")]
        [Tooltip("Индекс сенсора (0 — первый подключённый)")]
        [SerializeField] private int _sensorIndex = 0;

        [Tooltip("Угол наклона сенсора (-27..27)")]
        [SerializeField, Range(-27, 27)] private int _elevationAngle = 0;

        private IKinectProvider _provider;

        public SkeletonData PrimaryPlayer { get; private set; }
        public bool         IsConnected   { get; private set; }

        // ----------------------------------------------------------------

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            _provider = new KinectV1Provider(_sensorIndex, _elevationAngle);
#else
            _provider = new MockKinectProvider();
            Debug.LogWarning("[KinectManager] Платформа не Windows — используется MockKinectProvider.");
#endif
            bool ok = _provider.Initialize();
            IsConnected = ok;

            EventBus.Publish(ok ? GameEvents.KinectConnected : GameEvents.KinectDisconnected);
        }

        private void Update()
        {
            if (!IsConnected) return;

            PrimaryPlayer = _provider.GetPrimarySkeleton();

            if (PrimaryPlayer != null && PrimaryPlayer.IsTracked)
                EventBus.Publish(GameEvents.PlayerDetected, PrimaryPlayer);
        }

        private void OnDestroy()
        {
            _provider?.Shutdown();
        }
    }
}
