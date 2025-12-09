using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using UILib.ColorPicker;
using UILib.Behaviours;
using UILib.Notifications;

namespace UILib {
    /**
     * <summary>
     * The root of UILib.
     * This initializes audio, notifications, the input overlay, and
     * handles window management.
     *
     * You should use <see cref="UIRoot.onInit"/> to know when
     * you can start using UILib. `onInit` invokes listeners
     * once UILib has finished setting everything up internally.
     * </summary>
     */
    public static class UIRoot {
        /**
         * <summary>
         * Invokes listeners once UILib has been initialized.
         * This indicates when you can start building UIs.
         * </summary>
         */
        public static UnityEvent onInit { get; } = new UnityEvent();

        /**
         * <summary>
         * Invokes listeners once UILib's window
         * management has been initialized.
         *
         * This happens later than <see cref="onInit"/>
         * and indicates when canvases have been assigned sorting
         * orders.
         * </summary>
         */
        public static UnityEvent onWMInit { get; } = new UnityEvent();

        // The global shortcuts
        private static GlobalShortcuts globalShortcuts;

        // The minimum sorting order to apply to Overlay canvases
        private const int minSortingOrder = 1000;

        // Sorting order for the input overlay
        internal const int inputOverlaySortingOrder = 9998;

        // Sorting order for the notification area
        internal const int notificationSortingOrder = 9999;

        // The default theme used across UILib
        internal static Theme defaultTheme { get; } = new Theme();

        // UIRoot's GameObject
        private static GameObject gameObject;

        // The input overlay and notification area
        internal static InputOverlay inputOverlay;
        internal static NotificationArea notificationArea;

        // The color picker window
        internal static ColorPickerWindow colorPickerWindow;

        // Currently available overlays
        private static List<Overlay> overlays;

        // Currently hovered/focused overlay
        internal static Overlay hoveredOverlay { get; private set; }
        internal static Overlay focusedOverlay { get; private set; }

        // Whether UILib is fully initialized, including window management
        private static bool isInitialized = false;

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

            // Initialize global shortcuts
            globalShortcuts = gameObject.AddComponent<GlobalShortcuts>();

            // Initialize the color picker
            colorPickerWindow = new ColorPickerWindow();

            onInit.Invoke();
        }

        /**
         * <summary>
         * Initializes window management.
         * </summary>
         */
        internal static void InitWM() {
            if (isInitialized == true || overlays == null) {
                return;
            }

            // Assign all sorting orders
            for (int i = 0; i < overlays.Count; i++) {
                overlays[i].canvas.canvas.sortingOrder = minSortingOrder + i;
            }

            inputOverlay.canvas.canvas.sortingOrder = inputOverlaySortingOrder;
            notificationArea.canvas.canvas.sortingOrder = notificationSortingOrder;

            isInitialized = true;
            onWMInit.Invoke();
        }

        /**
         * <summary>
         * Registers an overlay for sorting.
         * </summary>
         * <param name="overlay">The overlay to register</param>
         */
        internal static void Register(Overlay overlay) {
            UIObject.SetParent(gameObject, overlay.canvas.gameObject);

            // Don't control InputOverlay
            if (overlay.GetType() == typeof(InputOverlay)) {
                return;
            }

            // Only assign a sorting order if window management
            // has been initialized
            if (isInitialized == true) {
                overlay.canvas.canvas.sortingOrder = minSortingOrder + overlays.Count;
            }

            overlays.Add(overlay);
        }

        /**
         * <summary>
         * Unregisters an overlay for sorting.
         * </summary>
         * <param name="overlay">The overlay to unregister</param>
         */
        internal static void Unregister(Overlay overlay) {
            // Make sure the sorting order stays nice
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
         * Adds a global shortcut. Global shortcuts can be triggered
         * at any time, but only when the <see cref="InputOverlay"/>
         * isn't waiting for an input.
         *
         * This is the recommended way of handling global shortcuts
         * when working with UILib.
         * </summary>
         * <param name="keys">The keys which must be pressed for this shortcut to trigger</param>
         * <returns>The <see cref="Behaviours.Shortcut"/> which was created</returns>
         */
        public static Shortcut AddShortcut(IList<KeyCode> keys) {
            Shortcut shortcut = new Shortcut(keys);
            globalShortcuts.Add(shortcut);

            return shortcut;
        }


#region Hovering/Focusing

        /**
         * <summary>
         * Sets a overlay to be in front of all others.
         *
         * Also handles setting the focus of an overlay.
         * </summary>
         * <param name="overlay">The overlay to bring to the front</param>
         */
        internal static void BringToFront(Overlay overlay) {
            // Try finding the overlay
            int index = overlays.IndexOf(overlay);

            // If not fully initialized, just re-add
            if (isInitialized == false && index >= 0) {
                overlays.Remove(overlay);
                overlays.Add(overlay);

                return;
            }

            // Make the overlay focused
            focusedOverlay = overlay;
            overlay.onFocus.Invoke();

            // Check for unsortable overlays
            if (overlay.sortable == false || index < 0) {
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

        /**
         * <summary>
         * Tells UIRoot which overlay currently has
         * the pointer hovering over it.
         * </summary>
         * <param name="overlay">The overlay being hovered over</param>
         */
        internal static void SetHoveredOverlay(Overlay overlay) {
            hoveredOverlay = overlay;
        }

        /**
         * <summary>
         * Tells UIRoot that an overlay no longer has
         * the pointer hovering over it.
         * </summary>
         * <param name="overlay">The overlay no longer being hovered over</param>
         */
        internal static void SetUnhoveredOverlay(Overlay overlay) {
            // If this overlay was the one being hovered over, unset it
            if (overlay == hoveredOverlay) {
                hoveredOverlay = null;
            }
        }

        /**
         * <summary>
         * Checks for a mouse input. If there is one
         * and there isn't currently an overlay being hovered over,
         * make sure the focus is lost.
         * </summary>
         */
        internal static void Update() {
            if (Input.GetMouseButtonDown(0) == false
                && Input.GetMouseButtonDown(1) == false
            ) {
                return;
            }

            if (hoveredOverlay == null && focusedOverlay != null) {
                focusedOverlay.onLostFocus.Invoke();
                focusedOverlay = null;
            }
        }

#endregion

    }
}
