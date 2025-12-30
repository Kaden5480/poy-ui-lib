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
                Plugin.LogDebug($"Easing to opacity: {opacity}");
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
         * Sets the minimum and maximum opacities
         * to fade between.
         *
         * Both `min` and `max` must be between 0-1 inclusive.
         * </summary>
         * <param name="min">The minimum opacity</param>
         * <param name="max">The maximum opacity</param>
         */
        public override void SetLimits(float min = 0f, float max = 1f) {
            min = Mathf.Clamp(min, 0f, 1f);
            max = Mathf.Clamp(max, 0f, 1f);
            base.SetLimits(min, max);
        }
    }
}
