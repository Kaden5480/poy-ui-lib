using UnityEngine;

namespace UILib {
    /**
     * <summary>
     * A class which holds a state for a Window.
     * </summary>
     */
    internal class WindowState {
        // The Window this state is for
        private Window window;

        // The state which is saved
        private float width;
        private float height;

        private Vector2 anchorMin;
        private Vector2 anchorMax;
        private Vector2 pivot;
        private Vector2 position;

        /**
         * <summary>
         * Save the current state of the Window.
         * </summary>
         * <param name="window">The Window to save the state for</param>
         */
        internal WindowState(Window window) {
            this.window = window;

            RectTransform rect = window.rectTransform;

            width = rect.sizeDelta.x;
            height = rect.sizeDelta.y;

            anchorMin = rect.anchorMin;
            anchorMax = rect.anchorMax;
            pivot = rect.pivot;
            position = rect.localPosition;
        }

        /**
         * <summary>
         * Restores the Window to the state saved.
         * </summary>
         */
        internal void Restore() {
            RectTransform rect = window.rectTransform;

            rect.sizeDelta = new Vector2(width, height);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.localPosition = position;
        }
    }
}
