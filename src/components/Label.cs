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
        public UEText text { get; private set; }

        /**
         * <summary>
         * Initializes a label.
         * </summary>
         * <param name="text">The text to display</param>
         * <param name="fontSize">The size of the font</param>
         */
        public Label(string text, int fontSize) {
            this.text = gameObject.AddComponent<UEText>();
            this.text.font = Resources.gameFont;
            this.text.fontSize = fontSize;
            this.text.text = text;
            this.text.lineSpacing = 3f;
            this.text.alignByGeometry = true;

            SetAlignment(TextAnchor.MiddleCenter);
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
