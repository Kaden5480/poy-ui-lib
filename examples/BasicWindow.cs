using UILib;
using UILib.Components;
using UILib.Notifications;

// This is required, since Peaks of Yore
// has a `Button` type
using UIButton = UILib.Components.Button;

namespace UILibExamples {
    /**
     * <summary>
     * A simple example of a window
     * with a single button in the middle.
     * </summary>
     */
    public class BasicWindow {
        private Window window;

        public BasicWindow() {
            // Create the window, make it 400x300
            // and give it a name
            window = new Window("Basic", 400f, 300f);

            // Create a button with the text "Click me!"
            // and a font size of 20
            UIButton button = new UIButton("Click me!", 20);

            // Set the size of the button so it actually displays
            // 100 is the width, 40 is the height
            button.SetSize(100f, 40f);

            // Add a listener to send a notification
            // when the button is clicked
            button.onClick.AddListener(() => {
                Notifier.Notify("Button Clicked", "You clicked the button!");
            });

            // And then just add it to the window
            // By default, all components are anchored
            // to the middle of the container
            window.Add(button);

            // Hide the window, show it later on
            window.Hide();
        }

        public void Show() {
            window.Show();
        }
    }
}
