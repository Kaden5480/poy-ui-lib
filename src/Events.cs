using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace UILib.Events {
    /**
     * <summary>
     * An event type which passes a value
     * to its listeners.
     * </summary>
     */
    public class ValueEvent<T> : UnityEvent<T> {}

    /**
     * <summary>
     * An event type which passes two values
     * to its listeners.
     * </summary>
     */
    public class ValueEvent<T1, T2> : UnityEvent<T1, T2> {}

    /**
     * <summary>
     * An event type which passes a `Vector2` position
     * to listeners when the mouse is clicked/dragged.
     * This will be the current position of the mouse.
     * </summary>
     */
    public class MouseEvent : UnityEvent<Vector2> {}

    /**
     * <summary>
     * An event type used for significant aspects of UILib's API.
     * </summary>
     */
    public class UIEvent {
        private Logger logger;
        private Type type;
        private string name;

        // The callbacks to invoke
        private List<Action> listeners = new List<Action>();

        /**
         * <summary>
         * Initializes an event.
         * </summary>
         */
        internal UIEvent() {
            type = GetType();
            name = "UIEvent";
            logger = new Logger(type);
        }

        /**
         * <summary>
         * Initializes an event for a custom type
         * with a specified name.
         * </summary>
         * <param name="type">The type this event is on</param>
         * <param name="name">The name of this event</param>
         */
        internal UIEvent(Type type, string name) {
            this.type = type;
            this.name = name;
            logger = new Logger($"{type}.{name}");
        }

        /**
         * <summary>
         * Adds a listener to this event.
         * </summary>
         * <param name="listener">The listener to add</param>
         */
        public void AddListener(Action listener) {
            if (listeners.Contains(listener) == true) {
                logger.LogDebug($"Can't add \"{listener}\", it's already been added");
                return;
            }

            listeners.Add(listener);
        }

        /**
         * <summary>
         * Invokes the provided listener, catching exceptions.
         * </summary>
         * <param name="listener">The listener to invoke</param>
         */
        private void Invoke(Action listener) {
            try {
                listener();
            }
            catch (Exception e) {
                logger.LogError(
                    $"Failed invoking listener: {e.GetType().Name}\n"
                    + $"Source: {e.TargetSite.DeclaringType}.{e.TargetSite.Name}\n"
                    + $"Reason: {e.Message}\n"
                    + $"Stack Trace:\n{e.StackTrace}"
                );
            }
        }

        /**
         * <summary>
         * Invokes all listeners, handling any exceptions
         * that may happen.
         * </summary>
         */
        internal void Invoke() {
            foreach (Action listener in listeners) {
                Invoke(listener);
            }
        }
    }
}
