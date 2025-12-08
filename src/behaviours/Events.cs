using UnityEngine;
using UnityEngine.Events;

namespace UILib.Behaviours {
    /**
     * <summary>
     * An event type which passes a value
     * to its listeners.
     * </summary>
     */
    public class ValueEvent<T> : UnityEvent<T> {}

    /**
     * <summary>
     * An event type which passes a `Vector2` position
     * to listeners when the mouse is clicked/dragged.
     * This will be the current position of the mouse.
     * </summary>
     */
    public class MouseEvent : UnityEvent<Vector2> {}
}
