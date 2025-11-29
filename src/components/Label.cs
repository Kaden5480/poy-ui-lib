using UnityEngine;

using UELayoutElement = UnityEngine.UI.LayoutElement;
using UEText = UnityEngine.UI.Text;

namespace UILib.Components {
    /**
     * <summary>
     * A component which just displays some text.
     * </summary>
     */
    public class Label : UIComponent {
        /**
         * <summary>
         * The underlying Unity `Text`.
         * </summary>
         */
        public UEText text { get; private set; }

        // The configured font size
        private int fontSize;

        /**
         * <summary>
         * Initializes a label.
         * </summary>
         * <param name="text">The text to display</param>
         * <param name="fontSize">The size of the font</param>
         */
        public Label(string text, int fontSize) {
            this.text = gameObject.AddComponent<UEText>();
            this.fontSize = fontSize;

            this.text.text = text;
            this.text.alignByGeometry = true;

            SetAlignment(TextAnchor.MiddleCenter);

            // Apply the theme
            SetTheme(theme);
        }

        /**
         * <summary>
         * Allows setting the theme of this label
         * and all children.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        public override void SetTheme(Theme theme) {
            base.SetTheme(theme);

            this.text.color = theme.foreground;
            this.text.font = theme.font;
            this.text.fontSize = (int) (theme.fontScaler * fontSize);
            this.text.lineSpacing = theme.fontLineSpacing;
        }

        /**
         * <summary>
         * Sets the alignment for the text.
         * </summary>
         * <param name="alignment">The alignment to use</param>
         */
        public void SetAlignment(TextAnchor alignment) {
            this.text.alignment = alignment;
        }

        /**
         * <summary>
         * Sets the text on this label.
         * </summary>
         * <param name="text">The text to set</param>
         */
        public void SetText(string text) {
            this.text.text = text;
        }
    }
}
