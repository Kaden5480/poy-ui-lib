using UnityEngine;

using Direction = UnityEngine.UI.Slider.Direction;

using UILib.Behaviours;
using UILib.Components;
using UILib.Layouts;
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

        private static Timer timer;

        private static Theme customTheme = new Theme() {
            foreground              = Theme.RGB(240, 0,   0),
            background              = Theme.RGB(0,   240, 0),
            accent                  = Theme.RGB(0,   0,   240),
            selectNormal            = Theme.RGB(0,   120, 120),
            selectHighlight         = Theme.RGB(20,  20,  90),
            selectAltNormal         = Theme.RGB(50,  60,  70),
            selectAltHighlight      = Theme.RGB(60,  40,  30),
            importantNormal         = Theme.RGB(20,  100, 90),
            importantHighlight      = Theme.RGB(43,  1,   87),
            selectFadeTime          = 3f,
            notificationMaxTitle    = 10,
            notificationMaxMessage  = 20,
            notificationVolume      = 0.1f,
            notificationErrorVolume = 0.1f,
            notificationWaitTime    = 10f,
            notificationFadeTime    = 2f,
        };

        internal static void Awake() {
            window = MakeLog();
            window.SetAnchor(AnchorType.TopLeft);

            window2 = MakeWindow("Cool Window", 800f, 600f);
            window2.SetAnchor(AnchorType.Middle);

            overlay = MakeOverlay(400f, 400f);
            overlay.SetAutoPause(false);
            overlay.SetAnchor(AnchorType.Middle);

            UIButton customNotif = new UIButton("Send a custom notification", 20);
            customNotif.SetSize(200f, 40f);
            customNotif.onClick.AddListener(() => {
                Notifier.Notify(bigTitle, bigMessage, theme: customTheme);
            });

            KeyCodeField readInput = new KeyCodeField(KeyCode.A, 20);
            readInput.SetSize(200f, 40f);

            window2.Add(customNotif);
            window2.Add(readInput);
        }

        internal static void Start() {
            //window.Show();
            window2.Show();
            //overlay.Show();
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
                window2.SetInteractable(!window2.canInteract);
            }
        }

        private static Overlay MakeOverlay(float width, float height) {
            float totalTime = 3f;

            Overlay overlay = new Overlay(width, height);
            timer = overlay.gameObject.AddComponent<Timer>();

            Image background = new Image(Color.black);
            background.SetFill(FillType.All);
            overlay.Add(background);

            Area area = new Area();
            area.SetContentLayout(LayoutType.Vertical);
            area.SetElementSpacing(20);
            overlay.Add(area);

            Label label = new Label("Cool Stuff", 40);
            label.SetSize(200f, 40f);
            area.Add(label);

            ProgressBar progress = new ProgressBar();
            progress.SetSize(200f, 20f);
            area.Add(progress);

            Label passedProgress = new Label("0%", 20);
            passedProgress.SetSize(200f, 20f);
            area.Add(passedProgress);

            timer.onIter.AddListener((float value) => {
                float percent = 1f - (value / totalTime);
                progress.SetProgress(percent);
                passedProgress.SetText($"{(int) (100 * percent)}%");
            });

            // Done text to show when finished
            Label done = new Label("Done!", 20);
            done.SetSize(200f, 20f);
            done.Hide();
            area.Add(done);

            timer.onStart.AddListener(() => {
                done.Hide();
            });
            timer.onEnd.AddListener(() => {
                done.Show();
            });

            // Button area for interacting
            Area buttonArea = new Area();
            buttonArea.SetContentLayout(LayoutType.Horizontal);
            buttonArea.SetElementSpacing(20);
            buttonArea.SetSize(300f, 40f);
            area.Add(buttonArea);

            UIButton runProgress = new UIButton("Start", 20);
            runProgress.onClick.AddListener(() => {
                timer.StartTimer(totalTime);
            });
            runProgress.SetSize(100f, 40f);
            buttonArea.Add(runProgress);

            UIButton pauseProgress = new UIButton("Pause", 20);
            pauseProgress.SetSize(100f, 40f);
            pauseProgress.onClick.AddListener(() => {
                timer.PauseTimer(!timer.paused);

                if (timer.paused == true) {
                    pauseProgress.SetText("Unpause");
                }
                else {
                    pauseProgress.SetText("Pause");
                }
            });
            buttonArea.Add(pauseProgress);

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
