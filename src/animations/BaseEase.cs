using UnityEngine;

using UILib.Behaviours;

namespace UILib.Animations {
    /**
     * <summary>
     * The base class of all "ease" behaviours.
     *
     * Eases between two values, supports reversing,
     * and different curves for easing in/out.
     * </summary>
     */
    public abstract class BaseEase : BaseTimer {
        /**
         * <summary>
         * The minimum value.
         * </summary>
         */
        public float minValue { get; private set; } = 0f;

        /**
         * <summary>
         * The maximum value.
         * </summary>
         */
        public float maxValue { get; private set; } = 1f;

        /**
         * <summary>
         * The current value.
         * </summary>
         */
        public float value { get; private set; } = 0f;

        /**
         * <summary>
         * Whether this behaviour is currently easing in.
         * </summary>
         */
        public bool easingIn {
            get => running == true && increasing == true;
        }

        /**
         * <summary>
         * Whether this behaviour is currently easing out.
         * </summary>
         */
        public bool easingOut {
            get => running == true && increasing == false;
        }

        /**
         * <summary>
         * Sets the current value.
         *
         * This is clamped between <see cref="minValue"/>
         * and <see cref="maxValue"/>.
         *
         * This is called in <see cref="OnIter"/> to update
         * the current easing value.
         * </summary>
         * <param name="value">The value to set</param>
         */
        protected virtual void _SetValue(float value) {
            this.value = Mathf.Clamp(value, minValue, maxValue);
        }

        /**
         * <summary>
         * Sets the minimum and maximum values to ease between.
         * </summary>
         * <param name="minValue">The minimum value to use</param>
         * <param name="maxValue">The maximum value to use</param>
         */
        protected void _SetValues(float minValue, float maxValue) {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        /**
         * <summary>
         * Eases in, heading towards the <see cref="maxValue"/>.
         * </summary>
         * <param name="force">Whether to force easing in</param>
         */
        public void EaseIn(bool force = false) {
            SetIncreasing(true);

            // ForceRun uses the increasing/decreasing state
            // set on the timer to know where to end
            if (force == true) {
                ForceRun();
                return;
            }

            StartTimer();
        }

        /**
         * <summary>
         * Eases out, heading towards the <see cref="minValue"/>.
         * </summary>
         * <param name="force">Whether to force easing in</param>
         */
        public void EaseOut(bool force = false) {
            SetIncreasing(false);

            // ForceRun uses the increasing/decreasing state
            // set on the timer to know where to end
            if (force == true) {
                ForceRun();
                return;
            }

            StartTimer();
        }

        /**
         * <summary>
         * Runs on each iteration of easing.
         * </summary>
         * <param name="time">The current value of the internal timer</param>
         */
        protected override void OnIter(float time) {
            base.OnIter(time);

            // If the duration is 0, this needs to be handled
            // in a very specific way
            if (duration <= 0f) {
                // Increasing means easing in to max value
                // Decreasing means easing out to min value
                _SetValue((increasing == true) ? maxValue : minValue);
                return;
            }

            // Otherwise, the current time has to scale to be between
            // the minimum and maximum configured values

            // This first requires normalising the time
            float normal = time / duration;

            // Then, the normalised time has to be used to scale the value
            float value = minValue + (normal * Mathf.Abs(maxValue - minValue));
            _SetValue(value);
        }

        /**
         * <summary>
         * Runs when this behaviour finishes easing in/out.
         * </summary>
         */
        protected override void OnEnd() {
            base.OnEnd();

            // Increasing means easing in
            if (increasing == true) {
                OnEaseIn();
            }
            else {
                OnEaseOut();
            }
        }

        /**
         * <summary>
         * Runs when easing in has finished.
         * </summary>
         */
        protected virtual void OnEaseIn() {}

        /**
         * <summary>
         * Runs when easing out has finished.
         * </summary>
         */
        protected virtual void OnEaseOut() {}
    }
}
