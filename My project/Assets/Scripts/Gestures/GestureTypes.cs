namespace EduMotion.Gestures
{
    public enum GestureType { None=0, RaiseHand=1, StepForward=2, Turn=3, Stop=4 }
    public enum HandSide { Left, Right, Both }

    public class GestureEvent
    {
        public GestureType Type;
        public HandSide    Hand;
        public float       Confidence;
        public float       Timestamp;
    }
}
