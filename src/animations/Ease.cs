using UnityEngine.Events;

using UILib.Events;

namespace UILib.Animations {
    /**
     * <summary>
     * A simple ease behaviour which eases between two
     * values, sending <see cref="onEase"/> events on
     * each iteration.
     * </summary>
     */
    public class Ease : BaseEase {
        /**
         * <summary>
         * Invokes listeners on each iteration of easing.
         * </summary
         */
        public ValueEvent<float> onEase { get; } = new ValueEvent<float>();

        /**
         * <summary>
         * Invokes listeners when easing in finishes.
         * </summary
         */
        public UnityEvent onEaseIn { get; } = new UnityEvent();

        /**
         * <summary>
         * Invokes listeners when easing out finishes.
         * </summary
         */
        public UnityEvent onEaseOut { get; } = new UnityEvent();

        /**
         * <summary>
         * Sets the current value.
         *
         * This will be clamped between <see cref="BaseEase.minValue"/>
         * and <see cref="BaseEase.maxValue"/>.
         *
         * This is called in <see cref="OnIter"/> to update
         * the current easing value.
         * </summary>
         * <param name="value">The value to set</param>
         */
        public void SetValue(float value) {
            _SetValue(value);
        }

        /**
         * <summary>
         * Sets the minimum and maximum values to ease between.
         * </summary>
         * <param name="minValue">The minimum value to use</param>
         * <param name="maxValue">The maximum value to use</param>
         */
        public void SetValues(float minValue, float maxValue) {
            _SetValues(minValue, maxValue);
        }


        /**
         * <summary>
         * Runs on each iteration of easing.
         * </summary>
         * <param name="time">The current value of the internal timer</param>
         */
        protected override void OnIter(float time) {
            base.OnIter(time);
            onEase.Invoke(value);
        }

        /**
         * <summary>
         * Runs when easing in has finished.
         * </summary>
         */
        protected override void OnEaseIn() { onEaseIn.Invoke(); }

        /**
         * <summary>
         * Runs when easing out has finished.
         * </summary>
         */
        protected override void OnEaseOut() { onEaseOut.Invoke(); }
    }
}
