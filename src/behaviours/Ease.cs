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
        internal Logger logger { get; private set; } = new Logger(typeof(Ease));

        // The current ease coroutine
        private IEnumerator coroutine = null;

        /**
         * <summary>
         * The time which has passed.
         * </summary>
         */
        internal float timer = 0f;

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
         * Whether this behaviour is currently easing in.
         * </summary>
         */
        public bool easingIn { get; private set; } = false;

        /**
         * <summary>
         * Whether this behaviour is currently easing out.
         * </summary>
         */
        public bool easingOut { get; private set; } = false;

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
         * Gets the value after applying a provided ease function.
         *
         * A `null` `easeFunction` means to leave the value as-is.
         * </summary>
         * <param name="value">The value to apply the ease function to</param>
         * <param name="easeFunction">The ease function to apply</param>
         * <returns>The current value (to send to listeners)</returns>
         */
        internal float GetValue(float value, Func<float, float> easeFunction) {
            float result = Mathf.Clamp(value, 0f, 1f);

            if (easeFunction != null) {
                result = easeFunction(value);
            }

            return easeOutValue + (result * Mathf.Abs(easeInValue - easeOutValue));
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
         * Performs one iteration of this ease behaviour.
         * </summary>
         * <param name="delta">The time delta</param>
         * <param name="maxTime">The maximum time iterating should take</param>
         * <param name="easeFunction">The ease function to apply</param>
         */
        internal void Iterate(float delta, float maxTime, Func<float, float> easeFunction) {
            if (maxTime == 0f) {
                timer = (delta < 0) ? 0f : 1f;
            }
            else {
                timer = Mathf.Clamp(timer + delta / maxTime, 0f, 1f);
            }

            onEase.Invoke(GetValue(timer, easeFunction));
        }

        /**
         * <summary>
         * The coroutine which handles easing in.
         * </summary>
         */
        private IEnumerator EaseInRoutine() {
            // Fix the timer
            timer = Mathf.Clamp(timer, 0f, 1f);

            if (easeInTime <= 0f) {
                timer = 1f;
            }

            while (timer < 1f) {
                Iterate(Time.deltaTime, easeInTime, easeInFunction);
                yield return null;
            }

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
            // Fix the timer
            timer = Mathf.Clamp(timer, 0f, 1f);

            if (easeOutTime <= 0f) {
                timer = 0f;
            }

            while (timer > 0f) {
                Iterate(-Time.deltaTime, easeOutTime, easeOutFunction);
                yield return null;
            }

            coroutine = null;
            easingOut = false;
            onEaseOut.Invoke();
            yield break;
        }

        /**
         * <summary>
         * Starts easing in.
         * </summary>
         * <param name="force">Whether to force easing in</param>
         */
        public void EaseIn(bool force = false) {
            Stop();

            if (force == true) {
                Iterate(1f, 0f, null);
                onEaseIn.Invoke();
                return;
            }

            easingIn = true;
            coroutine = EaseInRoutine();
            StartCoroutine(coroutine);
        }

        /**
         * <summary>
         * Starts easing out.
         * </summary>
         * <param name="force">Whether to force easing out</param>
         */
        public void EaseOut(bool force = false) {
            Stop();

            if (force == true) {
                Iterate(-1f, 0f, null);
                onEaseOut.Invoke();
                return;
            }

            easingOut = true;
            coroutine = EaseOutRoutine();
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
