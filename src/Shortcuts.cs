using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

using UILib.Patches.UI;

namespace UILib {
    /**
     * <summary>
     * A class which represents a shortcut.
     * This holds important information about a shortcut
     * such as the event listener and underlying keybinds.
     *
     * Also has a useful field <see cref="Shortcut.canRun"/> which indicates whether
     * a shortcut can run. This takes into consideration a
     * variety of cases including:
     * - UILib's `InputOverlay` waiting for an input
     * - An input field being selected (to avoid triggering while typing/deselecting)
     * - The player is editing keybinds in the `InGameMenu`
     * </summary>
     */
    public class Shortcut {
        private static bool CheckCanRun() {
            // Don't do anything if the input overlay
            // is waiting for an input
            if (InputOverlay.waitingForInput == true) {
                return false;
            }

            // Don't do anything when a text field is selected
            if (InputFieldFix.isSelected == true) {
                return false;
            }

            // Also don't do anything if the player
            // is currently in the keybinding options
            if (Patches.Cache.inGameMenu != null
                && Patches.Cache.inGameMenu.isCurrentlyRemapping == true
            ) {
                return false;
            }

            return true;
        }

        /**
         * <summary>
         * A static property which indicates whether a shortcut can run.
         * </summary>
         */
        public static bool canRun { get => CheckCanRun(); }

        /**
         * <summary>
         * Whether this shortcut is currently enabled.
         * </summary>
         */
        public bool enabled { get; private set; } = true;

        /**
         * <summary>
         * The event which is triggered when
         * this shortcut is entered.
         * </summary>
         */
        public UnityEvent onTrigger { get; } = new UnityEvent();

        /**
         * <summary>
         * The event which triggers for the entire duration
         * the shortcut is held.
         * </summary>
         */
        public UnityEvent onHold { get; } = new UnityEvent();

        /**
         * <summary>
         * Which `KeyCodes` need to be entered
         * at the same time for this shortcut
         * to trigger.
         * </summary>
         */
        public Dictionary<KeyCode, bool> keys { get; private set; }

        // Whether onTrigger has been called
        private bool hasTriggered = false;

        /**
         * <summary>
         * Initializes a shortcut
         * </summary>
         * <param name="keys">The shortcut</param>
         */
        internal Shortcut(IList<KeyCode> keys) {
            SetShortcut(keys);
        }

        /**
         * <summary>
         * Enables this shortcut. This allows it
         * to be triggered.
         *
         * By default shortcuts are enabled anyway,
         * so you'd only need to use this if you
         * have already disabled it.
         * </summary>
         */
        public void Enable() {
            this.enabled = true;
        }

        /**
         * <summary>
         * Disables this shortcut. This prevents it
         * from being triggered.
         * </summary>
         */
        public void Disable() {
            this.enabled = false;
        }

        /**
         * <summary>
         * Sets a different shortcut to use instead.
         * </summary>
         * <param name="keys">The new shortcut</param>
         */
        public void SetShortcut(IList<KeyCode> keys) {
            this.keys = new Dictionary<KeyCode, bool>();

            foreach (KeyCode key in keys) {
                this.keys[key] = false;
            }
        }

        /**
         * <summary>
         * Tracks key releases.
         * </summary>
         */
        internal void UpdateKeysUp() {
            foreach (KeyCode key in keys.Keys.ToList()) {
                if (Input.GetKeyUp(key) == true) {
                    keys[key] = false;
                }
            }
        }

        /**
         * <summary>
         * Updates the keys which are currently down.
         * </summary>
         */
        internal void UpdateKeysDown() {
            foreach (KeyCode key in keys.Keys.ToList()) {
                if (Input.GetKeyDown(key) == true) {
                    keys[key] = true;
                }
            }

            if (AllKeysDown() == false) {
                hasTriggered = false;
            }
        }

        /**
         * <summary>
         * Checks whether all configured keys are down.
         * </summary>
         * <returns>True if they are, false otherwise</returns>
         */
        private bool AllKeysDown() {
            // If keys is empty, just do nothing
            if (keys.Count < 1) {
                return false;
            }

            return keys.Values.All(isDown => isDown);
        }

        /**
         * <summary>
         * Checks the current state of the input,
         * invoking listeners where necessary.
         * </summary>
         */
        internal void Check() {
            // Do nothing if disabled
            if (enabled == false) {
                return;
            }

            UpdateKeysDown();

            // Check the current input state
            if (AllKeysDown() == false) {
                return;
            }

            // Call onTrigger
            if (hasTriggered == false) {
                onTrigger.Invoke();
                hasTriggered = true;
            }

            // Call onHold
            onHold.Invoke();
        }
    }

    /**
     * <summary>
     * The behaviour which handles global shortcuts.
     * </summary>
     */
    internal class GlobalShortcuts : MonoBehaviour {
        // The shortcuts assigned
        internal List<Shortcut> shortcuts { get; } = new List<Shortcut>();

        /**
         * <summary>
         * Adds a new shortcut to this behaviour.
         * </summary>
         * <param name="shortcut">The shortcut to add</param>
         */
        internal void Add(Shortcut shortcut) {
            shortcuts.Add(shortcut);
        }

        /**
         * <summary>
         * If any of the assigned shortcuts are
         * triggered, invoke the associated listeners.
         * </summary>
         */
        internal virtual void Update() {
            // Always need to update keys down
            foreach (Shortcut shortcut in shortcuts) {
                shortcut.UpdateKeysUp();
            }

            if (Shortcut.canRun == false) {
                return;
            }

            // Check if shortcuts should trigger
            foreach (Shortcut shortcut in shortcuts) {
                shortcut.Check();
            }
        }
    }

    /**
     * <summary>
     * The behaviour which handles local shortcuts.
     * </summary>
     */
    internal class LocalShortcuts : GlobalShortcuts {
        // The overlay this behaviour is for
        internal Overlay overlay;

        /**
         * <summary>
         * If any of the assigned shortcuts are
         * triggered, invoke the associated listeners.
         * </summary>
         */
        internal override void Update() {
            // Only run when the overlay is focused
            if (overlay.isFocused == true) {
                base.Update();
            }
        }
    }
}
