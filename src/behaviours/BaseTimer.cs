using System.Collections;

using UnityEngine;

namespace UILib.Behaviours {
    /**
     * <summary>
     * A reversible timer which always goes between 0 and 1.
     * </summary>
     */
    public class BaseTimer : MonoBehaviour {
        internal Logger logger = new Logger(typeof(BaseTimer));

        // The timer coroutine
        private IEnumerator coroutine = null;

        /**
         * <summary>
         * Whether the timer is currently running.
         * </summary>
         */
        public bool running {
            get => coroutine != null;
        }

        /**
         * <summary>
         * The current time this timer is at.
         * </summary>
         */
        public float time { get; private set; } = 0f;

        /**
         * <summary>
         * The total length of time this timer
         * will take to run.
         * </summary>
         */
        public float duration { get; private set; } = 1f;

        /**
         * <summary>
         * This timer's current time scale.
         *
         * This determines how quickly the timer runs
         * by scaling the time delta on each iteration by this value.
         *
         * A higher time scale means a faster timer,
         * a lower time scale means a slower timer.
         * </summary>
         */
        protected float timeScale { get; private set; } = 1f;

        /**
         * <summary>
         * Whether this timer is increasing or decreasing.
         *
         * Increasing means the timer is heading towards
         * the <see cref="duration"/>.
         * Decreasing means the timer is heading towards `0`.
         * </summary>
         */
        protected bool increasing { get; private set; } = true;

        /**
         * <summary>
         * Handles running the timer.
         * </summary>
         */
        private IEnumerator TimerRoutine() {
            for (;;) {
                // Calculate delta and decide end time
                float delta = Time.deltaTime * timeScale;
                float target = (increasing == true) ? duration : 0f;

                // Update the timer
                time = Mathf.MoveTowards(
                    time, target, delta
                );

                // Perform an iteration
                OnIter(time);

                // If the timer has reached the end time, stop
                if (time == target) {
                    break;
                }

                yield return null;
            }

            OnEnd();
            coroutine = null;
            yield break;
        }

        /**
         * <summary>
         * Forces the timer to reach an end point.
         *
         * If the timer is set to be <see cref="increasing"/>, the end point will
         * be the <see cref="duration"/>.
         * If the timer is set to be decreasing, the end point will be `0`.
         * </summary>
         */
        public virtual void ForceRun() {
            // Stop any coroutine that may be running
            Stop();

            // If increasing, set to end
            if (increasing == true) {
                SetToEnd();
            }
            // Otherwise, set to start
            else {
                SetToStart();
            }

            // Run one iteration, and end immediately
            OnIter(time);
            OnEnd();
        }

        /**
         * <summary>
         * Starts the timer.
         *
         * The timer will continue running from its current time.
         * If the timer routine is already running, this method does nothing.
         * </summary>
         */
        public virtual void Start() {
            if (coroutine != null) {
                return;
            }

            OnStart();

            // If the duration is 0, run one iteration
            // and stop immediately as there is no
            // reason to create a coroutine
            if (duration <= 0f) {
                OnIter(0f);
                OnEnd();
                return;
            }

            coroutine = TimerRoutine();
            StartCoroutine(coroutine);
        }

        /**
         * <summary>
         * Stops the timer immediately.
         *
         * This stops the coroutine, but keeps the <see cref="time"/>
         * at its current value.
         * </summary>
         */
        public virtual void Stop() {
            if (coroutine == null) {
                return;
            }

            StopCoroutine(coroutine);
            coroutine = null;
        }

        /**
         * <summary>
         * Sets the direction this timer should run in.
         * </summary>
         * <param name="increasing">Whether this timer should be increasing</param>
         */
        protected virtual void SetIncreasing(bool increasing) {
            this.increasing = increasing;
        }

        /**
         * <summary>
         * Tells the timer to go in the reverse of its
         * current direction.
         * </summary>
         */
        public virtual void Reverse() {
            increasing = !increasing;
        }

        /**
         * <summary>
         * Sets how long this timer should take to run.
         * </summary>
         * <param name="duration">The duration to set</param>
         */
        public virtual void SetDuration(float duration) {
            this.duration = duration;
        }

        /**
         * <summary>
         * Sets the timer's <see cref="timeScale"/>.
         *
         * This determines how quickly the timer will run.
         *
         * This value must be >= `0`.
         * </summary>
         * <param name="timeScale">The time scale to set</param>
         */
        public virtual void SetTimeScale(float timeScale) {
            if (timeScale < 0f) {
                logger.LogError($"Unexpected time scale: {timeScale}");
                return;
            }

            this.timeScale = timeScale;
        }

        /**
         * <summary>
         * Sets the <see cref="time"/> to a specific value.
         *
         * This must be between `0` and the configured <see cref="duration"/>
         * </summary>
         * <param name="time">The value to set the time to</param>
         */
        protected virtual void SetTime(float time) {
            this.time = Mathf.Clamp(time, 0f, duration);
        }

        /**
         * <summary>
         * Sets the time to `0`.
         * </summary>
         */
        public void SetToStart() {
            SetTime(0f);
        }

        /**
         * <summary>
         * Sets the time to the <see cref="duration"/>.
         * </summary>
         */
        public void SetToEnd() {
            SetTime(duration);
        }

        /**
         * <summary>
         * Runs just before the timer's coroutine
         * starts running.
         * </summary>
         */
        protected virtual void OnStart() {}

        /**
         * <summary>
         * Runs on each iteration of the timer.
         * </summary>
         * <param name="time">The current value of the timer</param>
         */
        protected virtual void OnIter(float time) {}

        /**
         * <summary>
         * Runs when the timer reaches an end point.
         * This would be <see cref="duration"/> if the timer is increasing,
         * and `0` if decreasing.
         * </summary>
         */
        protected virtual void OnEnd() {}
    }
}
