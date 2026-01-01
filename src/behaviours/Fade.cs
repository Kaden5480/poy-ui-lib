using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace UILib.Behaviours {
    /**
     * <summary>
     * </summary>
     */
    public class Fade : BaseTimer {
        // The canvas groups to fade in/out
        private List<CanvasGroup> groups = new List<CanvasGroup>();

        /**
         * <summary>
         * The minimum opacity (0-1 inclusive).
         * </summary>
         */
        public float minOpacity { get; private set; } = 0f;

        /**
         * <summary>
         * The maximum opacity (0-1 inclusive).
         * </summary>
         */
        public float maxOpacity { get; private set; } = 1f;

        /**
         * <summary>
         * The current opacity the canvas groups are set to.
         * </summary>
         */
        public float opacity { get; private set; } = 1f;

        /**
         * <summary>
         * Whether this behaviour is currently fading in.
         * </summary>
         */
        public bool fadingIn {
            get => running == true && increasing == true;
        }

        /**
         * <summary>
         * Whether this behaviour is currently fading out.
         * </summary>
         */
        public bool fadingOut {
            get => running == true && increasing == false;
        }

        /**
         * <summary>
         * Invokes listeners once fading in finishes.
         * </summary>
         */
        public UnityEvent onFadeIn { get; } = new UnityEvent();

        /**
         * <summary>
         * Invokes listeners once fading out finishes.
         * </summary>
         */
        public UnityEvent onFadeOut { get; } = new UnityEvent();

        /**
         * <summary>
         * Adds a canvas group to be managed by this behaviour.
         *
         * Adding the group to this behaviour will also immediately
         * update its alpha value.
         * </summary>
         * <param name="group">The canvas group to add</param>
         */
        public void Add(CanvasGroup group) {
            if (groups.Contains(group) == true) {
                logger.LogDebug($"Group was already added: {group}");
                return;
            }

            groups.Add(group);
        }

        /**
         * <summary>
         * Sets the opacities to fade between.
         * These opacities must be between 0-1.
         * </summary>
         * <param name="minOpacity">The minimum opacity</param>
         * <param name="maxOpacity">The maximum opacity</param>
         */
        public void SetOpacities(float minOpacity, float maxOpacity) {
            this.minOpacity = Mathf.Clamp(minOpacity, 0f, 1f);
            this.maxOpacity = Mathf.Clamp(maxOpacity, 0f, 1f);
        }

        /**
         * <summary>
         * Updates the opacity of all canvas groups.
         * </summary>
         * <param name="opacity">The new opacity to use</param>
         */
        public void SetOpacity(float opacity) {
            this.opacity = Mathf.Clamp(opacity, 0f, 1f);
            foreach (CanvasGroup group in groups) {
                group.alpha = this.opacity;
            }
        }

        /**
         * <summary>
         * Fades the attached canvas groups in.
         * </summary>
         * <param name="force">Whether to force fading in</param>
         */
        public void FadeIn(bool force = false) {
            SetIncreasing(true);

            if (force == true) {
                // Stop to prevent the coroutine
                // potentially conflicting with this logic
                Stop();

                SetOpacity(maxOpacity);
                onFadeIn.Invoke();
                return;
            }

            Start();
        }

        /**
         * <summary>
         * Fades the attached canvas groups out.
         * </summary>
         * <param name="force">Whether to force fading out</param>
         */
        public void FadeOut(bool force = false) {
            SetIncreasing(false);

            if (force == true) {
                // Stop to prevent the coroutine
                // potentially conflicting with this logic
                Stop();

                SetOpacity(minOpacity);
                onFadeOut.Invoke();
                return;
            }

            Start();
        }

        /**
         * <summary>
         * Runs on each iteration of fading.
         * </summary>
         * <param name="time">The current value of the internal timer</param>
         */
        protected override void OnIter(float time) {
            base.OnIter(time);

            // If the duration is 0, this needs to be handled
            // in a very specific way
            if (duration <= 0f) {
                // Increasing means fading in to max opacity
                // Decreasing means fading out to min opacity
                SetOpacity((increasing == true) ? maxOpacity : minOpacity);
                return;
            }

            // Otherwise, the current time has to scale to be between
            // the minimum and maximum configured opacities

            // This first requires normalising the time
            float normal = time / duration;

            // Then, the normalised time has to be used to scale the opacity
            float opacity = minOpacity + (normal * Mathf.Abs(maxOpacity - minOpacity));
            SetOpacity(opacity);
        }

        /**
         * <summary>
         * Runs when this behaviour finishes fading in/out.
         * </summary>
         */
        protected override void OnEnd() {
            base.OnEnd();

            // Increasing means fading in
            if (increasing == true) {
                onFadeIn.Invoke();
            }
            else {
                onFadeOut.Invoke();
            }
        }
    }
}
