using System;
using System.Collections.Generic;

namespace UILib.Behaviours {
    /**
     * <summary>
     * A class which manages multiple <see cref="Ease"/> behaviours
     * at once.
     * </summary>
     */
    public class EaseGroup : Ease {
        // The ease behaviours to control
        private List<Ease> eases = new List<Ease>();

        /**
         * <summary>
         * Initializes this ease group.
         * </summary>
         */
        private void Awake() {
            // Forward all events down
            onEaseIn.AddListener(() => {
                foreach (Ease ease in eases) {
                    ease.timer = timer;
                    ease.onEaseIn.Invoke();
                }
            });

            onEase.AddListener(delegate {
                foreach (Ease ease in eases) {
                    ease.timer = timer;

                    Func<float, float> easeFunction = null;
                    if (easingIn == true) {
                        easeFunction = ease.easeInFunction;
                    }
                    else if (easingOut == true) {
                        easeFunction = ease.easeOutFunction;
                    }

                    ease.onEase.Invoke(
                        ease.GetValue(timer, easeFunction)
                    );
                }
            });

            onEaseOut.AddListener(() => {
                foreach (Ease ease in eases) {
                    ease.timer = timer;
                    ease.onEaseOut.Invoke();
                }
            });
        }

        /**
         * <summary>
         * Adds an <see cref="Ease"/> behaviour to this group.
         * </summary>
         * <param name="ease">The `Ease` behaviour to add</param>
         */
        public void Add(Ease ease) {
            if (eases.Contains(ease) == true) {
                logger.LogDebug($"Not adding behaviour, {ease} has already been added");
                return;
            }

            eases.Add(ease);
        }
    }
}
