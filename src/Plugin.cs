using System;

using BepInEx;
using UnityEngine;

using UILib.Components;
using UILib.Layout;
using UIButton = UILib.Components.Button;

namespace UILib {
    [BepInPlugin("com.github.Kaden5480.poy-ui-lib", "UI Lib", PluginInfo.PLUGIN_VERSION)]
    internal class Plugin : BaseUnityPlugin {
        internal static Plugin instance { get; private set; }

        private int logCount = 0;

        private Window window;

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        private void Awake() {
            instance = this;

            UIRoot.Init();

            window = MakeLog();
            window.SetAnchor(AnchorType.TopLeft);
        }

        private void Start() {
            window.Show();
        }

        private Window MakeLog() {
            Window window = new Window("Log Example", 800f, 600f);
            window.SetLayout(LayoutType.Vertical);

            QueueArea area = new QueueArea(5);
            window.Add(area);

            area.SetLayout(LayoutType.Vertical);
            area.SetAnchor(AnchorType.BottomMiddle);
            area.SetFill(FillType.Vertical);

            UIButton button = new UIButton("Add some content", 40);
            button.SetSize(200f, 50f);
            button.AddListener(() => {
                LogDebug("Button clicked");
                Label label = new Label($"Content: {logCount}", 20);
                label.SetSize(200f, 50f);
                area.Add(label);
                logCount++;
            });

            window.Add(button);
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
