using UnityEngine;
using UEImage = UnityEngine.UI.Image;

namespace UILib {
    internal class TopBar : UIObject {
        private Window window;

        private Vector2 latestPosition = Vector2.zero;

        internal TopBar(Window window, float height) {
            this.window = window;

            SetSize(0f, height);
            SetAnchor(AnchorType.TopLeft, FillType.FillHorizontal);

            UEImage image = gameObject.AddComponent<UEImage>();
            image.color = Colors.grey;

            DragHandler dragHandler = gameObject.AddComponent<DragHandler>();
            dragHandler.SetBeginListener(SetDragOffset);
            dragHandler.SetDragListener(DragWindow);
        }

        private void SetDragOffset(Vector2 position) {
            window.BringToFront();
            latestPosition = position;
        }

        private void DragWindow(Vector2 position) {
            window.MoveBy(position - latestPosition);
            latestPosition = position;
        }
    }
}
