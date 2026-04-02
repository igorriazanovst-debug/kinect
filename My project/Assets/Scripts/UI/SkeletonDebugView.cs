using UnityEngine;
using EduMotion.Core;
using EduMotion.Kinect;

namespace EduMotion.UI
{
    public class SkeletonDebugView : MonoBehaviour
    {
        [SerializeField] private bool  _showInRelease = false;
        [SerializeField] private float _jointRadius   = 8f;
        [SerializeField] private Color _jointColor    = Color.cyan;
        [SerializeField] private Color _boneColor     = Color.yellow;

        private SkeletonData _skeleton;

        private static readonly (JointType,JointType)[] _bones =
        {
            (JointType.Head,JointType.ShoulderCenter),(JointType.ShoulderCenter,JointType.ShoulderLeft),
            (JointType.ShoulderCenter,JointType.ShoulderRight),(JointType.ShoulderLeft,JointType.ElbowLeft),
            (JointType.ElbowLeft,JointType.WristLeft),(JointType.WristLeft,JointType.HandLeft),
            (JointType.ShoulderRight,JointType.ElbowRight),(JointType.ElbowRight,JointType.WristRight),
            (JointType.WristRight,JointType.HandRight),(JointType.ShoulderCenter,JointType.Spine),
            (JointType.Spine,JointType.HipCenter),(JointType.HipCenter,JointType.HipLeft),
            (JointType.HipCenter,JointType.HipRight),(JointType.HipLeft,JointType.KneeLeft),
            (JointType.KneeLeft,JointType.AnkleLeft),(JointType.AnkleLeft,JointType.FootLeft),
            (JointType.HipRight,JointType.KneeRight),(JointType.KneeRight,JointType.AnkleRight),
            (JointType.AnkleRight,JointType.FootRight),
        };

        private void OnEnable()  => EventBus.Subscribe<SkeletonData>(GameEvents.PlayerDetected, s => _skeleton = s);
        private void OnDisable() => EventBus.Unsubscribe<SkeletonData>(GameEvents.PlayerDetected, s => _skeleton = s);

        private void OnGUI()
        {
            #if !UNITY_EDITOR
            if (!_showInRelease) return;
            #endif
            if (_skeleton == null || !_skeleton.IsTracked) return;
            var cam = Camera.main; if (cam == null) return;
            GUI.color = _boneColor;
            foreach (var (a,b) in _bones) DrawLine(ToScreen(cam,_skeleton.GetJointPosition(a)), ToScreen(cam,_skeleton.GetJointPosition(b)), 2f);
            GUI.color = _jointColor;
            for (int i=0;i<(int)JointType.Count;i++) { var p=ToScreen(cam,_skeleton.GetJointPosition((JointType)i)); GUI.DrawTexture(new Rect(p.x-_jointRadius/2,p.y-_jointRadius/2,_jointRadius,_jointRadius),Texture2D.whiteTexture); }
        }

        private static Vector2 ToScreen(Camera cam, Vector3 w) => new Vector2(Screen.width*0.5f+w.x*Screen.width*0.5f, Screen.height*0.5f-w.y*Screen.height*0.5f);

        private static void DrawLine(Vector2 a, Vector2 b, float w)
        {
            var m = GUI.matrix; float angle=Mathf.Atan2(b.y-a.y,b.x-a.x)*Mathf.Rad2Deg; float dist=Vector2.Distance(a,b);
            GUIUtility.RotateAroundPivot(angle,a); GUI.DrawTexture(new Rect(a.x,a.y-w/2,dist,w),Texture2D.whiteTexture); GUI.matrix=m;
        }
    }
}
