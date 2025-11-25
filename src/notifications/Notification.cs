using UnityEngine;

using UILib.Behaviours;
using UILib.Components;
using UILib.Layout;

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
    internal class Notification : UIComponent {
        private const float waitTime = 3f;
        private const float fadeTime = 1f;

        private Image background;
        private Label label;

        private CanvasGroup canvasGroup;
        private Timer timer;

        // TODO: Add titles to notifications.

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
            label.SetFill(FillType.All);
            Add(label);

            SetSize(NotificationArea.size, 100f);

            canvasGroup = gameObject.AddComponent<CanvasGroup>();

            timer = gameObject.AddComponent<Timer>();

            timer.onIter.AddListener((float value) => {
                // Do nothing if in wait time
                if (value > fadeTime) {
                    return;
                }

                // Scale opacity
                canvasGroup.alpha = 1 - ((fadeTime - value) / fadeTime);
            });

            timer.onEnd.AddListener(() => {
                Destroy();
            });

            timer.StartTimer(waitTime + fadeTime);
        }

        /**
         * <summary>
         * Immediately destroy when clicked.
         * </summary>
         */
        protected override void OnClick() {
            timer.StopAllCoroutines();
            Destroy();
        }
    }
}
