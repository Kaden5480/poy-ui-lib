using System;

using BepInEx;
using UnityEngine;

using Direction = UnityEngine.UI.Slider.Direction;

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

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Home) == true) {
                Audio.Play(Resources.notificationError, 0.6f);
                Notifier.Notify("One of your mods broke!");
            }
            else if (Input.GetKeyDown(KeyCode.End) == true) {
                Audio.Play(Resources.notification, 0.8f);
                Notifier.Notify("Informational notification");
            }
        }

        private void Start() {
            window.Show();
            window2.Show();
        }

        private Window MakeWindow(string name, float width, float height) {
            Window window = new Window(name, width, height);
            window.SetContentLayout(LayoutType.Vertical);
            window.SetContentPadding(top: 20, bottom: 20);
            window.SetElementSpacing(20);

            Label titleLabel = new Label("Title", 40);
            titleLabel.SetSize(200f, 100f);
            window.Add(titleLabel);

            UIButton notif = new UIButton("Notification", 20);
            notif.SetSize(200f, 40f);
            notif.onClick.AddListener(() => { Audio.Play(Resources.notification, 0.8f); });
            window.Add(notif);

            UIButton notifError = new UIButton("Error Notification", 20);
            notifError.SetSize(200f, 40f);
            notifError.onClick.AddListener(() => { Audio.Play(Resources.notificationError, 0.6f); });
            window.Add(notifError);

            TextField textField = new TextField("Placeholder text", 24);
            textField.SetSize(200f, 40f);
            window.Add(textField);

            Toggle toggle = new Toggle();
            toggle.SetSize(40f, 40f);
            window.Add(toggle);

            Slider slider = new Slider();
            slider.SetSize(200f, 10f);
            window.Add(slider);

            UIButton sliderChanger = new UIButton("Change the slider!", 20);
            sliderChanger.SetSize(200f, 50f);
            sliderChanger.onClick.AddListener(() => {
                if (slider.slider.direction == Direction.LeftToRight) {
                    slider.SetDirection(Direction.BottomToTop);
                }
                else {
                    slider.SetDirection(Direction.LeftToRight);
                }
            });
            window.Add(sliderChanger);

            for (int i = 0; i < 20; i++) {
                Area area = new Area();
                area.SetContentLayout(LayoutType.Horizontal);
                area.SetFill(FillType.All);

                Label label = new Label($"Some text: {i}", 20);
                label.SetSize(200f, 50f);
                area.Add(label);

                UIButton button = new UIButton($"Button: {i}", 20);
                button.SetSize(200f, 50f);

                string current = $"{i}";
                button.onClick.AddListener(() => {
                    Notifier.Notify($"Button {current} was pressed");
                });
                area.Add(button);

                window.Add(area);
            }

            window.Hide();

            return window;
        }

        private Window MakeLog() {
            Window window = new Window("Log Example", 800f, 600f);

            QueueArea area = new QueueArea(20);
            area.SetContentLayout(LayoutType.Vertical);
            area.SetFill(FillType.All);

            area.rectTransform.anchoredPosition = new Vector2(0f, 50f);
            area.SetSize(0f, -100f);

            // TODO: When content is added, the layout resizes
            // but the scrollbar stays fixed.
            // Even moving the scrollbar after adding doesn't work
            // because the layout gets shifted around.
            // Find a way to actually handle this.
            UIButton button = new UIButton("Add some content", 40);
            button.SetSize(0f, 100f);
            button.SetAnchor(AnchorType.BottomMiddle);
            button.SetFill(FillType.Horizontal);
            button.onClick.AddListener(() => {
                logCount++;

                Label label = new Label($"Log: {logCount}", 20);
                label.SetSize(200f, 50f);
                area.Add(label);
            });

            window.Add(area);
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
