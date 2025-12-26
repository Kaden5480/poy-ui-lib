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

        public Examples() {
            overlay = new Overlay(300f, 500f);
            // Overlays don't have a background by default
            Image background = new Image(Theme.GetTheme().background);
            background.SetFill(FillType.All);
            overlay.Add(background);

            area = new Area();
            area.SetContentLayout(LayoutType.Vertical);
            area.SetElementSpacing(20);
            overlay.Add(area);

            // Add examples
            AddExample(new BasicWindow());
            AddExample(new Chat());
            AddExample(new Inputs());
            AddExample(new ProgressBars());
            AddExample(new UILibExamples.Layouts());
            AddExample(new Shortcuts());
            AddExample(new Locks());
            AddExample(new Themes());

            // Global shortcut for toggling UI
            Shortcut shortcut = new Shortcut(new[] { KeyCode.Tab });
            shortcut.onTrigger.AddListener(() => {
                overlay.ToggleVisibility();
            });
            UIRoot.AddShortcut(shortcut);
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
    }
}
