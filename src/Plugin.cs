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

        private int buttonCount = 0;

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

            //window1 = MakeWindow("Top Left", 800f, 600f);
            //window1.SetAnchor(AnchorType.TopLeft);

            //window2 = MakeWindow("Bottom Right", 300f, 500f);
            //window2.SetAnchor(AnchorType.BottomRight);

            window3 = MakeWindow("Middle", 600f, 800f);
            window3.SetAnchor(AnchorType.Middle);
        }

        private void Start() {
            //window1.Show();
            //window2.Show();
            window3.Show();
        }

        private Window MakeWindow(string name, float width, float height) {
            Window window = new Window(name, width, height);
            window.SetLayout(LayoutType.Vertical);
            window.SetLayoutSpacing(20);
            window.SetLayoutPadding(bottom: 20, top: 20);

            Toggle toggle = new Toggle();
            toggle.SetSize(40f, 40f);
            window.Add(toggle);

            Slider slider = new Slider();
            slider.SetSize(200f, 10f);
            window.Add(slider);

            UIButton sliderChanger = new UIButton("Change the slider!", 30);
            sliderChanger.SetSize(250f, 50f);
            sliderChanger.AddListener(() => {
                slider.SetDirection(UnityEngine.UI.Slider.Direction.BottomToTop);
            });
            window.Add(sliderChanger);

            Slider slider2 = new Slider(direction: UnityEngine.UI.Slider.Direction.BottomToTop);
            slider2.SetSize(10f, 200f);
            window.Add(slider2);

            for (int i = 0; i < 20; i++) {
                Label label = new Label($"Hello, world! {i}", 40);
                label.SetSize(250f, 50f);

                window.Add(label);
            }

            UIButton button = new UIButton("Test UIButton", 40);
            button.SetSize(250f, 50f);
            button.AddListener(() => {
                buttonCount++;
                Notifier.Notify($"UIButton was clicked {buttonCount} time(s)");
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
