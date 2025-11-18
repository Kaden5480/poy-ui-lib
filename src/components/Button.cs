using UELayoutElement = UnityEngine.UI.LayoutElement;
using UEButton = UnityEngine.UI.Button;

namespace UILib {
    public class Button : BaseComponent {
        private UELayoutElement layout;
        private UEButton button;

        public Button(string text) {
            layout = root.AddComponent<UELayoutElement>();
            layout.minHeight = 80f;
            layout.preferredHeight = 80f;

            button = root.AddComponent<UEButton>();

            AddChild(new Label(text));
        }
    }
}
