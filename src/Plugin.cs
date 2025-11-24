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
        private Window window2;

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

            window2 = MakeWindow("Cool Window", 800f, 600f);
            window2.SetAnchor(AnchorType.Middle);
        }

        private void Start() {
            window.Show();
            window2.Show();
        }

        private Window MakeWindow(string name, float width, float height) {
            Window window = new Window(name, width, height);
            window.SetLayout(LayoutType.Vertical);
            window.SetLayoutPadding(top: 20, bottom: 20);
            window.SetElementSpacing(20);

            Label titleLabel = new Label("Title", 40);
            titleLabel.SetSize(200f, 100f);
            window.Add(titleLabel);

            for (int i = 0; i < 20; i++) {
                Area area = new Area();
                area.SetLayout(LayoutType.Horizontal);
                area.SetFill(FillType.All);

                Label label = new Label($"Some text: {i}", 20);
                label.SetSize(200f, 50f);
                area.Add(label);

                UIButton button = new UIButton($"Button: {i}", 20);
                button.SetSize(200f, 50f);
                area.Add(button);

                window.Add(area);
            }

            window.Hide();

            return window;
        }

        private Window MakeLog() {
            Window window = new Window("Log Example", 800f, 600f);

            QueueArea area = new QueueArea(20);
            area.SetLayout(LayoutType.Vertical);
            area.SetFill(FillType.All);

            area.rectTransform.anchoredPosition = new Vector2(0f, 50f);
            area.SetSize(0f, -100f);

            UIButton button = new UIButton("Add some content", 40);
            button.SetSize(0f, 100f);
            button.SetAnchor(AnchorType.BottomMiddle);
            button.SetFill(FillType.Horizontal);
            button.AddListener(() => {
                logCount++;

                Label label = new Label($"Log: {logCount}", 20);
                label.SetSize(200f, 50f);
                area.Add(label);

                window.ScrollToBottom();
            });

            window.Add(area);
            window.Add(button);

            window.SetContent(area);

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
