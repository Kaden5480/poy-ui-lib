using UnityEngine.Events;

using UILib.Events;

namespace UILib.Behaviours {
    /**
     * <summary>
     * A reversible timer with a configurable duration
     * and time scale (speed).
     * </summary>
     */
    public class Timer : BaseTimer {
        /**
         * <summary>
         * Invokes listeners on each iteration of the timer,
         * passing the current value the timer is at.
         * </summary>
         */
        public ValueEvent<float> onIter { get; } = new ValueEvent<float>();

        /**
         * <summary>
         * Invokes listeners when the timer finishes running.
         * </summary>
         */
        public UnityEvent onEnd { get; } = new UnityEvent();

        /**
         * <summary>
         * Runs on each iteration of the timer.
         * </summary>
         * <param name="time">The current value of the timer</param>
         */
        protected override void OnIter(float time) {
            base.OnIter(time);
            onIter.Invoke(time);
        }

        /**
         * <summary>
         * Runs when the timer finishes.
         * </summary>
         */
        protected override void OnEnd() {
            base.OnEnd();
            onEnd.Invoke();
        }
    }
}
