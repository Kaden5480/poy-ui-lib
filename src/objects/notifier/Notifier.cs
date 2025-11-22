namespace UILib {
    public static class Notifier {
        public static void Notify(string message) {
            Notification notification = new Notification(message);
            UIRoot.notificationArea.Add(notification);
        }
    }
}
