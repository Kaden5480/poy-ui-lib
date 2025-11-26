using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using UILib.Components;

namespace UILib.Patches {
    /**
     * <summary>
     * Patches InputFields because they're a pain.
     * Allows deselecting them by clicking anywhere outside them.
     * </summary>
     */
    internal static class InputFieldFix {
        internal static TextField current = null;

        /**
         * <summary>
         * Runs each frame checking if the currently selected
         * <see cref="TextField"/>, if any, was deselected.
         * </summary>
         */
        internal static void Update() {
            // Do nothing until lmb
            if (Input.GetMouseButtonDown(0) == false) {
                return;
            }

            // Do nothing if no text field has been selected
            if (current == null) {
                return;
            }

            // Otherwise check what's beneath the cursor
            PointerEventData eventData = new PointerEventData(
                EventSystem.current
            ) {
                position = Input.mousePosition,
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            // If the current text field was reselected, do nothing
            foreach (RaycastResult result in results) {
                if (result.gameObject == current.gameObject) {
                    return;
                }
            }

            // Otherwise, only lose focus if the currently selected
            // object is the text field
            if (EventSystem.current.currentSelectedGameObject
                == current.gameObject
            ) {
                EventSystem.current.SetSelectedGameObject(null);
            }

            current = null;
        }
    }
}
