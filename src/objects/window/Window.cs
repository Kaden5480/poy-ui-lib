using UnityEngine;

using UEText = UnityEngine.UI.Text;

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

    public class Window : FixedWindow {
        private Vector2 latestPosition;

        private bool fullscreen = false;
        private WindowState state;

        /**
         * <summary>
         * Initializes a Window.
         * </summary>
         * <param name="name">The name of the window</param>
         * <param name="width">The width of the window</param>
         * <param name="height">The height of the window</param>
         */
        public Window(string name, float width, float height) : base(name, width, height, true) {}

        /**
         * <summary>
         * Handles changing windowing mode.
         * </summary>
         */
        public void HandleWindowingChange() {
            UEText text = topBar.fullscreenButton.label.text;

            if (fullscreen == false) {
                state = new WindowState(this);
                SetAnchor(AnchorType.Middle, FillType.Fill);
                rectTransform.anchoredPosition = Vector2.zero;
                text.text = "-";
                fullscreen = true;
            }
            else {
                state.Restore(this);
                state = null;
                text.text = "+";
                fullscreen = false;
            }
        }

        /**
         * <summary>
         * Bring this Window's canvas to the front
         * </summary>
         */
        public override void OnClick() {
            UIRoot.BringToFront(this);
        }

        /**
         * <summary>
         * Handles this Window being dragged from anywhere (not just the top bar).
         * </summary>
         * <param name="position">The position the drag started at</param>
         */
        public override void OnBeginDrag(Vector2 position) {
            if (Input.GetKey(KeyCode.LeftAlt) == false) {
                UIRoot.BringToFront(this);
                return;
            }

            HandleBeginDrag(position);
        }

        /**
         * <summary>
         * Handles this Window being dragged from anywhere (not just the top bar).
         * </summary>
         * <param name="position">The position dragged to</param>
         */
        public override void OnDrag(Vector2 position) {
            if (Input.GetKey(KeyCode.LeftAlt) == false) {
                return;
            }

            HandleDrag(position);
        }


        /**
         * <summary>
         * Handles starting to drag this window.
         * </summary>
         * <param name="position">The position dragging started at</param>
         */
        internal void HandleBeginDrag(Vector2 position) {
            UIRoot.BringToFront(this);
            latestPosition = position;
        }

        /**
         * <summary>
         * Handles dragging this window.
         * </summary>
         * <param name="position">The position dragged to</param>
         */
        internal void HandleDrag(Vector2 position) {
            MoveBy(position - latestPosition);
            latestPosition = position;
        }

        /**
         * <summary>
         * Move this window by a given delta.
         * </summary>
         */
        public void MoveBy(Vector3 delta) {
            rectTransform.localPosition += delta;
        }
    }
}
