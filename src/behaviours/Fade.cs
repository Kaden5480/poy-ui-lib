using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace UILib.Behaviours {
    /**
     * <summary>
     * A behaviour which uses a <see cref="Timer"/> internally
     * to manage fading in/out `CanvasGroups`.
     * </summary>
     */
    public class Fade : Timer {
        /**
         * <summary>
         * The currently set length of time fading should take.
         * </summary>
         */
        public float fadeTime { get; private set; } = 0f;

        /**
         * <summary>
         * The currently set minimum opacity.
         * </summary>
         */
        public float minOpacity { get; private set; } = 0f;

        /**
         * <summary>
         * The currently set maximum opacity.
         * </summary>
         */
        public float maxOpacity { get; private set; } = 1f;

        /**
         * <summary>
         * The opacity the canvas groups are currently at.
         * </summary>
         */
        public float opacity { get; private set; } = 1f;

        /**
         * <summary>
         * Whether this fade is currently fading in.
         * </summary>
         */
        public bool fadingIn { get; private set; } = false;

        /**
         * <summary>
         * Whether this fade is currently fading out.
         * </summary>
         */
        public bool fadingOut { get; private set; } = false;

        /**
         * <summary>
         * An event which invokes listeners once
         * fading in has completed entirely.
         * </summary>
         */
        public UnityEvent onFadeIn { get; } = new UnityEvent();

        /**
         * <summary>
         * An event which invokes listeners once
         * fading out has completed entirely.
         * </summary>
         */
        public UnityEvent onFadeOut { get; } = new UnityEvent();

        // Canvas groups to fade in/out
        private List<CanvasGroup> groups = new List<CanvasGroup>();

        /**
         * <summary>
         * Sets the opacity of all canvas groups.
         * </summary>
         * <param name="opacity">The opacity to set</param>
         */
        private void SetCurrentOpacity(float opacity) {
            this.opacity = opacity;
            foreach (CanvasGroup group in groups) {
                group.alpha = opacity;
            }
        }

        /**
         * <summary>
         * Sets up listeners to control opacity.
         * </summary>
         */
        private void Awake() {
            onIter.AddListener(SetCurrentOpacity);

            onEnd.AddListener(() => {
                if (fadingIn == true) {
                    onFadeIn.Invoke();
                }
                if (fadingOut == true) {
                    onFadeOut.Invoke();
                }

                fadingIn = false;
                fadingOut = false;
            });
        }

        /**
         * <summary>
         * Adds a `CanvasGroup` which should have
         * its `alpha` controlled by this behaviour.
         * </summary>
         */
        public void Add(CanvasGroup group) {
            groups.Add(group);
        }

        /**
         * <summary>
         * Recalculates the appropriate time scale.
         * </summary>
         */
        private void CalcTimeScale() {
            SetTimeScale((maxOpacity - minOpacity) / fadeTime);
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

            CalcTimeScale();
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
            CalcTimeScale();
        }

        /**
         * <summary>
         * Fades the `CanvasGroups` in.
         * </summary>
         * <param name="force">Whether to force fading in instantly</param>
         */
        public void FadeIn(bool force = false) {
            fadingIn = true;
            fadingOut = false;

            // Fade in from current opacity
            if (force == true) {
                StartTimer(maxOpacity, maxOpacity);
                onFadeIn.Invoke();
            }
            else {
                StartTimer(opacity, maxOpacity);
            }
        }

        /**
         * <summary>
         * Fades the `CanvasGroups` out.
         * </summary>
         * <param name="force">Whether to force fading out instantly</param>
         */
        public void FadeOut(bool force = false) {
            fadingIn = false;
            fadingOut = true;

            // Fade out from current opacity
            if (force == true) {
                StartTimer(minOpacity, minOpacity);
                onFadeOut.Invoke();
            }
            else {
                StartTimer(opacity, minOpacity);
            }
        }
    }
}
