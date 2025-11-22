using UnityEngine;

namespace UILib {
    public class Window : FixedWindow {
        private Vector2 latestPosition;

        /**
         * <summary>
         * Initializes a Window.
         * </summary>
         * <param name="name">The name of the window</param>
         * <param name="width">The width of the window</param>
         * <param name="height">The height of the window</param>
         */
        public Window(string name, float width, float height) : base(name, width, height) {}

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
