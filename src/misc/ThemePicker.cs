using System;

using UILib.Components;
using UILib.Layouts;
using UIButton = UILib.Components.Button;

namespace UILib.Misc {
    /**
     * <summary>
     * A window which is used for displaying theme choices,
     * and allows a user to customise their selected theme
     * in a more interactive way.
     * </summary>
     */
    internal class ThemePicker : Window {
        private Label extraLabel;

        /**
         * <summary>
         * Initializes the theme picker.
         * </summary>
         */
        internal ThemePicker() : base("Theme Picker", 800f, 600f) {
            Area area = new Area();
            area.SetAnchor(AnchorType.TopMiddle);
            area.SetElementAlignment(AnchorType.TopMiddle);
            area.SetContentLayout(LayoutType.Vertical);
            area.SetContentPadding(20);
            area.SetElementSpacing(20f);

            Label mainLabel = new Label(
                "This UI allows you to customise your currently selected theme.",
                26
            );
            mainLabel.SetSize(500f, 0f);
            mainLabel.SetFill(FillType.Vertical);
            area.Add(mainLabel);

            extraLabel = new Label(
                "Below are buttons you can press to preview different themes."
                + " These will only update this UI, if you want the theme to apply"
                + " as the default across UILib, you will need to restart the game.",
                20
            );
            extraLabel.SetSize(500f, 0f);
            extraLabel.SetFill(FillType.Vertical);
            area.Add(extraLabel);

            foreach (Theme theme in Theme.GetThemes().Values) {
                UIButton button = new UIButton($"{theme.name}", 20);
                button.SetSize(200f, 40f);
                button.onClick.AddListener(() => {
                    UpdateTheme(theme);
                    Config.selectedTheme.Value = theme.name;
                });

                area.Add(button);
            }

            Add(area);

            UpdateTheme(Theme.GetTheme());
        }

        /**
         * <summary>
         * Updates the selected theme and the previewed one.
         * </summary>
         * <param name="theme">The theme to set</param>
         */
        internal void UpdateTheme(Theme theme) {
            SetTheme(theme);
            extraLabel.SetColor(theme.selectAltHighlight);
        }
    }
}
