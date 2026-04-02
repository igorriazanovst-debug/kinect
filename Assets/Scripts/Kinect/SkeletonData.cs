using UnityEngine;

namespace EduMotion.Kinect
{
    public enum JointType
    {
        HipCenter=0,Spine,ShoulderCenter,Head,
        ShoulderLeft,ElbowLeft,WristLeft,HandLeft,
        ShoulderRight,ElbowRight,WristRight,HandRight,
        HipLeft,KneeLeft,AnkleLeft,FootLeft,
        HipRight,KneeRight,AnkleRight,FootRight,
        Count
    }

    public enum TrackingState { NotTracked=0, Inferred, Tracked }

    [System.Serializable]
    public class Joint
    {
        public JointType     JointType;
        public Vector3       Position;
        public TrackingState TrackingState;
    }

    [System.Serializable]
    public class SkeletonData
    {
        public int     TrackingId;
        public bool    IsTracked;
        public Joint[] Joints = new Joint[(int)JointType.Count];

        public Joint   GetJoint(JointType t)         => Joints[(int)t];
        public Vector3 GetJointPosition(JointType t) => Joints[(int)t]?.Position ?? Vector3.zero;
        public bool    IsJointTracked(JointType t)   => Joints[(int)t]?.TrackingState == TrackingState.Tracked;
    }
}
