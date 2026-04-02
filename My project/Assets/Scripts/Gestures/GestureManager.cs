using System.Collections.Generic;
using UnityEngine;
using EduMotion.Core;
using EduMotion.Kinect;

namespace EduMotion.Gestures
{
    /// <summary>
    /// Распознаёт 4 жеста MVP:
    ///   - RaiseHand   (поднятие руки выше головы)
    ///   - StepForward (шаг вперёд — Z-дельта бёдер)
    ///   - Turn        (поворот — X-дельта плеч)
    ///   - Stop        (обе руки в стороны горизонтально)
    /// Применяет сглаживание и таймер удержания.
    /// </summary>
    public class GestureManager : MonoBehaviour
    {
        public static GestureManager Instance { get; private set; }

        [Header("Thresholds")]
        [SerializeField] private float _raiseThreshold   = 0.10f;   // рука выше головы на X м
        [SerializeField] private float _stepThreshold    = 0.15f;   // шаг вперёд, м
        [SerializeField] private float _turnThreshold    = 0.20f;   // поворот плеч, м
        [SerializeField] private float _stopArmSpan      = 0.35f;   // ширина рук для Стоп

        [Header("Smoothing & Hold")]
        [SerializeField] private float _smoothFactor     = 0.25f;   // EMA alpha
        [SerializeField] private float _holdDuration     = 0.4f;    // сек удержания жеста
        [SerializeField] private float _cooldown         = 0.8f;    // сек после срабатывания

        // ----------------------------------------------------------------

        private SkeletonData _skeleton;
        private Dictionary<GestureType, float> _holdTimers  = new();
        private Dictionary<GestureType, float> _coolTimers  = new();

        // Сглаженные позиции ключевых суставов
        private Vector3 _sHead, _sHandL, _sHandR, _sShoulderL, _sShoulderR;
        private Vector3 _sHipC, _sFootL, _sFootR;

        private bool _initialized;

        // ----------------------------------------------------------------

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            foreach (GestureType g in System.Enum.GetValues(typeof(GestureType)))
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
            if (!_initialized)
            {
                InitSmoothed();
                _initialized = true;
            }
        }

        private void OnPlayerLost()
        {
            _skeleton    = null;
            _initialized = false;
        }

        // ----------------------------------------------------------------

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

        // ----------------------------------------------------------------
        // Smoothing
        // ----------------------------------------------------------------

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
            _sHead      = Lerp(_sHead,      _skeleton.GetJointPosition(JointType.Head),          a);
            _sHandL     = Lerp(_sHandL,     _skeleton.GetJointPosition(JointType.HandLeft),       a);
            _sHandR     = Lerp(_sHandR,     _skeleton.GetJointPosition(JointType.HandRight),      a);
            _sShoulderL = Lerp(_sShoulderL, _skeleton.GetJointPosition(JointType.ShoulderLeft),   a);
            _sShoulderR = Lerp(_sShoulderR, _skeleton.GetJointPosition(JointType.ShoulderRight),  a);
            _sHipC      = Lerp(_sHipC,      _skeleton.GetJointPosition(JointType.HipCenter),      a);
            _sFootL     = Lerp(_sFootL,     _skeleton.GetJointPosition(JointType.FootLeft),        a);
            _sFootR     = Lerp(_sFootR,     _skeleton.GetJointPosition(JointType.FootRight),       a);
        }

        private static Vector3 Lerp(Vector3 prev, Vector3 next, float alpha)
            => Vector3.Lerp(prev, next, alpha);

        // ----------------------------------------------------------------
        // Cooldown
        // ----------------------------------------------------------------

        private void TickCooldowns()
        {
            foreach (GestureType g in System.Enum.GetValues(typeof(GestureType)))
            {
                if (_coolTimers[g] > 0f)
                    _coolTimers[g] -= Time.deltaTime;
            }
        }

        private bool IsOnCooldown(GestureType g) => _coolTimers[g] > 0f;

        // ----------------------------------------------------------------
        // Hold-timer logic
        // ----------------------------------------------------------------

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
            Debug.Log($"[GestureManager] Жест: {type} ({hand})");
        }

        // ----------------------------------------------------------------
        // Gesture checks
        // ----------------------------------------------------------------

        // 1. Поднятие руки — рука выше головы
        private void CheckRaiseHand()
        {
            bool leftUp  = _sHandL.y > _sHead.y + _raiseThreshold;
            bool rightUp = _sHandR.y > _sHead.y + _raiseThreshold;

            if (leftUp || rightUp)
            {
                var side = (leftUp && rightUp) ? HandSide.Both
                         : leftUp              ? HandSide.Left
                                               : HandSide.Right;
                HandleGestureHold(GestureType.RaiseHand, true, side);
            }
            else
            {
                HandleGestureHold(GestureType.RaiseHand, false);
            }
        }

        // 2. Шаг вперёд — один ботинок выдвинулся по Z
        private void CheckStepForward()
        {
            float avgFoot = (_sFootL.z + _sFootR.z) * 0.5f;
            float hip     = _sHipC.z;
            bool  stepped = (avgFoot - hip) > _stepThreshold;
            HandleGestureHold(GestureType.StepForward, stepped);
        }

        // 3. Поворот — правое плечо сильно впереди/позади левого по Z
        private void CheckTurn()
        {
            float delta   = Mathf.Abs(_sShoulderR.z - _sShoulderL.z);
            HandleGestureHold(GestureType.Turn, delta > _turnThreshold);
        }

        // 4. Стоп — обе руки горизонтально разведены
        private void CheckStop()
        {
            float span    = Mathf.Abs(_sHandR.x - _sHandL.x);
            float yDeltaL = Mathf.Abs(_sHandL.y - _sShoulderL.y);
            float yDeltaR = Mathf.Abs(_sHandR.y - _sShoulderR.y);
            bool  ok      = span > _stopArmSpan && yDeltaL < 0.15f && yDeltaR < 0.15f;
            HandleGestureHold(GestureType.Stop, ok, HandSide.Both);
        }
    }
}
