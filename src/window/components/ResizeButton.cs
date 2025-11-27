using UnityEngine;

using UILib.Layouts;
using UIButton = UILib.Components.Button;

namespace UILib {
    /**
     * <summary>
     * A button which is added to the bottom right
     * of Windows to allow resizing them.
     * </summary>
     */
    internal class ResizeButton : UIButton {
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

            image.image.type = UnityEngine.UI.Image.Type.Tiled;
            image.image.color = theme.selectHighlight;
            image.SetSize(-6f, -6f);

            SetTheme(theme);
        }

        /**
         * <summary>
         * Allows setting the theme of this resize button
         * and all children.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        public override void SetTheme(Theme theme) {
            base.SetTheme(theme);

            button.colors = theme.blockSelectDark;

            if (image != null) {
                image.image.color = theme.selectHighlight;
            }
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
