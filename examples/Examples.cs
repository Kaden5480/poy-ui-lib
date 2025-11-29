using UnityEngine;

using UILib;
using UILib.Components;
using UILib.Layouts;

using UIButton = UILib.Components.Button;

namespace UILibExamples {
    /**
     * <summary>
     * This is the main examples Overlay
     * which allows you to show/hide different examples.
     * </summary>
     */
    public class Examples {
        private Overlay overlay;
        private Area area;

        private BasicWindow basic;
        private Inputs inputs;
        private UILibExamples.Layouts layouts;
        private PauseHandles pauseHandles;
        private Themes themes;

        public Examples() {
            basic = new BasicWindow();
            inputs = new Inputs();
            layouts = new UILibExamples.Layouts();
            pauseHandles = new PauseHandles();
            themes = new Themes();

            overlay = new Overlay(300f, 400f);
            // Overlays don't have a background by default
            Image background = new Image(Color.black);
            background.SetFill(FillType.All);
            overlay.Add(background);

            area = new Area();
            area.SetContentLayout(LayoutType.Vertical);
            area.SetElementSpacing(20);
            overlay.Add(area);

            // Add examples
            AddExample(basic);
            AddExample(inputs);
            AddExample(layouts);
            AddExample(pauseHandles);
            AddExample(themes);
        }

        /**
         * <summary>
         * Adds examples.
         * </summary>
         */
        public void AddExample(Example example) {
            UIButton button = new UIButton($"{example.GetType().Name}", 20);
            button.SetSize(200f, 40f);
            button.onClick.AddListener(() => {
                example.Toggle();
            });
            area.Add(button);
        }

        public void Update() {
            pauseHandles.Update();

            if (Input.GetKeyDown(KeyCode.Tab) == true) {
                overlay.ToggleVisibility();
            }
        }

        public void Show() {
            overlay.Show();
        }
    }
}
