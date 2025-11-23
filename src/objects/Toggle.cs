using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using UEButton = UnityEngine.UI.Button;
using UEImage = UnityEngine.UI.Image;

namespace UILib {
    public class Toggle : UIObject {
        public UEImage background    { get; private set; }
        public UEButton button       { get; private set; }

        public Background check      { get; private set; }

        private bool _isChecked = false;
        public bool isChecked {
            get => _isChecked;
            set => SetEnabled(value);
        }

        public Toggle() {
            background = gameObject.AddComponent<UEImage>();
            button = gameObject.AddComponent<UEButton>();
            button.colors = Colors.colorBlock;
            button.targetGraphic = background;

            check = new Background();
            Add(check);
            check.Hide();
            GameObject.DestroyImmediate(check.mouseHandler);
            check.image.sprite = Sprite.Create(
                Resources.checkMark, new Rect(
                    0, 0,
                    Resources.checkMark.width,
                    Resources.checkMark.height
                ),
                new Vector2(0.5f, 0.5f)
            );

            AddListener(() => {
                isChecked = !isChecked;
            });
        }

        private void SetEnabled(bool isChecked) {
            _isChecked = isChecked;

            if (isChecked == true) {
                check.Show();
            }
            else {
                check.Hide();
            }
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
