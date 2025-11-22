using UnityEngine;
using UEImage = UnityEngine.UI.Image;

namespace UILib {
    internal class TopBar : UIObject {
        private Window window;

        internal TopBar(Window window, float height) {
            this.window = window;

            SetSize(0f, height);
            SetAnchor(AnchorType.TopLeft, FillType.FillHorizontal);

            UEImage image = gameObject.AddComponent<UEImage>();
            image.color = Colors.grey;

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

        public override void OnBeginDrag(Vector2 position) {
            window.HandleBeginDrag(position);
        }

        public override void OnDrag(Vector2 position) {
            window.HandleDrag(position);
        }
    }
}
