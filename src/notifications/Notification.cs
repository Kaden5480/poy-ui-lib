using UnityEngine;

using UILib.Behaviours;
using UILib.Components;
using UILib.Layout;

namespace UILib.Notifications {
    /**
     * <summary>
     * A Notification.
     *
     * The component which is displayed to the user
     * when a notification is sent.
     * These pop up in the bottom right of the screen.
     * </summary>
     */
    internal class Notification : UIComponent {
        private const float waitTime = 3f;
        private const float fadeTime = 1f;

        // Maximum title and message lengths
        private const int maxTitle = 48;
        private const int maxMessage = 256;

        private Image background;

        private Image titleBackground;
        private Label titleLabel;

        private Label messageLabel;

        private CanvasGroup canvasGroup;
        private Timer timer;

        /**
         * <summary>
         * Initializes a Notification.
         * </summary>
         * <param name="title">The title of the notification</param>
         * <param name="message">The message to display</param>
         * <param name="type">The type of this notification</param>
         */
        internal Notification(
            string title,
            string message,
            NotificationType type
        ) {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

            // If anything is too large, log it
            if (title.Length > maxTitle || message.Length > maxMessage) {
                logger.LogInfo("Got a large notification");
                logger.LogInfo($"Title: {title}");
                logger.LogInfo($"Message: {message}");
            }

            // Fix sizes
            title = ClampString(title, maxTitle);
            message = ClampString(message, maxMessage);

            // Building UI
            SetSize(NotificationArea.size, 100f);

            background = new Image(Colors.black);
            background.SetFill(FillType.All);
            Add(background);

            titleBackground = new Image(Colors.darkGrey);
            titleBackground.SetAnchor(AnchorType.TopMiddle);
            titleBackground.SetFill(FillType.Horizontal);
            titleBackground.SetSize(0f, 25f);

            titleLabel = new Label(title, 20);
            titleLabel.SetFill(FillType.All);
            titleBackground.Add(titleLabel);

            Add(titleBackground);

            messageLabel = new Label(message, 20);
            messageLabel.SetAnchor(AnchorType.BottomLeft);
            messageLabel.SetFill(FillType.All);
            messageLabel.SetOffset(0f, -12f);
            messageLabel.SetSize(-10f, -50f);

            messageLabel.text.resizeTextForBestFit = true;
            messageLabel.text.resizeTextMaxSize = 20;
            messageLabel.text.resizeTextMinSize = 15;

            // Aligning top left
            //messageLabel.SetOffset(0f, -6f);
            //messageLabel.SetAlignment(TextAnchor.UpperLeft);

            Add(messageLabel);

            // If this is an error notification, don't automatically hide
            if (type == NotificationType.Error) {
                return;
            }

            // Add timer to scale opacity
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
            if (timer != null) {
                timer.StopAllCoroutines();
            }

            Destroy();
        }
    }
}
