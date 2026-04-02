using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace EduMotion.Kinect
{
    /// <summary>
    /// РџРѕР»СѓС‡Р°РµС‚ РґР°РЅРЅС‹Рµ СЃРєРµР»РµС‚Р° РїРѕ UDP РѕС‚ mediapipe_sender.py
    /// Р¤РѕСЂРјР°С‚: "j0x,y,z|j1x,y,z|...|j19x,y,z"
    /// </summary>
    public class UDPKinectProvider : IKinectProvider
    {
        private readonly int    _port;
        private UdpClient       _udp;
        private Thread          _thread;
        private SkeletonData    _latest;
        private bool            _running;
        private readonly object _lock = new object();

        public UDPKinectProvider(int port = 7777) { _port = port; }

        public bool Initialize()
        {
            try
            {
                _udp     = new UdpClient(_port);
                _running = true;
                _thread  = new Thread(ReceiveLoop) { IsBackground = true };
                _thread.Start();
                Debug.Log($"[UDPKinectProvider] Listening on UDP port {_port}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[UDPKinectProvider] Init failed: {e.Message}");
                return false;
            }
        }

        public SkeletonData GetPrimarySkeleton()
        {
            lock (_lock) { return _latest; }
        }

        public void Shutdown()
        {
            _running = false;
            _udp?.Close();
            _thread?.Join(500);
        }

        private void ReceiveLoop()
        {
            var ep = new IPEndPoint(IPAddress.Any, _port);
            while (_running)
            {
                try
                {
                    byte[] bytes = _udp.Receive(ref ep);
                    string msg   = Encoding.UTF8.GetString(bytes);
                    var data     = Parse(msg);
                    if (data != null)
                        lock (_lock) { _latest = data; }
                }
                catch (SocketException) { break; }
                catch (Exception e) { Debug.LogWarning($"[UDP] {e.Message}"); }
            }
        }

        // Р¤РѕСЂРјР°С‚: "j0:x,y,z|j1:x,y,z|...|j19:x,y,z|tracked:1"
        private static SkeletonData Parse(string msg)
        {
            var data = new SkeletonData { TrackingId = 1, IsTracked = true };
            for (int i = 0; i < (int)JointType.Count; i++)
                data.Joints[i] = new Joint
                {
                    JointType     = (JointType)i,
                    Position      = Vector3.zero,
                    TrackingState = TrackingState.Inferred
                };

            var parts = msg.Split('|');
            foreach (var part in parts)
            {
                if (part.StartsWith("tracked:"))
                {
                    data.IsTracked = part.Split(':')[1] == "1";
                    continue;
                }
                if (!part.StartsWith("j")) continue;

                var colonIdx = part.IndexOf(':');
                if (colonIdx < 0) continue;

                if (!int.TryParse(part.Substring(1, colonIdx - 1), out int idx)) continue;
                if (idx < 0 || idx >= (int)JointType.Count) continue;

                var coords = part.Substring(colonIdx + 1).Split(',');
                if (coords.Length < 3) continue;

                if (float.TryParse(coords[0], System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out float x) &&
                    float.TryParse(coords[1], System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out float y) &&
                    float.TryParse(coords[2], System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out float z))
                {
                    data.Joints[idx].Position      = new Vector3(x, y, z);
                    data.Joints[idx].TrackingState = TrackingState.Tracked;
                }
            }
            return data;
        }
    }
}
