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
        private float fadeTime = 0f;
        private List<CanvasGroup> groups = new List<CanvasGroup>();

        /**
         * <summary>
         * Sets up listeners to control opacity.
         * </summary>
         */
        private void Awake() {
            onIter.AddListener((float value) => {
                foreach (CanvasGroup group in groups) {
                    group.alpha = value / fadeTime;
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
