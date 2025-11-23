using UnityEngine;

using UnityEngine.Events;
using UnityEngine.EventSystems;

using ColorBlock = UnityEngine.UI.ColorBlock;
using UEButton = UnityEngine.UI.Button;

namespace UILib {
    public class Button : UIObject {
        private Image background;
        private UEButton button;

        public Label label { get; private set; }
        public Image image { get; private set; }

        public Button() {
            background = new Image();
            GameObject.DestroyImmediate(background.mouseHandler);
            Add(background);

            button = gameObject.AddComponent<UEButton>();
            button.colors = Colors.colorBlock;
            button.targetGraphic = background.image;
            AddListener(() => {
                EventSystem.current.SetSelectedGameObject(null);
            });
        }

        public Button(string text, int fontSize) : this() {
            label = new Label(text, fontSize);
            GameObject.DestroyImmediate(label.mouseHandler);
            label.Fill();
            Add(label);
        }

        public Button(Texture2D texture) : this() {
            image = new Image(texture);
            GameObject.DestroyImmediate(image.mouseHandler);
            image.Fill();
            Add(image);
        }

        public void SetColorBlock(ColorBlock colorBlock) {
            button.colors = colorBlock;
        }

        public void AddListener(UnityAction callback) {
            button.onClick.AddListener(callback);
        }
    }
}
