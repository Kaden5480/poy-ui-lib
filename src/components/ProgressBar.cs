using UnityEngine;

using UILib.Layout;

namespace UILib.Components {
    public class ProgressBar : UIComponent {
        public float progress { get; private set; } = 0f;

        private Image background;
        private Image fill;

        /**
         * <summary>
         * Initializes the progress bar.
         * </summary>
         */
        public ProgressBar() {
            background = new Image(Colors.grey);
            background.SetFill(FillType.All);

            fill = new Image(Colors.lightGrey);
            fill.SetAnchor(AnchorType.BottomLeft);
            fill.SetFill(FillType.Vertical);
            fill.SetSize(0f, 0f);

            Add(background);
            Add(fill);
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
