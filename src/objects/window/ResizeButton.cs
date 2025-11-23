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

        public override void OnBeginDrag(Vector2 position) {
            if (window.fullscreen == true
                || Input.GetMouseButton(0) == false
            ) {
                return;
            }

            window.HandleBeginDrag(position);
        }

        public override void OnDrag(Vector2 position) {
            if (window.fullscreen == true
                || Input.GetMouseButton(0) == false
            ) {
                return;
            }

            window.HandleResize(position);
        }
    }
}
