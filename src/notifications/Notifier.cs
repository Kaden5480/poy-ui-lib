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
         * </summary>
         * <param name="message">The message to display</param>
         * <param name="type">The type of notification to display</param>
         */
        public static void Notify(
            string message,
            NotificationType type = NotificationType.Normal
        ) {
            Notification notification = new Notification(message);

            switch (type) {
                case NotificationType.Silent:
                    break;
                case NotificationType.Normal:
                    Audio.PlayNormal();
                    break;
                case NotificationType.Error:
                    Audio.PlayError();
                    break;
                default:
                    logger.LogDebug($"Unexpected notification type: {type}");
                    break;
            }

            UIRoot.notificationArea.Add(notification);
        }
    }
}
