using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace UILib.Behaviours {
    /**
     * <summary>
     * A class which manages multiple <see cref="Ease"/> behaviours
     * at once.
     * </summary>
     */
    public class EaseGroup : MonoBehaviour {
        private Logger logger = new Logger(typeof(EaseGroup));

        // The ease behaviours to control
        private List<Ease> eases = new List<Ease>();

        // The coroutine
        private IEnumerator coroutine = null;

        /**
         * <summary>
         * Whether this behaviour is currently easing in.
         * </summary>
         */
        public bool easingIn { get; private set; } = false;

        /**
         * <summary>
         * Whether this behaviour is currently easing out.
         * </summary>
         */
        public bool easingOut { get; private set; } = false;

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
        }

        /**
         * <summary>
         * Handles easing in all the ease behaviours in a coroutine.
         * </summary>
         */
        private IEnumerator EaseInRoutine() {
            bool finished = false;

            // While there are things left to do,
            // continue iterating
            while (finished == false) {
                // Make each ease iterate by a time delta
                // with respect to their ease-in time
                foreach (Ease ease in eases) {
                    ease.Iterate(
                        Time.deltaTime,
                        ease.easeInTime,
                        ease.easeInFunction
                    );
                }

                // Check if all eases are finished
                bool allDone = true;
                foreach (Ease ease in eases) {
                    if (ease.timer < 1f) {
                        allDone = false;
                        break;
                    }
                }

                finished = allDone;
                yield return null;
            }

            // Once finished, invoke onEaseIn
            easingIn = false;
            coroutine = null;
            onEaseIn.Invoke();
            yield break;
        }

        /**
         * <summary>
         * Handles easing out all the ease behaviours in a coroutine.
         * </summary>
         */
        private IEnumerator EaseOutRoutine() {
            bool finished = false;

            // While there are things left to do,
            // continue iterating
            while (finished == false) {
                // Make each ease iterate by a time delta
                // with respect to their ease-out time
                foreach (Ease ease in eases) {
                    ease.Iterate(
                        -Time.deltaTime,
                        ease.easeOutTime,
                        ease.easeOutFunction
                    );
                }

                // Check if all eases are finished
                bool allDone = true;
                foreach (Ease ease in eases) {
                    if (ease.timer > 0f) {
                        allDone = false;
                        break;
                    }
                }

                finished = allDone;
                yield return null;
            }

            // Once finished, invoke onEaseOut
            easingOut = false;
            coroutine = null;
            onEaseOut.Invoke();
            yield break;
        }

        /**
         * <summary>
         * Stops the current ease coroutine.
         * </summary>
         */
        public void Stop() {
            if (coroutine != null) {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            easingIn = false;
            easingOut = false;
        }

        /**
         * <summary>
         * Handles easing in all the ease behaviours.
         * </summary>
         * <param name="force">Whether to force easing in</param>
         */
        public void EaseIn(bool force = false) {
            Stop();

            if (force == true) {
                foreach (Ease ease in eases) {
                    ease.EaseIn(true);
                }

                onEaseIn.Invoke();
                return;
            }

            easingIn = true;
            coroutine = EaseInRoutine();
            StartCoroutine(coroutine);
        }

        /**
         * <summary>
         * Handles easing out all the ease behaviours.
         * </summary>
         * <param name="force">Whether to force easing out</param>
         */
        public void EaseOut(bool force = false) {
            Stop();

            if (force == true) {
                foreach (Ease ease in eases) {
                    ease.EaseOut(true);
                }

                onEaseOut.Invoke();
                return;
            }

            easingOut = true;
            coroutine = EaseOutRoutine();
            StartCoroutine(coroutine);
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
