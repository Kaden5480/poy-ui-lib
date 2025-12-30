using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Events;

using UILib.Events;

namespace UILib.Behaviours {
    /**
     * <summary>
     * A class which supports easing between a
     * minimum and maximum value.
     * </summary>
     */
    public class Ease : MonoBehaviour {
        internal Logger logger { get; private set; }

        // The current ease coroutine
        private IEnumerator coroutine = null;

        /**
         * The normalised time which has passed.
         * Make sure this is always between 0-1 inclusive.
         */
        internal float timer = 0f;

        /**
         * <summary>
         * The value easing is currently at (based upon
         * <see cref="easeInValue"/> and <see cref="easeOutValue"/>.
         * </summary>
         */
        public float current {
            get => GetCurrent();
        }

        /**
         * <summary>
         * The curve function this ease should use for easing in.
         * </summary>
         */
        public Func<float, float> easeInFunction = null;

        /**
         * <summary>
         * The curve function this ease should use for easing out.
         * </summary>
         */
        public Func<float, float> easeOutFunction = null;

        /**
         * <summary>
         * How long it should take for this behaviour to finish easing in.
         * </summary>
         */
        public float easeInTime = 0f;

        /**
         * <summary>
         * How long it should take for this behaviour to finish easing out.
         * </summary>
         */
        public float easeOutTime = 0f;

        /**
         * <summary>
         * Whether this behaviour is currently easing in.
         * </summary>
         */
        public bool easingIn { get; private set; } = false;

        /**
         * <summary>
         * Whether this behaviour is currently easing out.
         * </summary>
         */
        public bool easingOut { get; private set;  } = false;

        /**
         * <summary>
         * The value to ease in to.
         * </summary>
         */
        public float easeInValue { get; private set; } = 1f;

        /**
         * <summary>
         * The value to ease out to.
         * </summary>
         */
        public float easeOutValue { get; private set; } = 0f;

        /**
         * <summary>
         * Invokes listeners when easing in finishes.
         * </summary>
         */
        public UnityEvent onEaseIn { get; private set; } = new UnityEvent();

        /**
         * <summary>
         * Invokes listeners when easing out finishes.
         * </summary>
         */
        public UnityEvent onEaseOut { get; private set; } = new UnityEvent();

        /**
         * <summary>
         * Invokes listeners on each iteration of easing
         * with the current value (based upon the <see cref="SetLimits">limits
         * which were set</see>).
         * </summary>
         */
        public ValueEvent<float> onEase { get; private set; } = new ValueEvent<float>();

        /**
         * <summary>
         * Initializes this ease behaviour.
         * </summary>
         */
        private void Awake() {
            logger = new Logger($"{GetType()}.{gameObject.name}");
        }

        /**
         * <summary>
         * Gets the value after applying a provided ease function.
         *
         * A `null` `easeFunction` means to leave the value as-is.
         * </summary>
         * <param name="value">The value to apply the ease function to</param>
         * <param name="easeFunction">The ease function to apply</param>
         * <returns>The current value (to send to listeners)</returns>
         */
        internal float GetValue(float value, Func<float, float> easeFunction = null) {
            float result = value;

            if (easeFunction != null) {
                result = easeFunction(value);
            }

            return easeOutValue + (result * Mathf.Abs(easeInValue - easeOutValue));
        }

        /**
         * <summary>
         * Gets the value for the current easing status.
         * </summary>
         * <returns>The current value</returns>
         */
        private float GetCurrent() {
            if (easingIn == true) {
                return GetValue(timer, easeInFunction);
            }

            if (easingOut == true) {
                return GetValue(timer, easeOutFunction);
            }

            return GetValue(timer, null);
        }

        /**
         * <summary>
         * Sets the ease-in and ease-out times.
         * provided time.
         * </summary>
         * <param name="easeOutTime">The time to set for easing out</param>
         * <param name="easeInTime">The time to set for easing in</param>
         */
        public virtual void SetTimes(float easeInTime, float easeOutTime) {
            this.easeInTime = easeInTime;
            this.easeOutTime = easeOutTime;
        }

        /**
         * <summary>
         * Sets both the ease-in and ease-out times to the
         * provided time.
         * </summary>
         * <param name="time">The time to set</param>
         */
        public virtual void SetTimes(float time) {
            SetTimes(time, time);
        }

        /**
         * <summary>
         * Sets the values to ease between.
         * </summary>
         * <param name="easeInValue">The value to ease in to (maximum)</param>
         * <param name="easeOutValue">The value to ease out to (minimum)</param>
         */
        public virtual void SetLimits(float easeInValue, float easeOutValue) {
            this.easeInValue = easeInValue;
            this.easeOutValue = easeOutValue;
        }

        /**
         * <summary>
         * The coroutine which handles easing in.
         * </summary>
         * <param name="totalTime">The total time to take when easing</param>
         */
        private IEnumerator EaseInRoutine(float totalTime) {
            if (totalTime <= 0f) {
                timer = 1f;
            }

            while (timer < 1f) {
                timer = Mathf.MoveTowards(
                    timer, 1f, Time.deltaTime / totalTime
                );
                onEase.Invoke(current);
                yield return null;
            }

            onEase.Invoke(current);

            coroutine = null;
            easingIn = false;
            onEaseIn.Invoke();
            yield break;
        }

        /**
         * <summary>
         * The coroutine which handles easing out.
         * </summary>
         * <param name="totalTime">The total time to take when easing</param>
         */
        private IEnumerator EaseOutRoutine(float totalTime) {
            if (totalTime <= 0f) {
                timer = 0f;
            }

            while (timer > 0f) {
                timer = Mathf.MoveTowards(
                    timer, 0f, Time.deltaTime / totalTime
                );
                onEase.Invoke(current);
                yield return null;
            }

            onEase.Invoke(current);

            coroutine = null;
            easingOut = false;
            onEaseOut.Invoke();
            yield break;
        }

        /**
         * <summary>
         * Eases in using the provided time.
         * </summary>
         * <param name="time">How long easing in should take</param>
         */
        public void EaseIn(float time) {
            if (time < 0f) {
                logger.LogError("Can't ease using a negative time");
                return;
            }

            Stop();
            coroutine = EaseInRoutine(time);
            easingIn = true;
            StartCoroutine(coroutine);
        }

        /**
         * <summary>
         * Eases out using the provided time.
         * </summary>
         * <param name="time">How long easing out should take</param>
         */
        public void EaseOut(float time) {
            if (time < 0f) {
                logger.LogError("Can't ease using a negative time");
                return;
            }

            Stop();
            coroutine = EaseOutRoutine(time);
            easingOut = true;
            StartCoroutine(coroutine);
        }

        /**
         * <summary>
         * Stops the ease coroutine immediately.
         *
         * Note:
         * This won't invoke <see cref="onEaseIn"/> or <see cref="onEaseOut"/>.
         * </summary>
         */
        public void Stop() {
            if (coroutine != null) {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            easingIn = false;
            easingOut = false;
        }
    }
}
