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
        protected float _minValue { get; private set; } = 0f;

        /**
         * <summary>
         * The maximum value.
         * </summary>
         */
        protected float _maxValue { get; private set; } = 1f;

        /**
         * <summary>
         * The current value.
         * </summary>
         */
        protected float _value { get; private set; } = 0f;

        /**
         * <summary>
         * Whether this behaviour is currently easing in.
         * </summary>
         */
        protected bool _easingIn {
            get => running == true && increasing == true;
        }

        /**
         * <summary>
         * Whether this behaviour is currently easing out.
         * </summary>
         */
        protected bool _easingOut {
            get => running == true && increasing == false;
        }

        /**
         * <summary>
         * Sets the current value.
         *
         * This is called in <see cref="OnIter"/> to update
         * the current easing value.
         * </summary>
         * <param name="value">The value to set</param>
         */
        protected virtual void _SetValue(float value) {
            this._value = value;
        }

        /**
         * <summary>
         * Sets the minimum and maximum values to ease between.
         * </summary>
         * <param name="minValue">The minimum value to use</param>
         * <param name="maxValue">The maximum value to use</param>
         */
        protected void _SetValues(float minValue, float maxValue) {
            this._minValue = minValue;
            this._maxValue = maxValue;
        }

        /**
         * <summary>
         * Eases in, heading towards the <see cref="_maxValue"/>.
         * </summary>
         * <param name="force">Whether to force easing in</param>
         */
        protected void _EaseIn(bool force = false) {
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
         * Eases out, heading towards the <see cref="_minValue"/>.
         * </summary>
         * <param name="force">Whether to force easing in</param>
         */
        protected void _EaseOut(bool force = false) {
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
                _SetValue((increasing == true) ? _maxValue : _minValue);
                return;
            }

            // Otherwise, the current time has to scale to be between
            // the minimum and maximum configured values

            // This first requires normalising the time
            float normal = time / duration;

            // Then, the normalised time has to be used to scale the value
            float value = _minValue + (normal * Mathf.Abs(_maxValue - _minValue));
            _SetValue(value);
        }
    }
}
