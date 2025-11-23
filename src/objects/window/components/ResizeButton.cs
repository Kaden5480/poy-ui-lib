using UnityEngine;

namespace UILib {
    internal class ResizeButton : Button {
        private Window window;

        internal ResizeButton(Window window) : base(Resources.triangle) {
            this.window = window;
            SetAnchor(AnchorType.BottomRight);
            SetSize(20f, 20f);
            SetColorBlock(Colors.darkColorBlock);

            image.image.type = UnityEngine.UI.Image.Type.Tiled;
            image.image.color = Colors.lightGrey;
            image.SetSize(-6f, -6f);
        }

        protected override void OnBeginDrag(Vector2 position) {
            // Only resize when in windowed mode and holding lmb
            if (window.fullscreen == true
                || Input.GetMouseButton(0) == false
            ) {
                return;
            }

            window.HandleBeginDrag(position);
        }

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
