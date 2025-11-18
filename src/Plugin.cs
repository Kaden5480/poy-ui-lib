using System;

using BepInEx;
using UnityEngine;

namespace UILib {
    [BepInPlugin("com.github.Kaden5480.poy-ui-lib", "UI Lib", PluginInfo.PLUGIN_VERSION)]
    internal class Plugin : BaseUnityPlugin {
        internal static Plugin instance { get; private set; }

        private Window window;
        private ScrollView scrollView;

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        private void Awake() {
            instance = this;

            window = new Window(600f, 800f);
            window.DontDestroyOnLoad();

            scrollView = new ScrollView(ScrollType.Vertical);
            window.AddChild(scrollView);

            scrollView.AddChild(new Button("Cool button"));

            for (int i = 0; i < 20; i++) {
                scrollView.AddChild(
                    new Label($"Hello, world! {i}")
                );
            }

            // TODO: Initialize the scrollbar to the top somehow
            // TODO: Add buttons
            // TODO: Add checkboxes
            // TODO: Add text input
            // TODO: Add sliders
        }

        /**
         * <summary>
         * Logs a debug message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        internal static void LogDebug(string message) {
#if DEBUG
            if (instance == null) {
                Console.WriteLine($"[Debug] UILib: {message}");
                return;
            }

            instance.Logger.LogInfo(message);
#else
            if (instance != null) {
                instance.Logger.LogDebug(message);
            }
#endif
        }

        /**
         * <summary>
         * Logs an informational message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        internal static void LogInfo(string message) {
            if (instance == null) {
                Console.WriteLine($"[Info] UILib: {message}");
                return;
            }
            instance.Logger.LogInfo(message);
        }

        /**
         * <summary>
         * Logs an error message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        internal static void LogError(string message) {
            if (instance == null) {
                Console.WriteLine($"[Error] UILib: {message}");
                return;
            }
            instance.Logger.LogError(message);
        }
    }
}
