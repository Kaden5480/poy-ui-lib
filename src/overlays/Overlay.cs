using System.Collections.Generic;

using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Events;

using UILib.Behaviours;
using UILib.Components;
using UILib.Layouts;
using UILib.Patches;

namespace UILib {
    /**
     * <summary>
     * A primitive UIObject which stays in a
     * fixed position on the screen.
     * </summary>
     */
    public class Overlay : UIObject {
        /**
         * <summary>
         * The sorting modes which can be applied to <see cref="Overlay">
         * Overlays</see>.
         * </summary>
         */
        public enum SortingMode {
            /**
             * <summary>
             * The default sorting mode. Overlays can be fully controlled
             * by <see cref="UIRoot"/>. They can freely be placed above/below each other
             * as the user focuses/unfocuses them.
             * </summary>
             */
            Default,

            /**
             * <summary>
             * In this mode, the overlay can only move down the sorting order,
             * it can't be brought above another overlay when the user focuses it.
             *
             * It will still get brought to the front initially when opened through
             * <see cref="Show"/>, this only disables increasing the sorting order
             * from user interactions.
             * </summary>
             */
            Recede,

            /**
             * <summary>
             * Tells <see cref="UIRoot"/> to completely ignore sorting this overlay,
             * letting you set whatever sorting order you want on its <see cref="Canvas"/>.
             * </summary>
             */
            Static,
        }

        /**
         * <summary>
         * This overlay's canvas.
         * </summary>
         */
        public Canvas canvas { get; private set; }

        /**
         * <summary>
         * The lock modes this overlay will automatically
         * use when it's visible.
         * </summary>
         */
        public LockMode lockMode { get; private set; }

        // This overlay's lock
        private Lock @lock;

        // Canvas group for controlling opacity
        internal CanvasGroup canvasGroup;

        // Fade for controlling this overlay's canvas group
        internal Fade fade;

        // The behaviour for managing this overlay's
        // local shortcuts
        private LocalShortcuts localShortcuts;

        /**
         * <summary>
         * The sorting mode this overlay is currently in.
         * </summary>
         */
        public SortingMode sortingMode { get; private set; } = SortingMode.Default;

        /**
         * <summary>
         * Invokes listeners when this overlay becomes focused.
         * </summary>
         */
        public UnityEvent onFocus { get; } = new UnityEvent();

        /**
         * <summary>
         * Invokes listeners when this overlay loses focus.
         * </summary>
         */
        public UnityEvent onLostFocus { get; } = new UnityEvent();

        /**
         * <summary>
         * Whether this overlay is currently focused.
         * If the overlay is focused, it means that it
         * can receive local shortcuts.
         * </summary>
         */
        public bool isFocused {
            get => UIRoot.focusedOverlay == this;
        }

        /**
         * <summary>
         * Whether the user can interact with this overlay.
         * </summary>
         */
        public bool enabled {
            get => canvas.raycaster.enabled;
            private set => canvas.raycaster.enabled = value;
        }

        /**
         * <summary>
         * Initializes an overlay.
         *
         * By default overlays will pause the game when shown.
         * If you want to disable this behaviour see <see cref="SetLockMode"/>.
         * </summary>
         * <param name="width">The width of the overlay</param>
         * <param name="height">The height of the overlay</param>
         */
        public Overlay(float width, float height) {
            // Get a canvas to draw this overlay on
            canvas = new Canvas();
            canvas.Add(this);

            // Add a canvas group for controlling opacity
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

            // Assign a Fade and give it the canvas group
            fade = gameObject.AddComponent<Fade>();
            fade.Add(canvasGroup);

            // Only disable once fading out has finished
            // Showing is handled in Show() as the gameObject
            // has to be enabled immediately
            fade.onFadeOut.AddListener(() => {
                canvas.Hide();
                base.Hide();
            });

            // Register this overlay
            UIRoot.Register(this);

            // Set to the middle of the screen by default
            SetAnchor(AnchorType.Middle);

            // Auto-pause by default
            SetLockMode(LockMode.Default);

            // Set size
            SetSize(width, height);

            // Set the theme
            SetThisTheme(theme);

            // Hide by default
            Hide(true);
        }

        /**
         * <summary>
         * Destroy this overlay and all children.
         * </summary>
         */
        public override void Destroy() {
            UIRoot.Unregister(this);
            base.Destroy();

            // If an active lock exists, remove it
            if (@lock != null) {
                @lock.Close();
            }
        }

        /**
         * <summary>
         * Allows setting the theme of this overlay.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {
            // Tell the fade to use a different opacity
            // and fade time
            fade.SetOpacities(max: theme.overlayOpacity);
            fade.SetFadeTime(theme.overlayFadeTime);

            // Update the canvas group
            canvasGroup.alpha = theme.overlayOpacity;
        }

#region Hovering/Focusing

        /**
         * <summary>
         * Handles this overlay being hovered over.
         * </summary>
         */
        protected override void OnPointerEnter() {
            UIRoot.SetHoveredOverlay(this);

            // If focus on hover is enabled, also
            // bring this overlay to the front
            if (Config.focusOnHover.Value == true) {
                BringToFront();
            }
        }

        /**
         * <summary>
         * Handles this overlay no longer being hovered over.
         * </summary>
         */
        protected override void OnPointerExit() {
            UIRoot.SetUnhoveredOverlay(this);
        }

        /**
         * <summary>
         * Bring this overlay's canvas to the front.
         * </summary>
         */
        protected override void OnClick() {
            BringToFront();
        }

        /**
         * <summary>
         * Enables interacting with this overlay.
         * </summary>
         */
        public virtual void Enable() {
            this.enabled = true;
        }

        /**
         * <summary>
         * Disables interacting with this overlay.
         * </summary>
         */
        public virtual void Disable() {
            this.enabled = false;
        }

        /**
         * <summary>
         * Sets the sorting mode this overlay is currently in
         * </summary>
         * <param name="sortingMode">The new sorting mode to use</param>
         */
        public void SetSortingMode(SortingMode sortingMode) {
            this.sortingMode = sortingMode;
        }

        /**
         * <summary>
         * Brings this overlay to the front
         * so that it displays above all others.
         *
         * If the sorting mode is <see cref="SortingMode.Recede"/>, this will
         * only cause the overlay to become focused.
         * It won't move to the front.
         * </summary>
         */
        public void BringToFront() {
            UIRoot.BringToFront(this);
        }

#endregion

#region Local Shortcuts

        /**
         * <summary>
         * Adds a local <see cref="Shortcut"/> to this overlay.
         *
         * Local shortcuts are triggered whenever
         * all keys configured in the shortcut are pressed.
         * </summary>
         * <param name="shortcut">The shortcut to add</param>
         */
        public void AddShortcut(Shortcut shortcut) {
            if (localShortcuts == null) {
                localShortcuts = gameObject.AddComponent<LocalShortcuts>();
                localShortcuts.overlay = this;
            }

            localShortcuts.Add(shortcut);
        }

#endregion

        /**
         * <summary>
         * Sets the lock mode this overlay will use
         * when it's visible.
         *
         * If the overlay is currently visible, the provided
         * lock mode will apply immediately.
         * </summary>
         * <param name="lockMode">The lock mode to use</param>
         */
        public void SetLockMode(LockMode lockMode) {
            this.lockMode = lockMode;

            // If a lock is active, update it
            if (@lock != null) {
                @lock.SetMode(lockMode);
            }
        }

        /**
         * <summary>
         * Toggles the visibility of this overlay.
         * </summary>
         */
        public override void ToggleVisibility() {
            if (isVisible == false || fade.fadingOut == true) {
                Show();
            }
            else {
                Hide();
            }
        }

        /**
         * <summary>
         * Makes this overlay visible and
         * creates a new <see cref="Patches.Lock"/>
         * with the current <see cref="lockMode"/>.
         * </summary>
         */
        public override void Show() {
            // Make sure the game object is enabled first
            canvas.Show();
            base.Show();

            // Bring to the front
            BringToFront();

            // Start fading in
            fade.FadeIn();

            // Acquire a lock
            if (@lock == null) {
                @lock = new Lock();
            }

            // Update the lock mode
            @lock.SetMode(lockMode);
        }

        /**
         * <summary>
         * Another hide method for force hiding, which bypasses
         * the configured fade duration.
         * </summary>
         * <param name="force">Whether to bypass fading</param>
         */
        private void Hide(bool force) {
            if (isVisible == true) {
                fade.FadeOut(force);
            }

            // Clear the current lock
            if (@lock != null) {
                @lock.Close();
                @lock = null;
            }
        }

        /**
         * <summary>
         * Hides this overlay and closes
         * the internal <see cref="Patches.Lock"/>.
         * </summary>
         */
        public override void Hide() {
            Hide(false);
        }
    }
}
