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
        public float easeInTime { get; private set; } = 0f;

        /**
         * <summary>
         * How long it should take for this behaviour to finish easing out.
         * </summary>
         */
        public float easeOutTime { get; private set; } = 0f;

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
            if (easeInTime < 0f || easeOutTime < 0f) {
                logger.LogError("Can't use a negative ease time");
                return;
            }

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
         * Changes the timer by the provided time delta.
         * The delta must be normalised.
         * </summary>
         * <param name="delta">The delta to change the timer by</param>
         */
        internal void ChangeTimer(float delta) {
            timer = Mathf.Clamp(timer + delta, 0f, 1f);
        }

        /**
         * <summary>
         * The coroutine which handles easing in.
         * </summary>
         */
        private IEnumerator EaseInRoutine() {
            if (easeInTime <= 0f) {
                timer = 1f;
            }

            while (timer < 1f) {
                ChangeTimer(Time.deltaTime / easeInTime);
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
         */
        private IEnumerator EaseOutRoutine() {
            if (easeOutTime <= 0f) {
                timer = 0f;
            }

            while (timer > 0f) {
                ChangeTimer(-(Time.deltaTime / easeOutTime));
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
         * Starts easing in.
         * </summary>
         */
        public void EaseIn() {
            Stop();
            coroutine = EaseInRoutine();
            easingIn = true;
            StartCoroutine(coroutine);
        }

        /**
         * <summary>
         * Starts easing out.
         * </summary>
         */
        public void EaseOut() {
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
