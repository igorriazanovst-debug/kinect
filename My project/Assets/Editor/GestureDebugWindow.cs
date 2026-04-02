using UnityEngine;
using UnityEditor;
using EduMotion.Kinect;

namespace EduMotion.Editor
{
    public class GestureDebugWindow : EditorWindow
    {
        private Vector2 _scroll;

        [MenuItem("EduMotion/Gesture Debug Window")]
        public static void Open() => GetWindow<GestureDebugWindow>("Gesture Debug");

        private void OnEnable()  => EditorApplication.update += Repaint;
        private void OnDisable() => EditorApplication.update -= Repaint;

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Start Play mode and stand in front of camera.", MessageType.Info);
                return;
            }

            var km = KinectManager.Instance;
            if (km == null) { EditorGUILayout.LabelField("KinectManager not found."); return; }

            var sk = km.PrimaryPlayer;
            if (sk == null || !sk.IsTracked) { EditorGUILayout.LabelField("Skeleton not detected."); return; }

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            EditorGUILayout.LabelField("== Joints ==", EditorStyles.boldLabel);
            DrawJoint(sk, JointType.Head,         "Head");
            DrawJoint(sk, JointType.HandLeft,      "HandLeft");
            DrawJoint(sk, JointType.HandRight,     "HandRight");
            DrawJoint(sk, JointType.ShoulderLeft,  "ShoulderLeft");
            DrawJoint(sk, JointType.ShoulderRight, "ShoulderRight");
            DrawJoint(sk, JointType.HipCenter,     "HipCenter");
            DrawJoint(sk, JointType.FootLeft,      "FootLeft");
            DrawJoint(sk, JointType.FootRight,     "FootRight");
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("== Gesture Deltas ==", EditorStyles.boldLabel);
            var head  = sk.GetJointPosition(JointType.Head);
            var handL = sk.GetJointPosition(JointType.HandLeft);
            var handR = sk.GetJointPosition(JointType.HandRight);
            var shlL  = sk.GetJointPosition(JointType.ShoulderLeft);
            var shlR  = sk.GetJointPosition(JointType.ShoulderRight);
            var hipC  = sk.GetJointPosition(JointType.HipCenter);
            var footL = sk.GetJointPosition(JointType.FootLeft);
            var footR = sk.GetJointPosition(JointType.FootRight);
            float raiseL    = handL.y - head.y;
            float raiseR    = handR.y - head.y;
            float stepDelta = ((footL.z + footR.z) * 0.5f) - hipC.z;
            float turnDelta = Mathf.Abs(shlR.z - shlL.z);
            float stopSpan  = Mathf.Abs(handR.x - handL.x);
            float stopDyL   = Mathf.Abs(handL.y - shlL.y);
            float stopDyR   = Mathf.Abs(handR.y - shlR.y);
            EditorGUILayout.LabelField(string.Format("RaiseHand  Left  dy = {0:F4}  (threshold > 0.10)", raiseL));
            EditorGUILayout.LabelField(string.Format("RaiseHand  Right dy = {0:F4}  (threshold > 0.10)", raiseR));
            EditorGUILayout.LabelField(string.Format("StepForward    dz = {0:F4}  (threshold > 0.30)", stepDelta));
            EditorGUILayout.LabelField(string.Format("Turn           dz = {0:F4}  (threshold > 0.25)", turnDelta));
            EditorGUILayout.LabelField(string.Format("Stop  spanX    = {0:F4}  (threshold > 0.35)", stopSpan));
            EditorGUILayout.LabelField(string.Format("Stop  dyL      = {0:F4}  (threshold < 0.15)", stopDyL));
            EditorGUILayout.LabelField(string.Format("Stop  dyR      = {0:F4}  (threshold < 0.15)", stopDyR));
            EditorGUILayout.EndScrollView();
        }

        private static void DrawJoint(SkeletonData sk, JointType jt, string label)
        {
            var p = sk.GetJointPosition(jt);
            EditorGUILayout.LabelField(string.Format("  {0,-18} x={1:F3}  y={2:F3}  z={3:F3}", label, p.x, p.y, p.z));
        }
    }
}

