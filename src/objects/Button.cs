using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;
using UnityEngine.EventSystems;
using UEButton = UnityEngine.UI.Button;
using UEImage = UnityEngine.UI.Image;

namespace UILib {
    public class Button : UIObject {
        private Background background;

        private UEButton button;

        public Button(string text, int fontSize) {
            background = new Background();
            Add(background);

            button = gameObject.AddComponent<UEButton>();
            button.colors = Colors.colorBlock;
            button.targetGraphic = background.image;

            Label label = new Label(text, fontSize);
            GameObject.DestroyImmediate(label.clickHandler);
            label.Fill();
            Add(label);
        }

        public void SetColorBlock(ColorBlock colorBlock) {
            button.colors = colorBlock;
        }

        public void AddListener(UnityAction callback) {
            logger.LogDebug("Added listener");
            button.onClick.AddListener(callback);
        }
    }
}
