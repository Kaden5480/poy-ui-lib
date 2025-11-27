using UnityEngine;

using UILib.Layouts;

namespace UILib.Components {
    /**
     * <summary>
     * A simple progress bar.
     * Provides a simple visual representation of progress.
     * </summary>
     */
    public class ProgressBar : UIComponent {
        /**
         * <summary>
         * The amount of progress (between 0 and 1).
         * </summary>
         */
        public float progress { get; private set; } = 0f;

        private Image background;
        private Image fill;

        /**
         * <summary>
         * Initializes the progress bar.
         * </summary>
         */
        public ProgressBar() {
            background = new Image();
            background.SetFill(FillType.All);

            fill = new Image();
            fill.SetAnchor(AnchorType.BottomLeft);
            fill.SetFill(FillType.Vertical);
            fill.SetSize(0f, 0f);

            Add(background);
            Add(fill);

            // Set the theme
            SetTheme(theme);
        }

        /**
         * <summary>
         * Allows setting the theme of this progress bar
         * and all children.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        public override void SetTheme(Theme theme) {
            base.SetTheme(theme);

            background.SetColor(theme.selectNormal);
            fill.SetColor(theme.selectHighlight);
        }

        /**
         * <summary>
         * Sets the progress of this progress bar.
         * This must be between 0 and 1.
         * </summary>
         * <param name="value">The value to set the progress to</param>
         */
        public void SetProgress(float value) {
            progress = Mathf.Clamp(value, 0f, 1f);
            fill.rectTransform.anchorMax = new Vector2(
                progress, 1f
            );
        }
    }
}
