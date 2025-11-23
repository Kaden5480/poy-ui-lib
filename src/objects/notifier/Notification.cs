using UnityEngine;

namespace UILib {
    /**
     * <summary>
     * A Notification.
     *
     * This holds a background and label
     * for displaying some text and will
     * fade and then destroy itself after
     * a certain period of time.
     * </summary>
     */
    internal class Notification : UIObject {
        private Image background;
        private Label label;

        private FadeDestroy fadeDestroy;

        /**
         * <summary>
         * Initializes a Notification.
         * </summary>
         * <param name="text">The text to display</param>
         */
        internal Notification(string text) {
            background = new Image(Colors.black);
            Add(background);

            label = new Label(text, 30);
            label.Fill();
            Add(label);

            AddLayoutElement();
            SetSize(NotificationArea.size, 100f);

            // Start fading
            fadeDestroy = gameObject.AddComponent<FadeDestroy>();
            fadeDestroy.StartFade(this);
        }

        /**
         * <summary>
         * Immediately destroy when clicked.
         * </summary>
         */
        protected override void OnClick() {
            fadeDestroy.StopAllCoroutines();
            Destroy();
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
