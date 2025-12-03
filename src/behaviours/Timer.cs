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

        // The start and end times
        private float startTime;
        private float endTime;

        /**
         * <summary>
         * The time scale. By default this is `1`.
         * Scales how quickly the timer should run.
         *
         * The higher this value, the faster the timer runs.
         * The lower this value, the slower the timer runs.
         * </summary>
         */
        public float timeScale { get; private set; } = 1f;

        /**
         * <summary>
         * The current value of the timer.
         * </summary>
         */
        public float timer { get; private set; }

        /**
         * <summary>
         * Whether the timer coroutine is currently running.
         * </summary>
         */
        public bool running { get; private set; } = false;

        /**
         * <summary>
         * Whether the timer should pause.
         * </summary>
         */
        public bool paused { get; private set; } = false;

        /**
         * <summary>
         * Invokes listeners whenever the timer is started.
         * </summary>
         */
        public UnityEvent onStart { get; } = new UnityEvent();

        /**
         * <summary>
         * Invokes listeners on each iteration of the timer.
         * The value passed to listeners is the current value of the timer.
         * </summary>
         */
        public ValueEvent<float> onIter { get; } = new ValueEvent<float>();

        /**
         * <summary>
         * Invokes listeners when the timer finishes.
         * </summary>
         */
        public UnityEvent onEnd { get; } = new UnityEvent();

        /**
         * <summary>
         * Sets the <see cref="timeScale"/>.
         * </summary>
         * <param name="timeScaler">The new value for the time scaler</param>
         */
        public void SetTimeScale(float timeScale) {
            this.timeScale = timeScale;
        }

        /**
         * <summary>
         * Starts the timer.
         * If the timer is already running, this will restart it.
         *
         * Both the start and end time must be >= 0.
         * </summary>
         * <param name="startTime">What time the timer should start at</param>
         * <param name="endTime">What time the timer should end at</param>
         */
        public void StartTimer(float startTime, float endTime = 0f) {
            this.startTime = Mathf.Max(0f, startTime);
            this.endTime = Mathf.Max(0f, endTime);

            StopTimer();
            coroutine = RunTimer();
            running = true;
            StartCoroutine(coroutine);
        }

        /**
         * <summary>
         * Pauses the timer.
         *
         * If the timer is paused, the internal timer will no longer
         * decrease, and any <see cref="onIter"/> listeners will no longer be invoked.
         * Passing `false` will allow the timer to continue
         * normal execution.
         * </summary>
         * <param name="pause">Whether the timer should be paused</param>
         */
        public void PauseTimer(bool pause = true) {
            paused = pause;
        }

        /**
         * <summary>
         * Sets the timer to go in reverse of its current direction.
         * </summary>
         */
        public void ReverseTimer() {
            float oldStart = startTime;

            startTime = endTime;
            endTime = oldStart;
        }

        /**
         * <summary>
         * Stops the timer immediately.
         * </summary>
         */
        public void StopTimer() {
            running = false;
        }

        /**
         * <summary>
         * Runs the timer.
         * </summary>
         */
        private IEnumerator RunTimer() {
            timer = startTime;
            onStart.Invoke();

            while (running == true) {
                float prevTimer = timer;

                timer = Mathf.MoveTowards(
                    timer, endTime, timeScale * Time.deltaTime
                );

                if (timer == prevTimer) {
                    running = false;
                }

                onIter.Invoke(timer);
                yield return null;
            }

            onEnd.Invoke();
            coroutine = null;
            yield break;
        }
    }
}
