using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UILib.Components;
using ClearMode = UILib.Components.TextField.ClearMode;
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
        internal static TextField current { get; private set; } = null;

        // Indicates to other UILib functionality whether a text field
        // was recently selected
        internal static bool isSelected { get; private set; } = false;

        // Coroutine for delaying deselect
        private static IEnumerator deselectRoutine = null;

        /**
         * <summary>
         * Handles stopping the deselect routine.
         * </summary>
         */
        private static void StopRoutine() {
            if (deselectRoutine != null) {
                Plugin.instance.StopCoroutine(deselectRoutine);
                deselectRoutine = null;
            }
        }

        /**
         * <summary>
         * Delays notifying about text fields being deselected.
         * </summary>
         */
        private static IEnumerator DeselectRoutine() {
            yield return null;

            isSelected = false;
            deselectRoutine = null;

            yield break;
        }

        /**
         * <summary>
         * Handles deselecting the current text field.
         * </summary>
         */
        internal static void Deselect() {
            current = null;

            StopRoutine();
            deselectRoutine = DeselectRoutine();
            Plugin.instance.StartCoroutine(deselectRoutine);
        }

        /**
         * <summary>
         * Handles selecting a text field.
         * </summary>
         * <param name="field">The text field which was selected</param>
         */
        internal static void Select(TextField field) {
            current = field;

            StopRoutine();
            isSelected = true;
        }

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
            Deselect();
            string input = saved.userInput;

            EventSystem.current.SetSelectedGameObject(null);

            // Submits
            if ((wasClick == true
                && saved.submitMode.HasFlag(SubmitMode.Click) == true)
             || (wasClick == false
                && saved.submitMode.HasFlag(SubmitMode.Escape) == true)
            ) {
                if (saved.Validate(input) == true) {
                    saved.onValidSubmit.Invoke(input);
                }
                else {
                    saved.onInvalidSubmit.Invoke(input);
                }

                return;
            }

            // Handle different cancel states
            // Clicking outside
            if (wasClick == true) {
                if (saved.clearMode.HasFlag(ClearMode.Click) == true) {
                    saved.SetValue("");
                }
                else if (saved.retainMode.HasFlag(RetainMode.Click) == true) {
                    saved.SetText(input);
                }
                else {
                    saved.SetValue(saved.value);
                }
            }
            // Escape
            else {
                if (saved.clearMode.HasFlag(ClearMode.Escape) == true) {
                    saved.SetValue("");
                }
                else if (saved.retainMode.HasFlag(RetainMode.Escape) == true) {
                    saved.SetText(input);
                }
                else {
                    saved.SetValue(saved.value);
                }
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
            Deselect();

            string userInput = saved.userInput;

            // Check the user input
            bool validated = saved.Validate(userInput);

            EventSystem.current.SetSelectedGameObject(null);

            if (saved.retainFocus == true) {
                EventSystem.current.SetSelectedGameObject(saved.gameObject);
            }

            if (validated == true) {
                saved.onValidSubmit.Invoke(userInput);
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
