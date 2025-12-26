using UILib;
using UILib.Behaviours;
using UILib.Components;
using UILib.Layouts;
using UIButton = UILib.Components.Button;

namespace UILibExamples {
    /**
     * <summary>
     * An example demonstrating progress bars.
     * </summary>
     */
    public class ProgressBars: Example {
        private Window window;

        public ProgressBars() {
            window = new Window("Progress Bars", 800f, 600f);
            window.SetContentLayout(LayoutType.Vertical);
            window.SetElementSpacing(20);

            // Progress bars are horizontal by default
            ProgressBar progress = new ProgressBar();
            progress.SetSize(180f, 30f);
            window.Add(progress);

            // Create a slider to update the progress bar
            Slider slider = new Slider(0f, 100f);
            slider.SetSize(150f, 10f);
            slider.onValueChanged.AddListener((float value) => {
                progress.SetProgress(value / 100f);
            });
            window.Add(slider);

            window.Add(new Area(0f, 20f));

            // You can make vertical progress bars as well
            // Either through constructors by passing `true`
            ProgressBar progressV = new ProgressBar(true);
            progressV.SetSize(180f, 30f);

            // Or by using SetVertical
            progressV.SetVertical(true);

            window.Add(progressV);

            Label progressLabel = new Label("0%", 20);
            progressLabel.SetSize(0f, 60f);
            progressLabel.SetFill(FillType.Horizontal);
            progressLabel.text.alignByGeometry = false;
            progressLabel.SetOffset(0f, 4f);
            progressV.Add(progressLabel);

            // Add a timer to control the progress bar
            // Defined below
            AddTimer(progressV, progressLabel);
        }

        private void AddTimer(ProgressBar progress, Label progressLabel) {
            float totalTime = 2f;

            Area timerArea = new Area();
            timerArea.SetContentLayout(LayoutType.Horizontal);
            timerArea.SetElementSpacing(20);
            timerArea.SetSize(400f, 40f);

            // From UILib.Behaviours
            Timer timer = timerArea.gameObject.AddComponent<Timer>();

            // Update progress
            timer.onIter.AddListener((float value) => {
                float normal = value / totalTime;
                progress.SetProgress(normal);
                progressLabel.SetText($"{(int) (100f * normal)}%");
            });

            // Button to run the timer
            UIButton timerButton = new UIButton("Run", 20);
            timerButton.SetSize(160f, 40f);
            timerButton.onClick.AddListener(() => {
                timer.StartTimer(0f, totalTime);
            });
            timerArea.Add(timerButton);

            // Button to change the direction dynamically
            UIButton changeButton = new UIButton("Change Bar", 20);
            changeButton.SetSize(160f, 40f);
            changeButton.onClick.AddListener(() => {
                progress.SetVertical(!progress.vertical);
            });
            timerArea.Add(changeButton);

            window.Add(timerArea);
        }

        public override void Toggle() {
            window.ToggleVisibility();
        }
    }
}
