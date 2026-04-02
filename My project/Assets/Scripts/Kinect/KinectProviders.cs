using System;
using UnityEngine;

namespace EduMotion.Kinect
{
    public interface IKinectProvider
    {
        bool         Initialize();
        SkeletonData GetPrimarySkeleton();
        void         Shutdown();
    }

    public class KinectV1Provider : IKinectProvider
    {
        private readonly int _sensorIndex, _elevationAngle;
        private IntPtr _handle = IntPtr.Zero;

        public KinectV1Provider(int idx, int angle) { _sensorIndex = idx; _elevationAngle = angle; }

        public bool Initialize()
        {
            try
            {
                int hr = NativeMethods.NuiCreateSensorByIndex(_sensorIndex, out _handle);
                if (hr < 0) { Debug.LogError($"[KinectV1] NuiCreateSensorByIndex hr={hr:X8}"); return false; }
                hr = NativeMethods.NuiInitialize(_handle, NativeMethods.NUI_INITIALIZE_FLAG_USES_SKELETON);
                if (hr < 0) { Debug.LogError($"[KinectV1] NuiInitialize hr={hr:X8}"); return false; }
                NativeMethods.NuiCameraElevationSetAngle(_handle, _elevationAngle);
                Debug.Log("[KinectV1] OK");
                return true;
            }
            catch (Exception e) { Debug.LogError($"[KinectV1] {e.Message}"); return false; }
        }

        public SkeletonData GetPrimarySkeleton()
        {
            var frame = new NativeMethods.NUI_SKELETON_FRAME();
            if (NativeMethods.NuiSkeletonGetNextFrame(_handle, 0, ref frame) < 0) return null;
            var sp = NativeMethods.SmoothingParams.Default;
            NativeMethods.NuiTransformSmooth(_handle, ref frame, ref sp);
            foreach (var s in frame.SkeletonData)
                if (s.eTrackingState == NativeMethods.NUI_SKELETON_TRACKING_STATE.SkeletonTracked)
                    return NativeMethods.ConvertToSkeletonData(s);
            return null;
        }

        public void Shutdown()
        {
            if (_handle != IntPtr.Zero) { NativeMethods.NuiShutdown(_handle); _handle = IntPtr.Zero; }
        }
    }

    public class MockKinectProvider : IKinectProvider
    {
        private SkeletonData _data;
        private float _t;

        public bool Initialize() { _data = Build(); Debug.Log("[Mock] OK"); return true; }

        public SkeletonData GetPrimarySkeleton()
        {
            _t += Time.deltaTime;
            _data.Joints[(int)JointType.HandRight].Position = new Vector3(0.3f, 0.5f + Mathf.Sin(_t*2f)*0.3f, 0f);
            return _data;
        }

        public void Shutdown() {}

        private static SkeletonData Build()
        {
            var d = new SkeletonData { TrackingId=1, IsTracked=true };
            for (int i=0;i<(int)JointType.Count;i++)
                d.Joints[i] = new Joint { JointType=(JointType)i, Position=Vector3.zero, TrackingState=TrackingState.Tracked };
            d.Joints[(int)JointType.Head].Position          = new Vector3(0f, 0.5f, 0f);
            d.Joints[(int)JointType.ShoulderCenter].Position = new Vector3(0f, 0.3f, 0f);
            d.Joints[(int)JointType.HandLeft].Position      = new Vector3(-0.3f, 0.2f, 0f);
            d.Joints[(int)JointType.HandRight].Position     = new Vector3(0.3f, 0.2f, 0f);
            d.Joints[(int)JointType.FootLeft].Position      = new Vector3(-0.1f,-0.5f, 0f);
            d.Joints[(int)JointType.FootRight].Position     = new Vector3(0.1f,-0.5f, 0f);
            return d;
        }
    }
}
