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
         * Adds an ease behaviour to be controlled by this group.
         * </summary>
         * <param name="ease">The ease behaviour to add</param>
         */
        public void Add(BaseEase ease) {
            if (ease == null) {
                logger.LogDebug("Not adding null ease");
                return;
            }

            if (eases.Contains(ease) == true) {
                logger.LogDebug($"Ease already added: {ease}");
                return;
            }

            eases.Add(ease);
        }

        /**
         * <summary>
         * Updates the total duration based upon the
         * currently added ease behaviours.
         * </summary>
         */
        private void UpdateDuration() {
            float maxDuration = 0f;

            foreach (BaseEase ease in eases) {
                if (ease.duration > maxDuration) {
                    maxDuration = ease.duration;
                }
            }

            SetDuration(maxDuration);
        }

        /**
         * <summary>
         * Eases in all added ease behaviours.
         * </summary>
         * <param name="force">Whether to force easing in</param>
         */
        public void EaseIn(bool force = false) {
            SetIncreasing(true);

            // Update the end time (duration)
            UpdateDuration();

            // Force running needs to be handled in
            // a pretty specific way
            if (force == true) {
                ForceRun();
                return;
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

            // Iterate over all behaviours, telling them
            // to perform an iteration
            foreach (BaseEase ease in eases) {
                // If the time extends beyond the duration
                // the ease is configured to run in, that's fine
                // because the value is clamped by BaseEase._SetValue
                ease.ForceOnIter(time);
            }
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
