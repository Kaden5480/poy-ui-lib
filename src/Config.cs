using BepInEx.Configuration;
using UnityEngine;

#if MODMENU
using ModMenu.Config;
#endif

using UILib.ColorPicker;

namespace UILib {
    /**
     * <summary>
     * Class holding UILib's config.
     * </summary>
     */
    internal static class Config {
#if MODMENU
        // General
        [Field("Show Intro on Startup")]
        internal static ConfigEntry<bool> showIntro { get; private set; }

        internal static ConfigEntry<string> selectedTheme { get; private set; }

        // Keybinds
        [Category("Keybinds")]
        [Field("Drag Window", "The shortcut to move a window from anywhere.")]
        private const string dragWindow = "Left Alt + Left Click";

        [Category("Keybinds")]
        [Field("Resize Window", "The shortcut to resize a window from anywhere.")]
        private const string resizeWindow = "Left Alt + Right Click";

        [Field("Open Color Picker")]
        internal static ConfigEntry<KeyCode> openColorPicker { get; private set; }

        // Window Management
        [Category("Window Management")]
        [Field("Focus on Hover")]
        internal static ConfigEntry<bool> focusOnHover { get; private set; }
#else
        internal static ConfigEntry<bool> showIntro { get; private set; }
        internal static ConfigEntry<string> selectedTheme { get; private set; }
        internal static ConfigEntry<KeyCode> openColorPicker { get; private set; }
        internal static ConfigEntry<bool> focusOnHover { get; private set; }
#endif

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
                "Whether to show the intro on startup."
            );

            selectedTheme = configFile.Bind(
                "General", "selectedTheme", "Peaks Dark",
                "The currently selected theme to use by default."
            );

            // Keybinds
            openColorPicker = configFile.Bind(
                "Keybinds", "openColorPicker", KeyCode.F10,
                "Keybind to open the color picker in a \"detached\" mode."
            );

            // Window Management
            focusOnHover = configFile.Bind(
                "WindowManagement", "focusOnHover", false,
                "Whether windows should be focused when you hover over them."
            );
        }
    }
}
