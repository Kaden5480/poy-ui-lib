using System.Collections.Generic;

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
        // The canvas this overlay is attached to
        internal Canvas canvas;

        // Whether this overlay should pause the game
        // automatically when it's shown
        private bool autoPause;

        // This overlay's pause handle
        private PauseHandle pauseHandle;

        // This overlay's container
        internal Area container;

        // Canvas group for controlling opacity
        internal CanvasGroup canvasGroup;

        // Fade for controlling this overlay's canvas group
        internal Fade fade;

        // The behaviour for managing this overlay's
        // local shortcuts
        private LocalShortcuts localShortcuts;

        /**
         * <summary
         * Invokes listeners when this overlay becomes focused.
         * </summary>
         */
        public UnityEvent onFocus { get; } = new UnityEvent();

        /**
         * <summary
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
        public bool canInteract {
            get => canvas.raycaster.enabled;
            private set => canvas.raycaster.enabled = value;
        }

        /**
         * <summary>
         * Initializes an overlay.
         *
         * By default overlays will pause the game when shown.
         * If you want to disable this behaviour see <see cref="SetAutoPause"/>.
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
                base.Hide();
            });

            // Register this overlay
            UIRoot.Register(this);

            // Container for content
            container = new Area();
            container.SetFill(FillType.All);
            AddDirect(container);

            // Set to the middle of the screen by default
            SetAnchor(AnchorType.Middle);

            // Pause by default
            SetAutoPause(true);

            // Set size
            SetSize(width, height);

            // Set the theme
            SetTheme(theme);

            // Hide by default
            Hide();
        }

        /**
         * <summary>
         * Destroy this overlay and all children.
         * </summary>
         */
        public override void Destroy() {
            UIRoot.Unregister(this);
            base.Destroy();

            // If an active pause handle exists, remove it
            if (pauseHandle != null) {
                pauseHandle.Close();
            }
        }

        /**
         * <summary>
         * Allows setting the theme of this overlay
         * and all children.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        public override void SetTheme(Theme theme) {
            base.SetTheme(theme);

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
         * Sets whether this overlay can be interacted with.
         * </summary>
         * <param name="canInteract">Whether interactions should be allowed</param>
         */
        public virtual void SetInteractable(bool canInteract) {
            this.canInteract = canInteract;
        }

        /**
         * <summary>
         * Brings this overlay to the front
         * so that it displays above all others.
         * </summary>
         */
        public void BringToFront() {
            UIRoot.BringToFront(this);
        }

#endregion

#region Local Shortcuts

        /**
         * <summary>
         * Adds a local <see cref="Behaviours.Shortcut"/> to this overlay.
         *
         * Local shortcuts are triggered whenever
         * all keys configured in the shortcut are pressed.
         * </summary>
         * <param name="keys">The keys which must be pressed for this shortcut to trigger</param>
         * <returns>The <see cref="Behaviours.Shortcut"/> which was created</returns>
         */
        public Shortcut AddShortcut(IList<KeyCode> keys) {
            if (localShortcuts == null) {
                localShortcuts = gameObject.AddComponent<LocalShortcuts>();
                localShortcuts.overlay = this;
            }

            Shortcut shortcut = new Shortcut(keys);
            localShortcuts.Add(shortcut);

            return shortcut;
        }

#endregion

        /**
         * <summary>
         * Set whether this overlay should
         * automatically pause the game when it's shown.
         *
         * Calling with autoPause being `false`:
         * - If the overlay has an active <see cref="PauseHandle"/>,
         *   it will be closed immediately regardless of its visibility.
         *
         * Calling with autoPause being `true`:
         * - If the overlay is visible, a new <see cref="PauseHandle"/> will
         *   be allocated.
         * - If the overlay is hidden, no <see cref="PauseHandle"/> will
         *   be allocated.
         *
         * </summary>
         * <param name="autoPause">Whether to pause the game automatically</param>
         */
        public void SetAutoPause(bool autoPause) {
            this.autoPause = autoPause;

            // Remove pause handle, since auto pausing is disabled
            if (autoPause == false && pauseHandle != null) {
                pauseHandle.Close();
                pauseHandle = null;
                return;
            }

            // Allocate pause handle only if visible
            if (isVisible == true && pauseHandle == null) {
                pauseHandle = new PauseHandle();
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
         * creates a new <see cref="PauseHandle"/>
         * if auto-pausing is enabled.
         * </summary>
         */
        public override void Show() {
            // Make sure the game object is enabled first
            base.Show();

            // Bring to the front
            BringToFront();

            // Start fading in
            fade.FadeIn();

            if (pauseHandle != null) {
                pauseHandle.Close();
            }

            if (autoPause == true) {
                pauseHandle = new PauseHandle();
            }
        }

        /**
         * <summary>
         * Hides this overlay and closes
         * the internal <see cref="PauseHandle"/>.
         * </summary>
         */
        public override void Hide() {
            // Start fading out
            fade.FadeOut();

            if (pauseHandle != null) {
                pauseHandle.Close();
                pauseHandle = null;
            }
        }
    }
}
