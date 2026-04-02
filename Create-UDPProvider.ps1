$projectPath = "C:\Temp\2026\kinect\kinect\My project"
$repoPath    = "C:\Temp\2026\kinect\kinect"
$kinectPath  = "$projectPath\Assets\Scripts\Kinect"

# ---- UDPKinectProvider.cs ----
$udpProvider = @'
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace EduMotion.Kinect
{
    /// <summary>
    /// Получает данные скелета по UDP от mediapipe_sender.py
    /// Формат: "j0x,y,z|j1x,y,z|...|j19x,y,z"
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

        // Формат: "j0:x,y,z|j1:x,y,z|...|j19:x,y,z|tracked:1"
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
'@

# ---- mediapipe_sender.py ----
$pythonSender = @'
"""
mediapipe_sender.py
Захватывает позу с камеры через MediaPipe Pose,
маппит 33 лендмарка в 20 суставов EduMotion SkeletonData
и отправляет по UDP на Unity.

Установка:
    pip install mediapipe opencv-python

Запуск:
    python mediapipe_sender.py [--host 127.0.0.1] [--port 7777] [--cam 0]
"""

import argparse
import socket
import cv2
import mediapipe as mp

# MediaPipe Pose landmark indices
MP = mp.solutions.pose.PoseLandmark

# Маппинг EduMotion JointType (0-19) -> MediaPipe landmark index
# JointType: HipCenter=0,Spine=1,ShoulderCenter=2,Head=3,
#            ShoulderLeft=4,ElbowLeft=5,WristLeft=6,HandLeft=7,
#            ShoulderRight=8,ElbowRight=9,WristRight=10,HandRight=11,
#            HipLeft=12,KneeLeft=13,AnkleLeft=14,FootLeft=15,
#            HipRight=16,KneeRight=17,AnkleRight=18,FootRight=19
JOINT_MAP = {
    0:  [MP.LEFT_HIP, MP.RIGHT_HIP],          # HipCenter    — среднее
    1:  [MP.LEFT_SHOULDER, MP.RIGHT_SHOULDER], # Spine        — среднее плеч
    2:  [MP.LEFT_SHOULDER, MP.RIGHT_SHOULDER], # ShoulderCenter
    3:  [MP.NOSE],                             # Head
    4:  [MP.LEFT_SHOULDER],                    # ShoulderLeft
    5:  [MP.LEFT_ELBOW],                       # ElbowLeft
    6:  [MP.LEFT_WRIST],                       # WristLeft
    7:  [MP.LEFT_INDEX],                       # HandLeft
    8:  [MP.RIGHT_SHOULDER],                   # ShoulderRight
    9:  [MP.RIGHT_ELBOW],                      # ElbowRight
    10: [MP.RIGHT_WRIST],                      # WristRight
    11: [MP.RIGHT_INDEX],                      # HandRight
    12: [MP.LEFT_HIP],                         # HipLeft
    13: [MP.LEFT_KNEE],                        # KneeLeft
    14: [MP.LEFT_ANKLE],                       # AnkleLeft
    15: [MP.LEFT_FOOT_INDEX],                  # FootLeft
    16: [MP.RIGHT_HIP],                        # HipRight
    17: [MP.RIGHT_KNEE],                       # KneeRight
    18: [MP.RIGHT_ANKLE],                      # AnkleRight
    19: [MP.RIGHT_FOOT_INDEX],                 # FootRight
}


def avg_landmark(landmarks, indices):
    pts = [landmarks[i.value] for i in indices]
    x = sum(p.x for p in pts) / len(pts)
    y = sum(p.y for p in pts) / len(pts)
    z = sum(p.z for p in pts) / len(pts)
    # MediaPipe: x,y в [0,1] нормализованные, z — глубина
    # Конвертируем в метры ~[-1..1]
    return (x - 0.5) * 2.0, -(y - 0.5) * 2.0, z * 2.0


def build_message(landmarks, tracked):
    parts = []
    for joint_idx, mp_indices in JOINT_MAP.items():
        x, y, z = avg_landmark(landmarks, mp_indices)
        parts.append(f"j{joint_idx}:{x:.4f},{y:.4f},{z:.4f}")
    parts.append(f"tracked:{'1' if tracked else '0'}")
    return "|".join(parts)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--host", default="127.0.0.1")
    parser.add_argument("--port", type=int, default=7777)
    parser.add_argument("--cam",  type=int, default=0)
    args = parser.parse_args()

    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    pose = mp.solutions.pose.Pose(
        min_detection_confidence=0.5,
        min_tracking_confidence=0.5,
        model_complexity=1
    )
    cap = cv2.VideoCapture(args.cam)
    print(f"Sending to {args.host}:{args.port} | Press Q to quit")

    while cap.isOpened():
        ret, frame = cap.read()
        if not ret:
            break

        rgb     = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        results = pose.process(rgb)

        tracked = results.pose_landmarks is not None
        if tracked:
            msg  = build_message(results.pose_landmarks.landmark, True)
            data = msg.encode("utf-8")
            sock.sendto(data, (args.host, args.port))

            mp.solutions.drawing_utils.draw_landmarks(
                frame, results.pose_landmarks, mp.solutions.pose.POSE_CONNECTIONS)

        cv2.putText(frame, f"Tracked: {tracked}", (10, 30),
                    cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0) if tracked else (0, 0, 255), 2)
        cv2.imshow("MediaPipe Sender", frame)
        if cv2.waitKey(1) & 0xFF == ord("q"):
            break

    cap.release()
    cv2.destroyAllWindows()
    sock.close()


if __name__ == "__main__":
    main()
'@

Set-Content -Path "$kinectPath\UDPKinectProvider.cs" -Value $udpProvider   -Encoding UTF8
Set-Content -Path "$repoPath\mediapipe_sender.py"    -Value $pythonSender  -Encoding UTF8

Write-Host "Created: UDPKinectProvider.cs"
Write-Host "Created: mediapipe_sender.py"
