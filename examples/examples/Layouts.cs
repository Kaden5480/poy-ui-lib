using UILib;
using UILib.Components;
using UILib.Layouts;
using UILib.Notifications;

using UIButton = UILib.Components.Button;

namespace UILibExamples {
    /**
     * <summary>
     * An example demonstrating
     * layouts.
     * </summary>
     */
    public class Layouts : Example {
        private Window window;

        private float height = 40f;
        private float width = 200f;
        private int fontSize = 20;

        public Layouts() {
            window = new Window("Layouts", 800f, 600f);

            // Setting a vertical layout on this window
            // means any components added to it will
            // be displayed vertically automatically
            window.SetContentLayout(LayoutType.Vertical);

            // This sets the spacing between the elements/components
            window.SetElementSpacing(30);

            Label label = new Label("A Label", fontSize);
            label.SetSize(width, height);
            window.Add(label);

            UIButton button = new UIButton("A Button", fontSize);
            button.SetSize(width, height);
            button.onClick.AddListener(() => {
                Notifier.Notify("Layouts", "Button was clicked");
            });
            window.Add(button);

            // Sometimes you want to combine layouts though, like
            // having some content side by side
            // One way you could do this is by creating an Area
            // with a Horizontal layout
            Area area = new Area();
            area.SetSize(width*2 + 20, height);
            area.SetContentLayout(LayoutType.Horizontal);
            area.SetElementSpacing(20);

            // Then add some components to it
            Label toggleLabel = new Label("Label for the toggle", fontSize);
            toggleLabel.SetSize(width, height);
            area.Add(toggleLabel);

            Toggle toggle = new Toggle();
            toggle.SetSize(40f, 40f);
            area.Add(toggle);

            // Then add the area to the window
            window.Add(area);
        }

        public override void Toggle() {
            window.ToggleVisibility();
        }
    }
}
