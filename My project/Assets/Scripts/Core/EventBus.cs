using System;
using System.Collections.Generic;

namespace EduMotion.Core
{
    public static class EventBus
    {
        private static readonly Dictionary<string, List<Delegate>> _handlers = new();

        public static void Subscribe<T>(string eventName, Action<T> handler)
        {
            if (!_handlers.ContainsKey(eventName))
                _handlers[eventName] = new List<Delegate>();
            _handlers[eventName].Add(handler);
        }

        public static void Subscribe(string eventName, Action handler)
        {
            if (!_handlers.ContainsKey(eventName))
                _handlers[eventName] = new List<Delegate>();
            _handlers[eventName].Add(handler);
        }

        public static void Unsubscribe<T>(string eventName, Action<T> handler)
        {
            if (_handlers.ContainsKey(eventName))
                _handlers[eventName].Remove(handler);
        }

        public static void Unsubscribe(string eventName, Action handler)
        {
            if (_handlers.ContainsKey(eventName))
                _handlers[eventName].Remove(handler);
        }

        public static void Publish<T>(string eventName, T data)
        {
            if (!_handlers.ContainsKey(eventName)) return;
            foreach (var handler in _handlers[eventName])
                (handler as Action<T>)?.Invoke(data);
        }

        public static void Publish(string eventName)
        {
            if (!_handlers.ContainsKey(eventName)) return;
            foreach (var handler in _handlers[eventName])
                (handler as Action)?.Invoke();
        }

        public static void Clear()
        {
            _handlers.Clear();
        }
    }

    public static class GameEvents
    {
        public const string GestureDetected     = "GestureDetected";
        public const string GameStarted         = "GameStarted";
        public const string GameFinished        = "GameFinished";
        public const string RewardGranted       = "RewardGranted";
        public const string SceneChangeRequest  = "SceneChangeRequest";
        public const string KinectConnected     = "KinectConnected";
        public const string KinectDisconnected  = "KinectDisconnected";
        public const string PlayerDetected      = "PlayerDetected";
        public const string PlayerLost          = "PlayerLost";
    }
}
