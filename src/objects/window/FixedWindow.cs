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
        public override void AddResizeButton() {}
        public override void OnClick() {}
        public override void OnDoubleClick() {}
        public override void OnBeginDrag(Vector2 position) {}
        public override void OnDrag(Vector2 position) {}
        public override void OnEndDrag(Vector2 position) {}

        internal override void HandleBeginDrag(Vector2 position) {}
        internal override void HandleMove(Vector2 position) {}
        internal override void HandleResize(Vector2 position) {}
    }
}
