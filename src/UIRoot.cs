using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using UILib.Notifications;

namespace UILib {
    /**
     * <summary>
     * The root of UILib.
     * This initializes audio, notifications, the input overlay, and
     * handles window management.
     * </summary>
     */
    public static class UIRoot {
        /**
         * <summary>
         * Invokes listeners once UILib has been initialized.
         * </summary>
         */
        public static UnityEvent onInit { get; } = new UnityEvent();

        // The minimum sorting order to apply to Overlay canvases
        private const int minSortingOrder = 1000;

        // Sorting order for the input overlay
        internal const int inputOverlaySortingOrder = 9998;

        // Sorting order for the notification area
        internal const int notificationSortingOrder = 9999;

        private static GameObject gameObject;

        private static List<Overlay> overlays;
        internal static InputOverlay inputOverlay;
        internal static NotificationArea notificationArea;

        // The default theme used across UILib
        internal static Theme defaultTheme { get; } = new Theme();

        /**
         * <summary>
         * Initializes the UIRoot class.
         * </summary>
         */
        internal static void Init() {
            if (gameObject != null) {
                return;
            }

            // Instantiate game object to attach all overlays to
            gameObject = new GameObject("UILib Root");
            gameObject.layer = LayerMask.NameToLayer("UI");
            GameObject.DontDestroyOnLoad(gameObject);

            overlays = new List<Overlay>();

            // Initialize the notification area
            notificationArea = new NotificationArea();
            UIObject.SetParent(gameObject, notificationArea.canvas.gameObject);

            // Initialize audio
            Audio.Init();

            // Initialize input overlay
            inputOverlay = new InputOverlay();
            UIObject.SetParent(gameObject, inputOverlay.canvas.gameObject);

            onInit.Invoke();
        }

        /**
         * <summary>
         * Registers an overlay for sorting.
         * </summary>
         * <param name="overlay">The overlay to register</param>
         */
        internal static void Register(Overlay overlay) {
            UIObject.SetParent(gameObject, overlay.canvas.gameObject);
            overlay.canvas.canvas.sortingOrder = minSortingOrder + overlays.Count;
            overlays.Add(overlay);
        }

        /**
         * <summary>
         * Unregisters an overlay for sorting.
         * </summary>
         * <param name="overlay">The overlay to unregister</param>
         */
        internal static void Unregister(Overlay overlay) {
            BringToFront(overlay);
            overlays.Remove(overlay);
        }

        /**
         * <summary>
         * Iterates over all overlays and ensures their canvases
         * are enabled.
         * </summary>
         */
        internal static void EnableCanvases() {
            foreach (Overlay overlay in overlays) {
                overlay.canvas.Show();
            }

            inputOverlay.canvas.Show();
            notificationArea.canvas.Show();
        }

        /**
         * <summary>
         * Sets a overlay to be in front of all others.
         * </summary>
         * <param name="overlay">The overlay to bring to the front</param>
         */
        internal static void BringToFront(Overlay overlay) {
            // Try finding the overlay
            int index = overlays.IndexOf(overlay);

            if (index < 0) {
                return;
            }

            // Iterate the list in reverse, decrementing all sorting orders
            // until reaching the canvas to set on top
            for (int i = overlays.Count - 1; i > index; i--) {
                overlays[i].canvas.canvas.sortingOrder--;
            }

            // Now remove the overlay from the list, and add it back
            // to the end, while also updating the sorting order
            overlays.Remove(overlay);
            overlay.canvas.canvas.sortingOrder = minSortingOrder + overlays.Count;
            overlays.Add(overlay);
        }
    }
}
