"""
mediapipe_sender.py
Р—Р°С…РІР°С‚С‹РІР°РµС‚ РїРѕР·Сѓ СЃ РєР°РјРµСЂС‹ С‡РµСЂРµР· MediaPipe Pose,
РјР°РїРїРёС‚ 33 Р»РµРЅРґРјР°СЂРєР° РІ 20 СЃСѓСЃС‚Р°РІРѕРІ EduMotion SkeletonData
Рё РѕС‚РїСЂР°РІР»СЏРµС‚ РїРѕ UDP РЅР° Unity.

РЈСЃС‚Р°РЅРѕРІРєР°:
    pip install mediapipe opencv-python

Р—Р°РїСѓСЃРє:
    python mediapipe_sender.py [--host 127.0.0.1] [--port 7777] [--cam 0]
"""

import argparse
import socket
import cv2
import mediapipe as mp

# MediaPipe Pose landmark indices
MP = mp.solutions.pose.PoseLandmark

# РњР°РїРїРёРЅРі EduMotion JointType (0-19) -> MediaPipe landmark index
# JointType: HipCenter=0,Spine=1,ShoulderCenter=2,Head=3,
#            ShoulderLeft=4,ElbowLeft=5,WristLeft=6,HandLeft=7,
#            ShoulderRight=8,ElbowRight=9,WristRight=10,HandRight=11,
#            HipLeft=12,KneeLeft=13,AnkleLeft=14,FootLeft=15,
#            HipRight=16,KneeRight=17,AnkleRight=18,FootRight=19
JOINT_MAP = {
    0:  [MP.LEFT_HIP, MP.RIGHT_HIP],          # HipCenter    вЂ” СЃСЂРµРґРЅРµРµ
    1:  [MP.LEFT_SHOULDER, MP.RIGHT_SHOULDER], # Spine        вЂ” СЃСЂРµРґРЅРµРµ РїР»РµС‡
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
    # MediaPipe: x,y РІ [0,1] РЅРѕСЂРјР°Р»РёР·РѕРІР°РЅРЅС‹Рµ, z вЂ” РіР»СѓР±РёРЅР°
    # РљРѕРЅРІРµСЂС‚РёСЂСѓРµРј РІ РјРµС‚СЂС‹ ~[-1..1]
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
