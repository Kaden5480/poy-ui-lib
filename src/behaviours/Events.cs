using UnityEngine;
using UnityEngine.Events;

namespace UILib.Behaviours {
    public class DragEvent : UnityEvent<Vector2> {}
    public class StringEvent : UnityEvent<string> {}
    public class FloatEvent : UnityEvent<float> {}
}
