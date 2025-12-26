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
         * The current amount of progress (between 0 and 1).
         * </summary>
         */
        public float progress { get; private set; } = 0f;

        /**
         * <summary>
         * Whether this progress bar is vertical.
         * </summary>
         */
        public bool vertical { get; private set; } = false;

        /**
         * <summary>
         * The background for this progress bar.
         * </summary>
         */
        public Image background { get; private set; }

        /**
         * <summary>
         * The fill for this progress bar.
         * </summary>
         */
        public Image fill { get; private set; }

        /**
         * <summary>
         * Initializes the progress bar.
         * </summary>
         * <param name="vertical">Whether this progress bar should be vertical</param>
         */
        public ProgressBar(bool vertical = false) {
            background = new Image();
            background.SetFill(FillType.All);

            fill = new Image();
            fill.SetAnchor(AnchorType.BottomLeft);
            fill.SetFill(FillType.Vertical);
            fill.SetSize(0f, 0f);

            Add(background);
            Add(fill);

            // Set the theme
            SetThisTheme(theme);

            // Set vertical
            SetVertical(vertical);

            // Set to 0 progress
            SetProgress(0f);
        }

        /**
         * <summary>
         * Allows setting the theme of this progress bar.
         *
         * This handles setting the theme specifically for this component,
         * not its children. It's protected to allow overriding if you
         * were to create a subclass.
         *
         * In most cases, you'd probably want to use
         * <see cref="UIObject.SetTheme"/> instead.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {
            background.SetColor(theme.selectNormal);
            fill.SetColor(theme.selectHighlight);
        }

        /**
         * <summary>
         * Sets whether this progress bar is vertical.
         *
         * Passing `true` means it will become vertical (bottom to top).
         * Passing `false` means it will become horizontal (left to right).
         *
         * This will also immediately update the progress bar's visuals.
         * </summary>
         * <param name="vertical">Whether this progress bar is vertical</param>
         */
        public void SetVertical(bool vertical) {
            this.vertical = vertical;
            SetProgress(progress);
        }

        /**
         * <summary>
         * Sets the progress of this progress bar.
         * This must be between 0 and 1.
         * </summary>
         * <param name="progress">The value to set the progress to</param>
         */
        public void SetProgress(float progress) {
            this.progress = Mathf.Clamp(progress, 0f, 1f);

            if (vertical == true) {
                fill.rectTransform.anchorMax = new Vector2(
                    1f, this.progress
                );
            }
            else {
                fill.rectTransform.anchorMax = new Vector2(
                    this.progress, 1f
                );
            }
        }
    }
}
