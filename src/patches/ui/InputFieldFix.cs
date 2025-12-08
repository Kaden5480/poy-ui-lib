using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UILib.Components;

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
            HandleCancel();
        }

        /**
         * <summary>
         * Handles cancelling the input.
         * </summary>
         */
        private static void HandleCancel() {
            TextField saved = current;
            current = null;
            EventSystem.current.SetSelectedGameObject(null);

            if (saved.retainInput == true) {
                saved.SetText(saved.userInput);
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
            bool validated = saved.Validate(userInput);

            // Save the current user input
            if (validated == true) {
                saved.SetValue(userInput);
            }
            else if (saved.retainInput == true) {
                saved.SetText(userInput);
            }
            else {
                saved.SetText(saved.value);
            }

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
                HandleCancel();
            }

            // Submitted with return
            if (Input.GetKeyDown(KeyCode.Return) == true) {
                HandleSubmit();
            }
        }
    }
}
