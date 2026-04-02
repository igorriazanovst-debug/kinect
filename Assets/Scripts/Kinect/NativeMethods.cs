using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace EduMotion.Kinect
{
    internal static class NativeMethods
    {
        private const string KinectDll = "Kinect10";

        public const int  NUI_SKELETON_COUNT          = 6;
        public const int  NUI_SKELETON_POSITION_COUNT = 20;
        public const uint NUI_INITIALIZE_FLAG_USES_SKELETON = 0x00000008;

        public enum NUI_SKELETON_TRACKING_STATE { NotTracked=0, PositionOnly, SkeletonTracked }
        public enum NUI_SKELETON_POSITION_TRACKING_STATE { NotTracked=0, Inferred, Tracked }

        [StructLayout(LayoutKind.Sequential)]
        public struct Vector4 { public float x,y,z,w; }

        [StructLayout(LayoutKind.Sequential)]
        public struct NUI_SKELETON_DATA
        {
            public NUI_SKELETON_TRACKING_STATE eTrackingState;
            public uint dwTrackingID, dwEnrollmentIndex, dwUserIndex;
            public Vector4 Position;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=20)] public Vector4[] SkeletonPositions;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=20)] public NUI_SKELETON_POSITION_TRACKING_STATE[] eSkeletonPositionTrackingState;
            public uint dwQualityFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NUI_SKELETON_FRAME
        {
            public long liTimeStamp;
            public uint dwFrameNumber, dwFlags;
            public Vector4 vFloorClipPlane, vNormalToGravity;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=6)] public NUI_SKELETON_DATA[] SkeletonData;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NUI_TRANSFORM_SMOOTH_PARAMETERS
        {
            public float fSmoothing, fCorrection, fPrediction, fJitterRadius, fMaxDeviationRadius;
        }

        public static class SmoothingParams
        {
            public static readonly NUI_TRANSFORM_SMOOTH_PARAMETERS Default =
                new NUI_TRANSFORM_SMOOTH_PARAMETERS
                { fSmoothing=0.5f, fCorrection=0.5f, fPrediction=0.5f, fJitterRadius=0.05f, fMaxDeviationRadius=0.04f };
        }

        [DllImport(KinectDll)] public static extern int  NuiCreateSensorByIndex(int index, out IntPtr ppNuiSensor);
        [DllImport(KinectDll)] public static extern int  NuiInitialize(IntPtr p, uint dwFlags);
        [DllImport(KinectDll)] public static extern void NuiShutdown(IntPtr p);
        [DllImport(KinectDll)] public static extern int  NuiCameraElevationSetAngle(IntPtr p, int angle);
        [DllImport(KinectDll)] public static extern int  NuiSkeletonGetNextFrame(IntPtr p, uint ms, ref NUI_SKELETON_FRAME frame);
        [DllImport(KinectDll)] public static extern int  NuiTransformSmooth(IntPtr p, ref NUI_SKELETON_FRAME frame, ref NUI_TRANSFORM_SMOOTH_PARAMETERS sp);

        public static SkeletonData ConvertToSkeletonData(NUI_SKELETON_DATA n)
        {
            var data = new SkeletonData
            {
                TrackingId = (int)n.dwTrackingID,
                IsTracked  = n.eTrackingState == NUI_SKELETON_TRACKING_STATE.SkeletonTracked,
                Joints     = new Joint[20]
            };
            for (int i = 0; i < 20; i++)
            {
                var pos   = n.SkeletonPositions?[i] ?? new Vector4();
                var state = n.eSkeletonPositionTrackingState?[i] ?? NUI_SKELETON_POSITION_TRACKING_STATE.NotTracked;
                data.Joints[i] = new Joint
                {
                    JointType     = (JointType)i,
                    Position      = new UnityEngine.Vector3(pos.x, pos.y, pos.z),
                    TrackingState = (TrackingState)(int)state
                };
            }
            return data;
        }
    }
}
