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

        public UnityEvent onStart { get; } = new UnityEvent();
        public TimerEvent onIter  { get; } = new TimerEvent();
        public UnityEvent onEnd   { get; } = new UnityEvent();

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
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        /**
         * <summary>
         * Runs the timer.
         * </summary>
         * <param name="time">How long the timer should run for</param>
         */
        private IEnumerator RunTimer(float time) {
            onStart.Invoke();

            while (time > 0) {
                time -= Time.deltaTime;
                onIter.Invoke(time);
                yield return null;
            }

            onEnd.Invoke();
            yield break;
        }
    }
}
