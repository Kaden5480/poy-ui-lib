using UnityEngine;

namespace UILib {
    internal class Notification : UIObject {
        private Background background;
        private Label label;

        private FadeDestroy fadeDestroy;

        internal Notification(string text) {
            background = new Background(Colors.black);
            Add(background);

            label = new Label(text, 30);
            label.Fill();
            Add(label);

            AddLayoutElement();
            SetSize(NotificationArea.size, 100f);

            fadeDestroy = gameObject.AddComponent<FadeDestroy>();
        }

        /**
         * <summary>
         * Handles being clicked.
         * </summary>
         */
        public override void OnClick() {
            fadeDestroy.StartFade(this);
        }

        /**
         * <summary>
         * Sets the opacity of this notification.
         * </summary>
         * <param name="opacity">The opacity to set</param>
         */
        internal void SetOpacity(float opacity) {
            Color backgroundColor = background.image.color;
            Color labelColor = label.text.color;

            backgroundColor.a = opacity;
            labelColor.a = opacity;

            background.image.color = backgroundColor;
            label.text.color = labelColor;
        }
    }
}
