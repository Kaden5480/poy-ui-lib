using System;

using BepInEx;
using UnityEngine;

namespace UILib {
    [BepInPlugin("com.github.Kaden5480.poy-ui-lib", "UI Lib", PluginInfo.PLUGIN_VERSION)]
    internal class Plugin : BaseUnityPlugin {
        internal static Plugin instance { get; private set; }

        private Window window1;
        private Window window2;
        private Window window3;

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        private void Awake() {
            instance = this;

            UIRoot.Init();

            window1 = MakeWindow(800f, 600f);
            window1.SetAnchor(AnchorType.TopLeft);

            window2 = MakeWindow(300f, 500f);
            window2.SetAnchor(AnchorType.MiddleLeft);

            window3 = MakeWindow(600f, 800f);
            window3.SetAnchor(AnchorType.Middle);
        }

        private void Start() {
            window1.Show();
            window2.Show();
            window3.Show();
        }

        private Window MakeWindow(float width, float height) {
            Window window = new Window(width, height);
            ScrollView scrollView = new ScrollView();

            window.Add(scrollView);

            for (int i = 0; i < 20; i++) {
                scrollView.Add(new Label($"Hello, world! {i}"));
            }

            window.Hide();

            return window;
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
