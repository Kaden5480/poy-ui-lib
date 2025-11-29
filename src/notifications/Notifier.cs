namespace UILib.Notifications {
    /**
     * <summary>
     * A class which gives public access
     * to UILib's notifications.
     * </summary>
     */
    public static class Notifier {
        private static Logger logger = new Logger(typeof(Notifier));

        /**
         * <summary>
         * Sends a notification.
         *
         * This will also play whichever notification sounds
         * are configured in the theme.
         *
         * A `theme` of null just means to use UILib's default.
         * </summary>
         * <param name="title">The title (could just be the name of the mod)</param>
         * <param name="message">The message to display</param>
         * <param name="type">The type of notification to display</param>
         * <param name="theme">The theme to apply to the notification</param>
         */
        public static void Notify(
            string title,
            string message,
            NotificationType type = NotificationType.Normal,
            Theme theme = null
        ) {
            if (theme == null) {
                theme = UIRoot.defaultTheme;
            }

            Notification notification = new Notification(
                title, message, type, theme
            );

            switch (type) {
                case NotificationType.Silent:
                    break;
                case NotificationType.Normal:
                    Audio.Play(
                        theme.notification,
                        theme.notificationVolume
                    );
                    break;
                case NotificationType.Error:
                    Audio.Play(
                        theme.notificationError,
                        theme.notificationErrorVolume
                    );
                    break;
                default:
                    logger.LogDebug($"Unexpected notification type: {type}");
                    break;
            }

            UIRoot.notificationArea.Add(notification, false);
        }
    }
}
