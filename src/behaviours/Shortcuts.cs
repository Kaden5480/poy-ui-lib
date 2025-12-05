using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace UILib.Behaviours {
    /**
     * <summary>
     * A class which represents a shortcut.
     * This holds important information about a shortcut
     * such as the event listener and underlying keybinds.
     * </summary>
     */
    public class Shortcut {
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
         * Which `KeyCodes` need to be entered
         * at the same time for this shortcut
         * to trigger.
         * </summary>
         */
        public List<KeyCode> keys { get; private set; }

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
            this.keys = new List<KeyCode>(keys);
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
         * Checks whether all provided `KeyCodes` are down.
         * </summary>
         * <param name="keys">The keys to check</param>
         * <returns>True if they are, false otherwise</returns>
         */
        private bool AllKeysDown(IList<KeyCode> keys) {
            // If keys is empty, just do nothing
            if (keys.Count < 1) {
                return false;
            }

            foreach (KeyCode key in keys) {
                if (Input.GetKeyDown(key) == false) {
                    return false;
                }
            }

            return true;
        }

        /**
         * <summary>
         * If any of the assigned shortcuts are
         * triggered, invoke the associated listeners.
         * </summary>
         */
        internal virtual void Update() {
            // Don't do anything if the input overlay
            // is waiting for an input
            if (InputOverlay.waitingForInput == true) {
                return;
            }

            // Also don't do anything if the player
            // is currently in the keybinding options
            if (Patches.Cache.inGameMenu != null
                && Patches.Cache.inGameMenu.isCurrentlyRemapping == true
            ) {
                return;
            }

            foreach (Shortcut shortcut in shortcuts) {
                // Ignore disabled shortcuts
                if (shortcut.enabled == false) {
                    continue;
                }

                if (AllKeysDown(shortcut.keys) == true) {
                    shortcut.onTrigger.Invoke();
                }
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
