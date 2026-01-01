using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace UILib.Animations {
    /**
     * <summary>
     * A <see cref="BaseEase"/> behaviour which fades between two
     * opacities.
     * </summary>
     */
    public class Fade : BaseEase {
        // The canvas groups to fade in/out
        private List<CanvasGroup> groups = new List<CanvasGroup>();

        /**
         * <summary>
         * The minimum opacity (0-1 inclusive).
         * </summary>
         */
        public float minOpacity { get => _minValue; }

        /**
         * <summary>
         * The maximum opacity (0-1 inclusive).
         * </summary>
         */
        public float maxOpacity { get => _maxValue; }

        /**
         * <summary>
         * The current opacity the canvas groups are set to.
         * </summary>
         */
        public float opacity { get => _value; }

        /**
         * <summary>
         * Whether this behaviour is currently fading in.
         * </summary>
         */
        public bool fadingIn { get => _easingIn; }

        /**
         * <summary>
         * Whether this behaviour is currently fading out.
         * </summary>
         */
        public bool fadingOut { get => _easingOut; }

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
            minOpacity = Mathf.Clamp(minOpacity, 0f, 1f);
            maxOpacity = Mathf.Clamp(maxOpacity, 0f, 1f);
            _SetValues(minOpacity, maxOpacity);
        }

        /**
         * <summary>
         * Internally updates canvas opacity on each iteration.
         * </summary>
         * <param name="opacity">The opacity to set</param>
         */
        protected override void _SetValue(float opacity) {
            opacity = Mathf.Clamp(opacity, 0f, 1f);
            base._SetValue(opacity);

            foreach (CanvasGroup group in groups) {
                group.alpha = opacity;
            }
        }

        /**
         * <summary>
         * Updates the opacity of all canvas groups.
         * </summary>
         * <param name="opacity">The new opacity to use</param>
         */
        public void SetOpacity(float opacity) {
            _SetValue(opacity);
        }

        /**
         * <summary>
         * Fades the attached canvas groups in.
         * </summary>
         * <param name="force">Whether to force fading in</param>
         */
        public void FadeIn(bool force = false) { _EaseIn(force); }

        /**
         * <summary>
         * Fades the attached canvas groups out.
         * </summary>
         * <param name="force">Whether to force fading out</param>
         */
        public void FadeOut(bool force = false) { _EaseOut(force); }

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
