using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;
using UnityEngine.EventSystems;
using UEButton = UnityEngine.UI.Button;
using UEImage = UnityEngine.UI.Image;

namespace UILib {
    public class Button : UIObject {
        private UEButton button;
        private UEImage image;

        public Button(string text, int fontSize) {
            button = gameObject.AddComponent<UEButton>();
            image = gameObject.AddComponent<UEImage>();

            button.colors = Colors.colorBlock;
            button.targetGraphic = image;

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
