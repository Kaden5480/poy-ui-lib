using System.Collections.Generic;

using UnityEngine.Events;

using UILib.Behaviours;

namespace UILib.Animations {
    /**
     * <summary>
     * A group which manages easing in/out many
     * <see cref="BaseEase"/> behaviours.
     *
     * It only finishes once all of the attached ease behaviours
     * have reached an end point.
     * </summary>
     */
    public class EaseGroup : BaseTimer {
        // Ease behaviours to control
        private List<BaseEase> eases = new List<BaseEase>();

        /**
         * <summary>
         * Whether the group is currently easing in.
         * </summary>
         */
        public bool easingIn {
            get => running == true && increasing == true;
        }

        /**
         * <summary>
         * Whether the group is currently easing out.
         * </summary>
         */
        public bool easingOut {
            get => running == true && increasing == false;
        }

        /**
         * <summary>
         * Invokes listeners when easing in finishes.
         * </summary
         */
        public UnityEvent onEaseIn { get; } = new UnityEvent();

        /**
         * <summary>
         * Invokes listeners when easing out finishes.
         * </summary
         */
        public UnityEvent onEaseOut { get; } = new UnityEvent();

        /**
         * <summary>
         * Normally, an ease group must run infinitely.
         *
         * Since it relies on all added ease behaviours
         * reaching an end point.
         * </summary>
         */
        private void Awake() {
            SetInfinite(true);
        }

        /**
         * <summary>
         * Adds an ease behaviour to be controlled by this group.
         * </summary>
         * <param name="ease">The ease behaviour to add</param>
         */
        public void Add(BaseEase ease) {
            if (eases.Contains(ease) == true) {
                logger.LogDebug($"Ease already added: {ease}");
                return;
            }

            eases.Add(ease);
        }

        /**
         * <summary>
         * Forces all ease behaviours to be at their min/max
         * values.
         * This uses the current "increasing" mode to determine
         * where to end.
         * </summary>
         */
        private void ForceEase() {
            foreach (BaseEase ease in eases) {
                if (increasing == true) {
                    ease.EaseIn(true);
                }
                else {
                    ease.EaseOut(true);
                }
            }
        }

        /**
         * <summary>
         * Eases in all added ease behaviours.
         * </summary>
         * <param name="force">Whether to force easing in</param>
         */
        public void EaseIn(bool force = false) {
            SetIncreasing(true);

            // Force running needs to be handled in
            // a pretty specific way
            if (force == true) {
                ForceEase();
            }

            StartTimer();
        }

        /**
         * <summary>
         * Eases out all added ease behaviours.
         * </summary>
         * <param name="force">Whether to force easing out</param>
         */
        public void EaseOut(bool force = false) {
            SetIncreasing(false);

            // Force running needs to be handled in
            // a pretty specific way
            if (force == true) {
                ForceEase();
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

            // Whether all ease behaviours have finished, there's only
            // a need to care about this in "increasing" mode, otherwise
            // this group will continue ticking off to "infinity"
            bool finished = true;

            // Iterate over all behaviours, telling them
            // to perform an iteration
            foreach (BaseEase ease in eases) {
                // If the time extends beyond the duration
                // the ease is configured to run in, that's fine
                // because the value is clamped by BaseEase._SetValue
                ease.ForceOnIter(time);

                // The condition below assumes that the value
                // is clamped between minValue and maxValue, which it should
                // be if you refer to BaseEase._SetValue

                // If increasing, the ease must reach its max value
                if (increasing == true && ease.value != ease.maxValue) {
                    finished = false;
                }
            }

            // If in increasing mode and finished, stop manually
            if (increasing == true && finished == true) {
                StopTimer();
                OnEnd();
            }

            // Otherwise, the iterations are handled lower down by
            // the BaseTimer
            // This will cause this group to stop iterating
            // once it returns to `0`
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
                onEaseIn.Invoke();
            }
            else {
                onEaseOut.Invoke();
            }
        }
    }
}
