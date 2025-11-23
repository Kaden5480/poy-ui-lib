using UnityEngine;

namespace UILib {
    public class FixedWindow: Window {
        /**
         * <summary>
         * Initializes a FixedWindow.
         * </summary>
         * <param name="name">The name of the window</param>
         * <param name="width">The width of the window</param>
         * <param name="height">The height of the window</param>
         */
        public FixedWindow(string name, float width, float height) : base(name, width, height) {
            topBar.fullscreenButton.Destroy();
        }

        /**
         * <summary>
         * Prevent normal windowing functionality.
         * </summary>
         */
        protected override void OnClick() {}
        protected override void OnDoubleClick() {}
        protected override void OnBeginDrag(Vector2 position) {}
        protected override void OnDrag(Vector2 position) {}
        protected override void OnEndDrag(Vector2 position) {}

        internal override void AddResizeButton() {}
        internal override void HandleBeginDrag(Vector2 position) {}
        internal override void HandleMove(Vector2 position) {}
        internal override void HandleResize(Vector2 position) {}
    }
}
