using System;

using BepInEx;
using UnityEngine;

namespace UILib {
    [BepInPlugin("com.github.Kaden5480.poy-ui-lib", "UI Lib", PluginInfo.PLUGIN_VERSION)]
    internal class Plugin : BaseUnityPlugin {
        internal static Plugin instance { get; private set; }

        private int buttonCount = 0;

        private FixedWindow fixedWindow;

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

            fixedWindow = MakeFixedWindow();

            window1 = MakeWindow(800f, 600f);
            window1.SetAnchor(AnchorType.TopLeft);

            window2 = MakeWindow(300f, 500f);
            window2.SetAnchor(AnchorType.MiddleLeft);

            window3 = MakeWindow(600f, 800f);
            window3.SetAnchor(AnchorType.Middle);
        }

        private void Start() {
            fixedWindow.Show();
            window1.Show();
            window2.Show();
            window3.Show();
        }

        private FixedWindow MakeFixedWindow() {
            FixedWindow window = new FixedWindow("Fixed Window", 0f, 0f);
            window.Fill();

            ScrollView scrollView = new ScrollView();
            scrollView.SetLayout(LayoutType.Vertical);
            scrollView.SetLayoutSpacing(20);
            scrollView.SetLayoutPadding(bottom: 20);
            window.Add(scrollView);

            Label header = new Label("My Config Options", 40);
            header.AddLayoutElement();
            header.SetSize(300f, 100f);
            scrollView.Add(header);

            float width = 200f;
            float height = 50f;

            for (int i = 0; i < 30; i++) {
                Area area = new Area();
                area.AddLayoutElement();
                area.SetSize(600f, height);
                area.SetLayout(LayoutType.Horizontal);
                area.SetLayoutSpacing(20f);

                Label label = new Label($"Config option: {i}", 20);
                label.AddLayoutElement();
                label.SetSize(width, height);
                area.Add(label);

                Button button = new Button($"Button for option: {i}", 20);
                button.AddLayoutElement();
                button.SetSize(width, height);
                area.Add(button);

                scrollView.Add(area);
            }

            window.Hide();

            return window;
        }

        private Window MakeWindow(float width, float height) {
            Window window = new Window("Some Window", width, height);
            ScrollView scrollView = new ScrollView();
            scrollView.SetLayout(LayoutType.Vertical);
            scrollView.SetLayoutPadding(bottom: 20);

            window.Add(scrollView);

            for (int i = 0; i < 20; i++) {
                Label label = new Label($"Hello, world! {i}", 40);
                label.AddLayoutElement();
                label.SetSize(250f, 80f);

                scrollView.Add(label);
            }

            Button button = new Button("Test Button", 40);
            button.AddLayoutElement();
            button.SetSize(250f, 50f);
            button.AddListener(() => {
                buttonCount++;
                Notifier.Notify($"Button was clicked {buttonCount} time(s)");
            });

            scrollView.Add(button);

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
