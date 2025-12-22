using UnityEngine;

using UILib;
using UILib.Behaviours;
using UILib.Components;
using UILib.Layouts;
using UILib.Notifications;

namespace UILibExamples {
    /**
     * <summary>
     * An example demonstrating
     * local and global shortcuts.
     * </summary>
     */
    public class Shortcuts : Example {
        private Window window;

        public Shortcuts() {
            window = new Window("Shortcuts", 400f, 300f);
            window.SetContentLayout(LayoutType.Vertical);
            window.SetElementSpacing(20);

            Label globalLabel = new Label(
                "Pressing \"End\" toggles this overlay."
                + " This is a global shortcut", 20
            );
            globalLabel.SetSize(200f, 50f);
            window.Add(globalLabel);

            Label localLabel = new Label(
                "Pressing \"Home\" sends a notification,"
                + " but only when this window is focused."
                + " This is a local shortcut", 20
            );
            localLabel.SetSize(200f, 50f);
            window.Add(localLabel);

            // Add the local shortcut
            Shortcut localShortcut = new Shortcut(new[] { KeyCode.Home });
            localShortcut.onTrigger.AddListener(() => {
                Notifier.Notify("Shortcuts", "You triggered the local shortcut!");
            });
            window.AddShortcut(localShortcut);

            // Add a global shortcut
            Shortcut globalShortcut = new Shortcut(new[] { KeyCode.End });
            globalShortcut.onTrigger.AddListener(() => {
                Toggle();
            });
            UIRoot.AddShortcut(globalShortcut);
        }

        public override void Toggle() {
            window.ToggleVisibility();
        }
    }
}
