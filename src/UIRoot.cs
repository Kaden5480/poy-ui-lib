using System.Collections.Generic;

using UnityEngine;

namespace UILib {
    internal static class UIRoot {
        private const int minSortingOrder = 1000;

        private static GameObject gameObject;
        private static List<Window> windows;

        internal static void Init() {
            if (gameObject != null) {
                return;
            }

            gameObject = new GameObject("UILib Root");
            gameObject.layer = LayerMask.NameToLayer("UI");
            GameObject.DontDestroyOnLoad(gameObject);

            windows = new List<Window>();
        }

        /**
         * <summary>
         * Registers a window for sorting.
         * </summary>
         * <param name="window">The window to register</param>
         */
        internal static void Register(Window window) {
            window.canvasObj.transform.SetParent(
                gameObject.transform, false
            );
            window.canvas.sortingOrder = minSortingOrder + windows.Count;
            windows.Add(window);
        }

        /**
         * <summary>
         * Unregisters a window for sorting.
         * </summary>
         * <param name="window">The window to unregister</param>
         */
        internal static void Unregister(Window window) {
            BringToFront(window);
            windows.Remove(window);
        }

        /**
         * <summary>
         * Sets a window to be in front of all others.
         * </summary>
         * <param name="window">The window to bring to the front</param>
         */
        internal static void BringToFront(Window window) {
            // Try finding the window
            int index = windows.IndexOf(window);

            if (index < 0) {
                return;
            }

            // Iterate the list in reverse, decrementing all sorting orders
            // until reaching the canvas to set on top
            for (int i = windows.Count - 1; i > index; i--) {
                windows[i].canvas.sortingOrder--;
            }

            // Now remove the window from the list, and add it back
            // to the end, while also updating the sorting order
            windows.Remove(window);
            window.canvas.sortingOrder = minSortingOrder + windows.Count;
            windows.Add(window);
        }
    }
}
