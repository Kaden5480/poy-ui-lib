using UnityEngine;

using UILib.Behaviours;
using UILib.Components;
using UILib.Layouts;

namespace UILib.Notifications {
    /**
     * <summary>
     * A notification.
     *
     * The component which is displayed to the user
     * when a notification is sent.
     * These pop up in the bottom right of the screen.
     * </summary>
     */
    internal class Notification : Area {
        private Timer timer;

        /**
         * <summary>
         * Initializes a notification.
         * </summary>
         * <param name="title">The title of the notification</param>
         * <param name="message">The message to display</param>
         * <param name="type">The type of this notification</param>
         * <param name="theme">The theme to use</param>
         */
        internal Notification(
            string title,
            string message,
            NotificationType type,
            Theme theme
        ) {
            int maxTitle = theme.notificationMaxTitle;
            int maxMessage = theme.notificationMaxMessage;

            float fadeTime = theme.notificationFadeTime;
            float waitTime = theme.notificationWaitTime;

            CanvasGroup canvasGroup = gameObject.AddComponent<CanvasGroup>();

            // Default opacity
            canvasGroup.alpha = theme.notificationOpacity;

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
            SetSize(UIRoot.notificationOverlay.width, 100f);

            Image background = new Image(theme.background);
            background.SetFill(FillType.All);
            Add(background);

            Image titleBackground = new Image(theme.accent);
            titleBackground.SetAnchor(AnchorType.TopMiddle);
            titleBackground.SetFill(FillType.Horizontal);
            titleBackground.SetSize(0f, 25f);

            Label titleLabel = new Label(title, 20);
            titleLabel.SetFill(FillType.All);
            titleBackground.Add(titleLabel);

            Add(titleBackground);

            Label messageLabel = new Label(message, 20);
            messageLabel.SetAnchor(AnchorType.BottomLeft);
            messageLabel.SetFill(FillType.All);
            messageLabel.SetOffset(0f, -8f);
            messageLabel.SetSize(-10f, -50f);

            messageLabel.text.resizeTextForBestFit = true;
            messageLabel.text.resizeTextMaxSize = 20;
            messageLabel.text.resizeTextMinSize = 15;

            Add(messageLabel);

            Label dismissLabel = new Label("Click anywhere to dismiss", 12);
            dismissLabel.SetAnchor(AnchorType.BottomMiddle);
            dismissLabel.SetFill(FillType.Horizontal);
            dismissLabel.SetSize(0f, 15f);
            Add(dismissLabel);

            // Apply theme to children
            SetTheme(theme);

            // Add warnings if error
            if (type == NotificationType.Error) {
                Label leftWarning = MakeWarning();
                leftWarning.SetAnchor(AnchorType.MiddleLeft);
                leftWarning.SetOffset(8f, 0f);
                titleBackground.Add(leftWarning);

                Label rightWarning = MakeWarning();
                rightWarning.SetAnchor(AnchorType.MiddleRight);
                rightWarning.SetOffset(-8f, 0f);
                titleBackground.Add(rightWarning);
            }

            // Customise small text
            dismissLabel.SetColor(theme.selectAltNormal);

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

                // What proportion of fade time has passed
                float fadeProportion = (fadeTime - value) / fadeTime;

                // Scale opacity
                canvasGroup.alpha = theme.notificationOpacity * (1 - fadeProportion);
            });

            timer.onEnd.AddListener(() => {
                Destroy();
            });

            timer.StartTimer(waitTime + fadeTime);
        }

        /**
         * <summary>
         * Makes a warning label.
         * </summary>
         * <returns>The warning label</returns>
         */
        private Label MakeWarning() {
            Label label = new Label("!", 32);
            label.SetColor(theme.importantNormal);
            label.SetSize(20f, 20f);

            return label;
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
