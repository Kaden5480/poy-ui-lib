using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UILib.Behaviours {
    /**
     * <summary>
     * Runs a timer.
     * </summary>
     */
    public class Timer : MonoBehaviour {
        // The timer coroutine
        private IEnumerator coroutine;

        private class FloatEvent : UnityEvent<float> {}

        private UnityEvent startEvent = new UnityEvent();
        private FloatEvent iterEvent = new FloatEvent();
        private UnityEvent endEvent = new UnityEvent();

        /**
         * <summary>
         * Starts the timer.
         * If the timer is already running, this will restart it.
         * </summary>
         * <param name="time">How long the timer should last for</param>
         */
        public void StartTimer(float time) {
            StopTimer();
            coroutine = RunTimer(time);
            StartCoroutine(coroutine);
        }

        /**
         * <summary>
         * Stops the timer immediately.
         * </summary>
         */
        public void StopTimer() {
            if (coroutine != null) {
                StopCoroutine();
            }
        }

        /**
         * <summary>
         * Adds a listener for the start of the timer.
         * </summary>
         * <param name="callback">The callback to run</param>
         */
        public void AddStartListener(UnityAction callback) {
            startEvent.AddListener(callback);
        }

        /**
         * <summary>
         * Adds a listener for each iteration of the timer.
         *
         * The argument provided to the callback is the
         * amount of time left on the timer (in seconds).
         * </summary>
         * <param name="callback">The callback to run</param>
         */
        public void AddListener(UnityAction<float> callback) {
            iterEvent.AddListener(callback);
        }

        /**
         * <summary>
         * Adds a listener for when the timer finishes.
         * </summary>
         * <param name="callback">The callback to run</param>
         */
        public void AddEndListener(UnityAction callback) {
            endEvent.AddListener(callback);
        }

        /**
         * <summary>
         * Runs the timer.
         * </summary>
         * <param name="time">How long the timer should run for</param>
         */
        private IEnumerator RunTimer(float time) {
            startEvent.Invoke();

            while (time > 0) {
                time -= Time.deltaTime;
                iterEvent.Invoke(time);
                yield return null;
            }

            endEvent.Invoke();
            yield break;
        }
    }
}
