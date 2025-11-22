using UnityEngine;
using UEImage = UnityEngine.UI.Image;

namespace UILib {
    internal class TopBar : UIObject {
        private Area buttonArea;

        internal Button fullscreenButton { get; private set; }
        internal Button closeButton      { get; private set; }

        private float height;

        internal Window window { get; private set; }

        internal TopBar(Window window, float height, int padding) {
            this.window = window;
            this.height = height;

            float totalHeight = height + 2*padding;

            SetSize(0f, totalHeight);
            SetAnchor(AnchorType.TopLeft, FillType.FillHorizontal);

            UEImage image = gameObject.AddComponent<UEImage>();
            image.color = Colors.grey;

            Label label = new Label(window.name, (int) totalHeight - 5);
            Add(label);
            label.SetSize(0f, totalHeight);
            label.SetAnchor(AnchorType.TopLeft, FillType.FillHorizontal);

            buttonArea = new Area();
            Add(buttonArea);
            buttonArea.SetLayout(LayoutType.Horizontal);
            buttonArea.SetAnchor(AnchorType.TopRight);
            buttonArea.SetLayoutPadding(padding, padding, padding, padding);
            buttonArea.SetLayoutSpacing(padding);

            fullscreenButton = new Button("+", (int) height);
            fullscreenButton.AddLayoutElement();
            fullscreenButton.SetSize(height, height);
            fullscreenButton.SetAnchor(AnchorType.TopRight);
            fullscreenButton.AddListener(() => {
                ChangeWindowingMode();
                Notifier.Notify("Change windowing mode");
            });

            closeButton = new Button("x", (int) height);
            closeButton.AddLayoutElement();
            closeButton.SetSize(height, height);
            closeButton.SetAnchor(AnchorType.TopRight);
            closeButton.SetColorBlock(Colors.redColorBlock);
            closeButton.AddListener(() => {
                window.Hide();
            });

            buttonArea.Add(fullscreenButton);
            buttonArea.Add(closeButton);
        }

        public void ChangeWindowingMode() {
            window.HandleWindowingChange();

            if (window.fullscreen == true) {
                fullscreenButton.label.text.text = "-";
            }
            else {
                fullscreenButton.label.text.text = "+";
            }
        }

        public override void OnBeginDrag(Vector2 position) {
            window.HandleBeginDrag(position);
        }

        public override void OnDrag(Vector2 position) {
            window.HandleDrag(position);
        }
    }
}
