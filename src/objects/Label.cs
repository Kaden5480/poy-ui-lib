using UnityEngine;

using UELayoutElement = UnityEngine.UI.LayoutElement;
using UEText = UnityEngine.UI.Text;

namespace UILib {
    public class Label : UIObject {
        private UEText text;

        public Label(string text, int fontSize) {
            this.text = gameObject.AddComponent<UEText>();
            this.text.font = Resources.gameFont;
            this.text.fontSize = fontSize;
            this.text.text = text;
            this.text.alignByGeometry = true;

            SetAlignment(TextAnchor.MiddleCenter);
        }

        public void SetAlignment(TextAnchor textAnchor) {
            this.text.alignment = textAnchor;
        }
    }
}
