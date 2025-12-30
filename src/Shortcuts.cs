using System;
using System.Collections.Generic;
using System.Linq;

using BepInEx.Configuration;
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
         *
         * Shortcuts **can't** run in these cases:
         * - UILib's `InputOverlay` waiting for an input
         * - An input field being selected (to avoid triggering while typing/deselecting)
         * - The player is editing keybinds in the `InGameMenu`
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

        /**
         * <summary>
         * Like <see cref="keys"/> but holds the BepInEx
         * `ConfigEntry` keys.
         * </summary>
         */
        public Dictionary<ConfigEntry<KeyCode>, bool> bepInKeys { get; private set; }

        // Whether onTrigger has been called
        private bool hasTriggered = false;

        /**
         * <summary>
         * Initializes a shortcut using the provided `KeyCodes`.
         *
         * All `keys` must be down for the shortcut to trigger.
         * </summary>
         * <param name="keys">The keys for the shortcut</param>
         */
        public Shortcut(IList<KeyCode> keys) : this(keys, null) {}

        /**
         * <summary>
         * Initializes a shortcut using the provided `KeyCodes`
         * wrapped in BepInEx `ConfigEntry` types.
         *
         * All `keys` must be down for the shortcut to trigger.
         *
         * As the provided `ConfigEntry` types are references, any updates
         * to their `Value` are automatically/naturally picked up by this shortcut.
         *
         * There is no reason to call <see cref="SetShortcut"/> again in this case.
         * </summary>
         * <param name="keys">The keys for the shortcut</param>
         */
        public Shortcut(IList<ConfigEntry<KeyCode>> keys) : this(null, keys) {}

        /**
         * <summary>
         * Initializes a shortcut.
         *
         * Both arguments are optional.
         *
         * All provided `keys` and `bepInKeys` must be down
         * for the shortcut to trigger.
         *
         * As the provided `ConfigEntry` types are references, any updates
         * to their `Value` are automatically/naturally picked up by this shortcut.
         *
         * There is no reason to call <see cref="SetShortcut"/> again in this case.
         * </summary>
         * <param name="keys">The shortcut's plain keys</param>
         * <param name="bepInKeys">The shortcut's BepInEx `ConfigEntry` keys</param>
         */
        public Shortcut(IList<KeyCode> keys = null, IList<ConfigEntry<KeyCode>> bepInKeys = null) {
            SetShortcut(keys, bepInKeys);
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
            Reset();
        }

        /**
         * <summary>
         * Sets a different shortcut to use instead.
         *
         * Both arguments are optional.
         *
         * All provided `keys` and `bepInKeys` must be down
         * for the shortcut to trigger.
         * </summary>
         * <param name="keys">The new shortcut</param>
         */
        public void SetShortcut(IList<KeyCode> keys = null, IList<ConfigEntry<KeyCode>> bepInKeys = null) {
            this.keys = new Dictionary<KeyCode, bool>();
            this.bepInKeys = new Dictionary<ConfigEntry<KeyCode>, bool>();

            if (keys != null) {
                foreach (KeyCode key in keys) {
                    this.keys[key] = false;
                }
            }

            if (bepInKeys != null) {
                foreach (ConfigEntry<KeyCode> key in bepInKeys) {
                    this.bepInKeys[key] = false;
                }
            }
        }

        /**
         * <summary>
         * Updates which keys are down.
         * </summary>
         */
        internal void UpdateKeysDown() {
            foreach (KeyCode key in keys.Keys.ToList()) {
                if (Input.GetKeyDown(key) == true) {
                    keys[key] = true;
                }
            }

            foreach (ConfigEntry<KeyCode> key in bepInKeys.Keys.ToList()) {
                if (Input.GetKeyDown(key.Value) == true) {
                    bepInKeys[key] = true;
                }
            }
        }

        /**
         * <summary>
         * Updates which keys are up.
         * </summary>
         */
        internal void UpdateKeysUp() {
            foreach (KeyCode key in keys.Keys.ToList()) {
                if (Input.GetKeyUp(key) == true) {
                    keys[key] = false;
                }
            }

            foreach (ConfigEntry<KeyCode> key in bepInKeys.Keys.ToList()) {
                if (Input.GetKeyUp(key.Value) == true) {
                    bepInKeys[key] = false;
                }
            }
        }

        /**
         * <summary>
         * Resets the key press state.
         * </summary>
         */
        internal void Reset() {
            foreach (KeyCode key in keys.Keys.ToList()) {
                keys[key] = false;
            }

            foreach (ConfigEntry<KeyCode> key in bepInKeys.Keys.ToList()) {
                bepInKeys[key] = false;
            }

            hasTriggered = false;
        }

        /**
         * <summary>
         * Checks whether all configured keys are down.
         * </summary>
         * <returns>True if they are, false otherwise</returns>
         */
        private bool AllKeysDown() {
            // If keys are empty, just do nothing
            if (keys.Count < 1 && bepInKeys.Count < 1) {
                return false;
            }

            // Check if all keys are down
            foreach (bool down in keys.Values) {
                if (down == false) {
                    return false;
                }
            }

            foreach (bool down in bepInKeys.Values) {
                if (down == false) {
                    return false;
                }
            }

            return true;
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

            // Check which keys are down
            UpdateKeysDown();

            // Check the current input state
            if (AllKeysDown() == false) {
                hasTriggered = false;
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
        private Logger logger = new Logger(typeof(GlobalShortcuts));

        // The shortcuts assigned
        internal List<Shortcut> shortcuts { get; } = new List<Shortcut>();

        /**
         * <summary>
         * Reset the state of shortcuts on disable.
         * </summary>
         */
        internal void OnDisable() {
            foreach (Shortcut shortcut in shortcuts) {
                shortcut.Reset();
            }
        }

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
            // Always update keys up
            foreach (Shortcut shortcut in shortcuts) {
                shortcut.UpdateKeysUp();
            }

            if (Shortcut.canRun == false) {
                return;
            }

            // Check if shortcuts should trigger
            foreach (Shortcut shortcut in shortcuts) {
                try {
                    shortcut.Check();
                }
                catch (Exception e) {
                    logger.LogError("Failed checking shortcut", e);
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
