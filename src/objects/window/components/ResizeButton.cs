using UnityEngine;

namespace UILib {
    /**
     * <summary>
     * A button which is added to the bottom right
     * of Windows to allow resizing them.
     * </summary>
     */
    internal class ResizeButton : Button {
        // The Window this ResizeButton is attached to
        private Window window;

        /**
         * <summary>
         * Initializes a ResizeButton.
         * </summary>
         * <param name="window">The window this ResizeButton is attached to</param>
         */
        internal ResizeButton(Window window) : base(Resources.triangle) {
            this.window = window;
            SetAnchor(AnchorType.BottomRight);
            SetSize(20f, 20f);
            SetColorBlock(Colors.darkColorBlock);

            image.image.type = UnityEngine.UI.Image.Type.Tiled;
            image.image.color = Colors.lightGrey;
            image.SetSize(-6f, -6f);
        }

        /**
         * <summary>
         * Handles resizing the window when dragged.
         * </summary>
         */
        protected override void OnBeginDrag(Vector2 position) {
            // Only resize when in windowed mode and holding lmb
            if (window.fullscreen == true
                || Input.GetMouseButton(0) == false
            ) {
                return;
            }

            window.HandleBeginDrag(position);
        }

        /**
         * <summary>
         * Handles resizing the window when dragged.
         * </summary>
         */
        protected override void OnDrag(Vector2 position) {
            // Only resize when in windowed mode and holding lmb
            if (window.fullscreen == true
                || Input.GetMouseButton(0) == false
            ) {
                return;
            }

            window.HandleResize(position);
        }
    }
}
