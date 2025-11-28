using System.Collections.Generic;

using UnityEngine;

namespace UILib.Behaviours {
    /**
     * <summary>
     * A behaviour which uses a <see cref="Timer"/> internally
     * to manage fading in/out `CanvasGroups`.
     * </summary>
     */
    public class Fade : Timer {
        // How long fading should take
        private float fadeTime = 0f;

        // The minimum and maximum opacities
        private float minOpacity = 0f;
        private float maxOpacity = 1f;

        private List<CanvasGroup> groups = new List<CanvasGroup>();

        /**
         * <summary>
         * Sets up listeners to control opacity.
         * </summary>
         */
        private void Awake() {
            onIter.AddListener((float value) => {
                // The proportion of the fade time
                // which has passed (0-1)
                float t = value / fadeTime;

                // The total fade delta
                float delta = maxOpacity - minOpacity;

                // The actual opacity to set
                float opacity = minOpacity + delta*t;

                foreach (CanvasGroup group in groups) {
                    group.alpha = opacity;
                }
            });
        }

        /**
         * <summary>
         * Add a `CanvasGroup` which should have
         * its `alpha` controlled by this behaviour.
         * </summary>
         */
        public void Add(CanvasGroup group) {
            groups.Add(group);
        }

        /**
         * <summary>
         * Sets the minimum and maximum opacities
         * to fade between.
         *
         * Both `min` and `max` must be between 0-1 inclusive.
         *
         * If you enter a `min` > `max` they
         * will be swapped around.
         * </summary>
         * <param name="min">The minimum opacity</param>
         * <param name="max">The maximum opacity</param>
         */
        public void SetOpacities(float min = 0f, float max = 1f) {
            min = Mathf.Clamp(min, 0f, 1f);
            max = Mathf.Clamp(max, 0f, 1f);

            if (min <= max) {
                this.minOpacity = min;
                this.maxOpacity = max;
            }
            else {
                // Swap the order
                this.minOpacity = max;
                this.maxOpacity = min;
            }

        }

        /**
         * <summary>
         * Sets how long it should take to fade in/out the
         * `alpha` of the `CanvasGroups`.
         * </summary>
         * <param name="time">The length of time the fade should last</param>
         */
        public void SetFadeTime(float time) {
            this.fadeTime = time;
        }

        /**
         * <summary>
         * Fade the `CanvasGroups` in.
         * </summary>
         */
        public void FadeIn() {
            // If the timer is already running, fade in
            // from its current value
            if (running == true) {
                StartTimer(timer, fadeTime);
            }
            // Otherwise, fade in from 0
            else {
                StartTimer(0f, fadeTime);
            }
        }

        /**
         * <summary>
         * Fade the `CanvasGroups` out.
         * </summary>
         */
        public void FadeOut() {
            // If the timer is already running, fade out
            // from its current value
            if (running == true) {
                StartTimer(timer, 0f);
            }
            // Otherwise, fade out from the max
            else {
                StartTimer(fadeTime, 0f);
            }
        }
    }
}
