using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UILib.Components;
using RetainMode = UILib.Components.TextField.RetainMode;
using SubmitMode = UILib.Components.TextField.SubmitMode;

namespace UILib.Patches.UI {
    /**
     * <summary>
     * A patch which deals with input fields
     * being awful.
     * </summary>
     */
    internal static class InputFieldFix {
        // The currently selected TextField
        internal static TextField current = null;

        /**
         * <summary>
         * Handles clicking outside the current field.
         * </summary>
         */
        private static void HandleClickOutside() {
            // If another object has since been selected,
            // don't do anything
            if (EventSystem.current.currentSelectedGameObject
                != current.gameObject
            ) {
                return;
            }

            // Otherwise, cancel here
            HandleCancel(true);
        }

        /**
         * <summary>
         * Handles cancelling the input.
         * </summary>
         * <param name="wasClick">Whether a click cancelled</param>
         */
        private static void HandleCancel(bool wasClick) {
            TextField saved = current;
            current = null;
            string input = saved.userInput;

            EventSystem.current.SetSelectedGameObject(null);

            // Submits
            if ((wasClick == true
                && saved.submitMode.HasFlag(SubmitMode.Click) == true)
             || (wasClick == false
                && saved.submitMode.HasFlag(SubmitMode.Escape) == true)
            ) {
                if (saved.Validate() == true) {
                    saved.onValidSubmit.Invoke(saved.value);
                }
                else {
                    saved.onInvalidSubmit.Invoke(input);
                }

                return;
            }

            // Actual cancels
            if ((wasClick == true
                && saved.retainMode.HasFlag(RetainMode.CancelClick) == true)
             || (wasClick == false
                && saved.retainMode.HasFlag(RetainMode.CancelEscape) == true)
            ) {
                saved.SetText(input);
            }
            else {
                saved.SetText(saved.value);
            }

            saved.onCancel.Invoke();
        }

        /**
         * <summary>
         * Handles submitting the input.
         * </summary>
         */
        private static void HandleSubmit() {
            TextField saved = current;
            current = null;

            string userInput = saved.userInput;

            // Check the user input
            bool validated = saved.Validate();

            EventSystem.current.SetSelectedGameObject(null);

            if (saved.retainFocus == true) {
                EventSystem.current.SetSelectedGameObject(saved.gameObject);
            }

            if (validated == true) {
                saved.onValidSubmit.Invoke(saved.value);
            }
            else {
                saved.onInvalidSubmit.Invoke(userInput);
            }
        }

        /**
         * <summary>
         * Checks the type of event which happened
         * on the currently selected input field.
         * </summary>
         */
        internal static void Update() {
            if (current == null) {
                return;
            }

            // Clicked outside
            if (Input.GetMouseButtonDown(0) == true
                && current.isPointerInside == false
            ) {
                HandleClickOutside();
            }

            // Cancelled with escape
            if (Input.GetKeyDown(KeyCode.Escape) == true) {
                HandleCancel(false);
            }

            // Submitted with return
            if (Input.GetKeyDown(KeyCode.Return) == true) {
                HandleSubmit();
            }
        }
    }
}
