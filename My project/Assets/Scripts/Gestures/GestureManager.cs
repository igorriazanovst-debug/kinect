using System.Collections.Generic;
using UnityEngine;
using EduMotion.Core;
using EduMotion.Kinect;

namespace EduMotion.Gestures
{
    public class GestureManager : MonoBehaviour
    {
        public static GestureManager Instance { get; private set; }

        [Header("Thresholds")]
        [SerializeField] private float _raiseThreshold = 0.10f;
        [SerializeField] private float _stepThreshold  = 0.40f;
        [SerializeField] private float _turnThreshold  = 0.25f;
        [SerializeField] private float _stopArmSpan    = 0.80f;

        [Header("Smoothing & Hold")]
        [SerializeField] private float _smoothFactor = 0.25f;
        [SerializeField] private float _holdDuration = 0.50f;
        [SerializeField] private float _cooldown      = 1.00f;

        private SkeletonData _skeleton;
        private readonly Dictionary<GestureType, float> _holdTimers = new Dictionary<GestureType, float>();
        private readonly Dictionary<GestureType, float> _coolTimers = new Dictionary<GestureType, float>();

        private Vector3 _sHead, _sHandL, _sHandR, _sShoulderL, _sShoulderR;
        private Vector3 _sHipC, _sFootL, _sFootR;
        private bool _initialized;

        private static readonly GestureType[] ActiveGestures =
        {
            GestureType.RaiseHand,
            GestureType.StepForward,
            GestureType.Turn,
            GestureType.Stop
        };

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            foreach (var g in ActiveGestures)
            {
                _holdTimers[g] = 0f;
                _coolTimers[g] = 0f;
            }
        }

        private void OnEnable()
        {
            EventBus.Subscribe<SkeletonData>(GameEvents.PlayerDetected, OnPlayerDetected);
            EventBus.Subscribe(GameEvents.PlayerLost, OnPlayerLost);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<SkeletonData>(GameEvents.PlayerDetected, OnPlayerDetected);
            EventBus.Unsubscribe(GameEvents.PlayerLost, OnPlayerLost);
        }

        private void OnPlayerDetected(SkeletonData data)
        {
            _skeleton = data;
            if (!_initialized) { InitSmoothed(); _initialized = true; }
        }

        private void OnPlayerLost()
        {
            _skeleton    = null;
            _initialized = false;
        }

        private void Update()
        {
            if (_skeleton == null) return;
            UpdateSmoothed();
            TickCooldowns();
            CheckRaiseHand();
            CheckStepForward();
            CheckTurn();
            CheckStop();
        }

        private void InitSmoothed()
        {
            _sHead      = _skeleton.GetJointPosition(JointType.Head);
            _sHandL     = _skeleton.GetJointPosition(JointType.HandLeft);
            _sHandR     = _skeleton.GetJointPosition(JointType.HandRight);
            _sShoulderL = _skeleton.GetJointPosition(JointType.ShoulderLeft);
            _sShoulderR = _skeleton.GetJointPosition(JointType.ShoulderRight);
            _sHipC      = _skeleton.GetJointPosition(JointType.HipCenter);
            _sFootL     = _skeleton.GetJointPosition(JointType.FootLeft);
            _sFootR     = _skeleton.GetJointPosition(JointType.FootRight);
        }

        private void UpdateSmoothed()
        {
            float a = _smoothFactor;
            _sHead      = Vector3.Lerp(_sHead,      _skeleton.GetJointPosition(JointType.Head),          a);
            _sHandL     = Vector3.Lerp(_sHandL,     _skeleton.GetJointPosition(JointType.HandLeft),      a);
            _sHandR     = Vector3.Lerp(_sHandR,     _skeleton.GetJointPosition(JointType.HandRight),     a);
            _sShoulderL = Vector3.Lerp(_sShoulderL, _skeleton.GetJointPosition(JointType.ShoulderLeft),  a);
            _sShoulderR = Vector3.Lerp(_sShoulderR, _skeleton.GetJointPosition(JointType.ShoulderRight), a);
            _sHipC      = Vector3.Lerp(_sHipC,      _skeleton.GetJointPosition(JointType.HipCenter),     a);
            _sFootL     = Vector3.Lerp(_sFootL,     _skeleton.GetJointPosition(JointType.FootLeft),      a);
            _sFootR     = Vector3.Lerp(_sFootR,     _skeleton.GetJointPosition(JointType.FootRight),     a);
        }

        private void TickCooldowns()
        {
            foreach (var g in ActiveGestures)
            {
                if (_coolTimers[g] > 0f)
                    _coolTimers[g] -= Time.deltaTime;
            }
        }

        private bool IsOnCooldown(GestureType g) => _coolTimers[g] > 0f;

        private void HandleGestureHold(GestureType g, bool isActive, HandSide hand = HandSide.Both)
        {
            if (IsOnCooldown(g)) { _holdTimers[g] = 0f; return; }

            if (isActive)
            {
                _holdTimers[g] += Time.deltaTime;
                if (_holdTimers[g] >= _holdDuration)
                {
                    Fire(g, hand);
                    _holdTimers[g] = 0f;
                    _coolTimers[g] = _cooldown;
                }
            }
            else
            {
                _holdTimers[g] = Mathf.Max(0f, _holdTimers[g] - Time.deltaTime * 2f);
            }
        }

        private void Fire(GestureType type, HandSide hand)
        {
            var evt = new GestureEvent
            {
                Type       = type,
                Hand       = hand,
                Confidence = 1f,
                Timestamp  = Time.time
            };
            EventBus.Publish(GameEvents.GestureDetected, evt);
            Debug.Log($"[GestureManager] Gesture: {type} ({hand})");
        }

        private void CheckRaiseHand()
        {
            bool leftUp  = _sHandL.y > _sHead.y + _raiseThreshold;
            bool rightUp = _sHandR.y > _sHead.y + _raiseThreshold;
            if (leftUp || rightUp)
            {
                var side = (leftUp && rightUp) ? HandSide.Both : leftUp ? HandSide.Left : HandSide.Right;
                HandleGestureHold(GestureType.RaiseHand, true, side);
            }
            else HandleGestureHold(GestureType.RaiseHand, false);
        }

        private void CheckStepForward()
        {
            float avgFoot = (_sFootL.z + _sFootR.z) * 0.5f;
            HandleGestureHold(GestureType.StepForward, (_sHipC.z - avgFoot) > _stepThreshold);
        }

        private void CheckTurn()
        {
            HandleGestureHold(GestureType.Turn, Mathf.Abs(_sShoulderR.z - _sShoulderL.z) > _turnThreshold);
        }

        private void CheckStop()
        {
            float span    = Mathf.Abs(_sHandR.x - _sHandL.x);
            float yDeltaL = Mathf.Abs(_sHandL.y - _sShoulderL.y);
            float yDeltaR = Mathf.Abs(_sHandR.y - _sShoulderR.y);
            HandleGestureHold(GestureType.Stop, span > _stopArmSpan && yDeltaL < 0.15f && yDeltaR < 0.15f, HandSide.Both);
        }
    }
}




