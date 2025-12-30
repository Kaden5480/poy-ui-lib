using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace UILib.Behaviours {
    /**
     * <summary>
     * A behaviour which uses an <see cref="Ease"/> behaviour internally
     * to manage fading in/out `CanvasGroups`.
     * </summary>
     */
    public class Fade : Ease {
        // Canvas groups to fade in/out
        private List<CanvasGroup> groups = new List<CanvasGroup>();

        /**
         * <summary>
         * Initializes this fade behaviour.
         * </summary>
         */
        private void Awake() {
            onEase.AddListener((float opacity) => {
                UpdateOpacity(opacity);
            });
        }

        /**
         * <summary>
         * Sets the opacity of all canvas groups.
         * </summary>
         * <param name="opacity">The opacity to set</param>
         */
        private void UpdateOpacity(float opacity) {
            foreach (CanvasGroup group in groups) {
                group.alpha = opacity;
            }
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
         * Sets the opacities to fade between.
         *
         * Both `fadeInOpacity` and `fadeOutOpacity` must be between 0-1 inclusive.
         * </summary>
         * <param name="fadeInOpacity">The opacity to fade in to</param>
         * <param name="fadeOutOpacity">The opacity to fade out to</param>
         */
        public override void SetLimits(float fadeInOpacity = 1f, float fadeOutOpacity = 0f) {
            fadeInOpacity = Mathf.Clamp(fadeInOpacity, 0f, 1f);
            fadeOutOpacity = Mathf.Clamp(fadeOutOpacity, 0f, 1f);
            base.SetLimits(fadeInOpacity, fadeOutOpacity);
        }
    }
}
