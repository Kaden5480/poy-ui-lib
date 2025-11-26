using UnityEngine;

using Direction = UnityEngine.UI.Slider.Direction;

using UILib.Components;
using UILib.Layout;
using UILib.Patches;
using UILib.Notifications;

using UIButton = UILib.Components.Button;

namespace UILib {
    /**
     * <summary>
     * A temporary class I'm using for testing UILib.
     * </summary>
     */
    internal static class Examples {
        private const string bigTitle = "Here's a really long title"
            + " to test how notifications handle really large titles."
            + " This doesn't need to be as large as the message though.";

        private const string bigMessage = "Here's a giant string which"
            + " eventually is going to have more than 256 characters."
            + " This is to make sure that notifications can handle"
            + " exceedingly large notification messages."
            + " Currently the message is a bit over half of it's total"
            + " intended length, so there's still some more to go."
            + " But now the notificaton message should be fine";

        private static int logCount = 0;

        private static Window window;
        private static Window window2;
        private static Overlay overlay;

        internal static void Awake() {
            window = MakeLog();
            window.SetAnchor(AnchorType.TopLeft);

            window2 = MakeWindow("Cool Window", 800f, 600f);
            window2.SetAnchor(AnchorType.Middle);

            overlay = MakeOverlay(400f, 400f);
            overlay.SetPauseMode(false);
            overlay.SetAnchor(AnchorType.MiddleLeft);
        }

        internal static void Start() {
            //window.Show();
            window2.Show();
            overlay.Show();
        }

        internal static void Update() {
            if (Input.GetKeyDown(KeyCode.Home) == true) {
                Notifier.Notify(
                    "UILib", "One of your mods broke!", NotificationType.Error
                );
            }
            else if (Input.GetKeyDown(KeyCode.End) == true) {
                Notifier.Notify("UILib", "Informational notification");
            }
            else if (Input.GetKeyDown(KeyCode.PageUp) == true) {
                window2.ToggleVisibility();
            }
            else if (Input.GetKeyDown(KeyCode.PageDown) == true) {
                overlay.ToggleVisibility();
            }
        }

        private static Overlay MakeOverlay(float width, float height) {
            Overlay overlay = new Overlay(width, height);
            Image background = new Image(Colors.black);
            background.SetFill(FillType.All);
            overlay.Add(background);

            Area area = new Area();
            area.SetContentLayout(LayoutType.Vertical);
            overlay.Add(area);

            Label label = new Label("Cool Stuff", 40);
            label.SetSize(200f, 100f);
            area.Add(label);

            overlay.Hide();

            return overlay;
        }

        private static Window MakeWindow(string name, float width, float height) {
            Window window = new Window(name, width, height);
            window.SetContentLayout(LayoutType.Vertical);
            window.SetContentPadding(top: 20, bottom: 20);
            window.SetElementSpacing(20);

            Label titleLabel = new Label("Title", 40);
            titleLabel.SetSize(200f, 100f);
            window.Add(titleLabel);

            UIButton notif = new UIButton("Normal sound", 20);
            notif.SetSize(200f, 40f);
            notif.onClick.AddListener(() => { Audio.PlayNormal(); });
            window.Add(notif);

            UIButton notifError = new UIButton("Error Sound", 20);
            notifError.SetSize(200f, 40f);
            notifError.onClick.AddListener(() => { Audio.PlayError(); });
            window.Add(notifError);

            UIButton notifBig = new UIButton("Send a big notification", 20);
            notifBig.SetSize(200f, 40f);
            notifBig.onClick.AddListener(() => { Notifier.Notify(bigTitle, bigMessage); });
            window.Add(notifBig);

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
                    Notifier.Notify($"UILib {name}", $"Button {current} was pressed");
                });
                area.Add(button);

                window.Add(area);
            }

            window.Hide();

            return window;
        }

        private static Window MakeLog() {
            Window window = new Window("Log Example", 800f, 600f);

            QueueArea area = new QueueArea(20);
            area.SetContentLayout(LayoutType.Vertical);
            area.SetContentPadding(bottom: 100);
            area.SetFill(FillType.All);

            // TODO: When content is added, the layout resizes
            // but the scrollbar stays fixed.
            // Even moving the scrollbar after adding doesn't work
            // because the layout gets shifted around.
            // Find a way to actually handle this.
            UIButton button = new UIButton("Add some content", 40);
            button.SetFill(FillType.Horizontal);
            button.SetAnchor(AnchorType.BottomLeft);
            button.SetSize(-20f, 100f);
            button.SetOffset(-10f, 0f);
            button.onClick.AddListener(() => {
                logCount++;

                Label label = new Label($"Log: {logCount}", 20);
                label.SetSize(200f, 50f);
                area.Add(label);
            });

            window.Add(area);

            // Add the button directly to the window
            // instead of to the scrollview
            window.AddDirect(button);

            // Tell the scrollview to scroll over the
            // queue area instead
            window.scrollView.SetContent(area);

            window.Hide();
            return window;
        }
    }
}
