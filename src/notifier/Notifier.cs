namespace UILib {
    /**
     * <summary>
     * A class which gives public access
     * to UILib's notifications.
     * </summary>
     */
    public static class Notifier {
        /**
         * <summary>
         * Sends a notification.
         * </summary>
         * <param name="message">The message to display</param>
         */
        public static void Notify(string message) {
            Notification notification = new Notification(message);
            UIRoot.notificationArea.Add(notification);
        }
    }
}
