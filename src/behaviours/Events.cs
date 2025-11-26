using UnityEngine;
using UnityEngine.Events;

namespace UILib.Behaviours {
    /**
     * <summary>
     * An event type which passes a Vector2 position
     * to listeners when the mouse is dragged.
     * </summary>
     */
    public class DragEvent : UnityEvent<Vector2> {}

    /**
     * <summary>
     * An event type which passes a <see cref="string"/> value
     * to its listeners.
     * </summary>
     */
    public class StringEvent : UnityEvent<string> {}

    /**
     * <summary>
     * An event type which passes a <see cref="float"/> value
     * to its listeners.
     * </summary>
     */
    public class FloatEvent : UnityEvent<float> {}
}
