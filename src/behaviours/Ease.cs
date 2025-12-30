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
        private Logger logger;

        // Normalised value currently at (used for easing)
        private float normalised = 0f;

        // The current ease coroutine
        private IEnumerator coroutine = null;

        /**
         * <summary>
         * The value easing is currently at (based upon
         * <see cref="easeInValue"/> and <see cref="easeOutValue"/>.
         * </summary>
         */
        public float current {
            get => easeOutValue
                + (normalised * Mathf.Abs(easeInValue - easeOutValue));
            set {
                normalised = (value - easeOutValue) / (Mathf.Abs(easeInValue - easeOutValue));
            }
        }

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
         * Sets the values to ease between.
         * </summary>
         * <param name="easeOutValue">The value to ease out to (minimum)</param>
         * <param name="easeInValue">The value to ease in to (maximum)</param>
         */
        public virtual void SetLimits(float easeOutValue, float easeInValue) {
            this.easeOutValue = easeOutValue;
            this.easeInValue = easeInValue;
        }

        /**
         * <summary>
         * Interpolates the current value based upon the provided delta.
         * </summary>
         * <param name="value">The value to interpolate</param>
         * <param name="delta">The delta to interpolate with</param>
         * <param name="min">The minimum possible value</param>
         * <param name="max">The maximum possible value</param>
         * <returns>The new, interpolated value</returns>
         */
        private float Interpolate(float value, float delta, float min = 0f, float max = 1f) {
            value += delta;
            return Mathf.Max(min, Mathf.Min(max, value));
        }

        /**
         * <summary>
         * The coroutine which handles easing in.
         * </summary>
         * <param name="time">The total time to take when easing</param>
         */
        private IEnumerator EaseInRoutine(float time) {
            if (time <= 0f) {
                normalised = 1f;
                onEase.Invoke(current);
            }

            while (normalised < 1f) {
                normalised = Interpolate(normalised, Time.deltaTime / time);
                onEase.Invoke(current);
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
         * <param name="time">The total time to take when easing</param>
         */
        private IEnumerator EaseOutRoutine(float time) {
            if (time <= 0f) {
                normalised = 0f;
                onEase.Invoke(current);
            }

            while (normalised > 0f) {
                normalised = Interpolate(normalised, -(Time.deltaTime / time));
                onEase.Invoke(current);
                yield return null;
            }

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
