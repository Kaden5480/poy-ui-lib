using UnityEngine;

using UILib;
using UILib.Components;
using UILib.Layouts;

using UIButton = UILib.Components.Button;

namespace UILibExamples {
    /**
     * <summary>
     * An example demonstrating
     * how themes work.
     * </summary>
     */
    public class Themes : Example {
        private Window window;

        private float height = 40f;
        private float width = 200f;
        private int fontSize = 20;

        public Themes() {
            // Create a new theme from the current one
            Theme theme = Theme.GetTheme();

            // Override some of the theme
            // There are also some nice methods for making Colors
            theme.background         = Colors.RGB(100f, 40f, 40f);
            theme.foreground         = Colors.HSL(180f, 50f, 70f);
            theme.selectNormal       = Colors.HSV(240f, 30f, 50f);
            theme.selectHighlight    = Colors.Hex(0xffaaee);
            theme.selectFadeTime     = 2f;
            theme.overlayFadeTime    = 0.5f;
            theme.windowOpacity      = 0.9f;

            window = new Window("Themes", 800f, 600f);
            window.SetContentLayout(LayoutType.Vertical);
            window.SetElementSpacing(20);

            Label label = new Label("A Label", fontSize);
            label.SetSize(width, height);
            window.Add(label);

            UIButton button = new UIButton("A Button", fontSize);
            button.SetSize(width, height);
            window.Add(button);

            // Setting the theme here will only cause the
            // elements already added to inherit the theme
            window.SetTheme(theme);

            // That means this slider will be the default theme
            Slider slider = new Slider();
            slider.SetSize(width, 10f);
            window.Add(slider);

            // You could forcefully apply the theme though
            // by passing `true` as a second argument to `Add`
            Toggle toggle = new Toggle();
            toggle.SetSize(40f, 40f);
            window.Add(toggle, true);

            // Similarly if you really want to change something
            // from earlier on, you can do so and ignore the theme
            label.SetColor(Color.red);
        }

        public override void Toggle() {
            window.ToggleVisibility();
        }
    }
}
