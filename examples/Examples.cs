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
        private Chat chat;
        private Inputs inputs;
        private UILibExamples.Layouts layouts;
        private Shortcuts shortcuts;
        private Locks locks;
        private Themes themes;

        public Examples() {
            basic = new BasicWindow();
            chat = new Chat();
            inputs = new Inputs();
            layouts = new UILibExamples.Layouts();
            shortcuts = new Shortcuts();
            locks = new Locks();
            themes = new Themes();

            overlay = new Overlay(300f, 450f);
            // Overlays don't have a background by default
            Image background = new Image(Theme.GetTheme().background);
            background.SetFill(FillType.All);
            overlay.Add(background);

            area = new Area();
            area.SetContentLayout(LayoutType.Vertical);
            area.SetElementSpacing(20);
            overlay.Add(area);

            // Add examples
            AddExample(basic);
            AddExample(chat);
            AddExample(inputs);
            AddExample(layouts);
            AddExample(shortcuts);
            AddExample(locks);
            AddExample(themes);

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
