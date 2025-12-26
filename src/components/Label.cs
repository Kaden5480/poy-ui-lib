using UnityEngine;
using UELayoutElement = UnityEngine.UI.LayoutElement;
using UEText = UnityEngine.UI.Text;

using UILib.Layouts;

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

            SetAlignment(AnchorType.Middle);

            // Apply the theme
            SetThisTheme(theme);
        }

        /**
         * <summary>
         * Allows setting the theme of this label.
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
            text.color = theme.foreground;
            text.font = theme.font;
            text.fontSize = (int) (theme.fontScaler * fontSize);
            text.lineSpacing = theme.fontLineSpacing;
        }

        /**
         * <summary>
         * Sets the alignment for the text.
         * </summary>
         * <param name="alignment">The alignment to use</param>
         */
        public void SetAlignment(AnchorType alignment) {
            text.alignment = AnchorToText(alignment);
        }

        /**
         * <summary>
         * Set the color of the text on this label.
         * </summary>
         * <param name="color">The color to set</param>
         */
        public void SetColor(Color color) {
            text.color = color;
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
