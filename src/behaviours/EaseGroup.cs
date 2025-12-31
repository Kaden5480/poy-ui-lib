using System;
using System.Collections.Generic;

namespace UILib.Behaviours {
    /**
     * <summary>
     * A class which manages multiple <see cref="Ease"/> behaviours
     * at once.
     * </summary>
     */
    public class EaseGroup {
        // The ease behaviours to control
        private List<Ease> eases = new List<Ease>();

        /**
         * <summary>
         * Invokes listeners once all <see cref="Ease"/> behaviours
         * have finished easing in.
         * </summary>
         */
        public UnityEvent onEaseIn { get; private set; } = new UnityEvent();

        /**
         * <summary>
         * Invokes listeners once all <see cref="Ease"/> behaviours
         * have finished easing out.
         * </summary>
         */
        public UnityEvent onEaseOut { get; private set; } = new UnityEvent();

        /**
         * <summary>
         * Initializes this ease group.
         * </summary>
         */
        private void Awake() {
            // Forward ease in and out
            onEaseIn.AddListener(() => {
                foreach (Ease ease in eases) {
                    ease.onEaseIn.Invoke();
                }
            });

            onEaseOut.AddListener(() => {
                foreach (Ease ease in eases) {
                    ease.onEaseOut.Invoke();
                }
            });

            onEase.AddListener(delegate {
                foreach (Ease ease in eases) {
                    ease.timer = timer;

                    Func<float, float> easeFunction = null;
                    if (easingIn == true) {
                        easeFunction = (ease.easeInFunction == null)
                            ? this.easeInFunction
                            : ease.easeInFunction;
                    }
                    else if (easingOut == true) {
                        easeFunction = (ease.easeOutFunction == null)
                            ? this.easeOutFunction
                            : ease.easeOutFunction;
                    }

                    ease.onEase.Invoke(
                        ease.GetValue(timer, easeFunction)
                    );
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
