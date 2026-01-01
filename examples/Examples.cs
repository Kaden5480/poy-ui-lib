using UnityEngine;

using UILib;
using UILib.Components;
using UILib.Layouts;

using UIButton = UILib.Components.Button;

namespace UILibExamples {
    /**
     * <summary>
     * This is the main examples Window
     * which allows you to show/hide different examples.
     * </summary>
     */
    public class Examples {
        private Window window;

        public Examples() {
            window = new Window("UILib Examples", 300f, 550f);
            window.SetMinSize(300f, 200f);
            window.SetContentLayout(LayoutType.Vertical);
            window.SetContentPadding(10);
            window.SetElementSpacing(10f);

            // See Plugin.cs
            //
            // The code below is pretty bad for optimisation
            // as a lot of UIs are going to be created all at once
            //
            // It's better to try to delay the creation of UIs until
            // the first time it's needed

            // Add examples
            AddExample(new BasicWindow());
            AddExample(new Chat());
            AddExample(new Inputs());
            AddExample(new ProgressBars());
            AddExample(new UILibExamples.Layouts());
            AddExample(new Shortcuts());
            AddExample(new Locks());
            AddExample(new Themes());
            AddExample(new Animate());

            // Global shortcut for toggling UI
            Shortcut shortcut = new Shortcut(new[] { KeyCode.Tab });
            shortcut.onTrigger.AddListener(() => {
                window.ToggleVisibility();
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
            window.Add(button);
        }
    }
}
