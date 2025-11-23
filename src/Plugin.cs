using System;

using BepInEx;
using UnityEngine;

namespace UILib {
    [BepInPlugin("com.github.Kaden5480.poy-ui-lib", "UI Lib", PluginInfo.PLUGIN_VERSION)]
    internal class Plugin : BaseUnityPlugin {
        internal static Plugin instance { get; private set; }

        private int buttonCount = 0;

        private FixedWindow configWindow;

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

            configWindow = MakeConfigWindow();

            window1 = MakeWindow("Top Left", 800f, 600f);
            window1.SetAnchor(AnchorType.TopLeft);

            window2 = MakeWindow("Bottom Right", 300f, 500f);
            window2.SetAnchor(AnchorType.BottomRight);

            window3 = MakeWindow("Middle", 600f, 800f);
            window3.SetAnchor(AnchorType.Middle);
        }

        private void Start() {
            //configWindow.Show();
            window1.Show();
            window2.Show();
            window3.Show();
        }

        private FixedWindow MakeConfigWindow() {
            FixedWindow window = new FixedWindow("Config Window", 0f, 0f);
            window.Fill();

            window.SetLayout(LayoutType.Vertical);
            window.SetLayoutSpacing(20);
            window.SetLayoutPadding(bottom: 20);

            Label header = new Label("My Config Options", 40);
            header.SetSize(300f, 100f);
            window.Add(header);

            float width = 200f;
            float height = 50f;

            for (int i = 0; i < 30; i++) {
                Area area = new Area();
                area.SetSize(600f, height);
                area.SetLayout(LayoutType.Horizontal);
                area.SetLayoutSpacing(20f);

                Label label = new Label($"Config option: {i}", 20);
                label.SetSize(width, height);
                area.Add(label);

                Button button = new Button($"Button for option: {i}", 20);
                button.SetSize(width, height);
                string current = $"{i}";
                button.AddListener(() => {
                    Notifier.Notify($"Option {current} was clicked");
                });
                area.Add(button);

                window.Add(area);
            }

            window.Hide();

            return window;
        }

        private Window MakeWindow(string name, float width, float height) {
            Window window = new Window(name, width, height);
            window.SetLayout(LayoutType.Vertical);
            window.SetLayoutSpacing(20);
            window.SetLayoutPadding(bottom: 20, top: 20);

            Toggle toggle = new Toggle();
            toggle.SetSize(40f, 40f);
            window.Add(toggle);

            for (int i = 0; i < 20; i++) {
                Label label = new Label($"Hello, world! {i}", 40);
                label.SetSize(250f, 50f);

                window.Add(label);
            }

            Button button = new Button("Test Button", 40);
            button.SetSize(250f, 50f);
            button.AddListener(() => {
                buttonCount++;
                Notifier.Notify($"Button was clicked {buttonCount} time(s)");
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
