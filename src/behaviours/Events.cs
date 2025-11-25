using UnityEngine;
using UnityEngine.Events;

namespace UILib.Behaviours {
    public class DragEvent : UnityEvent<Vector2> {}
    public class TimerEvent : UnityEvent<float> {}
}
