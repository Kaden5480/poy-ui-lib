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

            Label label = new Label(window.name, (int) height - 5);
            Add(label);
            label.SetSize(0f, height);
            label.SetAnchor(AnchorType.TopLeft, FillType.FillHorizontal);

            Button button = new Button("Close", (int) height - 5);
            Add(button);
            button.SetSize(50f, height);
            button.SetAnchor(AnchorType.TopRight);
            button.SetColorBlock(Colors.redColorBlock);
            button.AddListener(() => {
                window.Hide();
            });
        }

        private void SetDragOffset(Vector2 position) {
            UIRoot.BringToFront(window);
            latestPosition = position;
        }

        private void DragWindow(Vector2 position) {
            window.MoveBy(position - latestPosition);
            latestPosition = position;
        }
    }
}
