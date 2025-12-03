using BepInEx.Configuration;

namespace UILib {
    /**
     * <summary>
     * Class holding UILib's config.
     * </summary>
     */
    internal static class Config {
        // General
        internal static ConfigEntry<bool> showIntro { get; private set; }

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
        }

    }
}
