using UELayoutElement = UnityEngine.UI.LayoutElement;
using UEText = UnityEngine.UI.Text;

namespace UILib {
    public class Label : UIObject {
        private UELayoutElement layout;
        private UEText text;

        public Label(string text) {
            layout = gameObject.AddComponent<UELayoutElement>();
            layout.minHeight = 80f;
            layout.preferredHeight = 80f;

            this.text = gameObject.AddComponent<UEText>();
            this.text.font = Resources.gameFont;
            this.text.fontSize = 40;
            this.text.text = text;
        }
    }
}
