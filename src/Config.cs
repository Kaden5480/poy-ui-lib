using BepInEx.Configuration;
using UnityEngine;

namespace UILib {
    /**
     * <summary>
     * Class holding UILib's config.
     * </summary>
     */
    internal static class Config {
        // General
        internal static ConfigEntry<bool> showIntro { get; private set; }

        // Keybinds
        internal static ConfigEntry<KeyCode> openColorPicker { get; private set; }

        // Window Management
        internal static ConfigEntry<bool> focusOnHover { get; private set; }

        /**
         * <summary>
         * Initializes the config.
         * </summary>
         * <param name="configFile">The config file to use</param>
         */
        internal static void Init(ConfigFile configFile) {
            // General
            showIntro = configFile.Bind(
                "General", "showIntro", true,
                "Whether to show the intro on startup"
            );

            // Keybinds
            openColorPicker = configFile.Bind(
                "Keybinds", "openColorPicker", KeyCode.F10,
                "Keybind to open the color picker in a \"detached\" mode"
            );

            // Window Management
            focusOnHover = configFile.Bind(
                "WindowManagement", "focusOnHover", false,
                "Whether windows should be focused when you hover over them"
            );
        }
    }
}
