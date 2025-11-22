using UnityEngine;

namespace UILib {
    internal class WindowState {
        private float width;
        private float height;

        private Vector2 anchorMin;
        private Vector2 anchorMax;
        private Vector2 pivot;
        private Vector2 position;

        internal WindowState(Window window) {
            RectTransform rect = window.rectTransform;

            width = rect.sizeDelta.x;
            height = rect.sizeDelta.y;

            anchorMin = rect.anchorMin;
            anchorMax = rect.anchorMax;
            pivot = rect.pivot;
            position = rect.localPosition;
        }

        internal void Restore(Window window) {
            RectTransform rect = window.rectTransform;

            rect.sizeDelta = new Vector2(width, height);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.localPosition = position;
        }
    }
}
