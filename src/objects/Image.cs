using UnityEngine;
using UEImage = UnityEngine.UI.Image;

namespace UILib {
    public class Image : UIObject {
        public UEImage image { get; private set; }

        public Image() {
            image = gameObject.AddComponent<UEImage>();
            SetAnchor(AnchorType.Middle, FillType.Fill);
        }

        public Image(Color color) : this() {
            image.color = color;
        }

        public Image(Texture2D texture) : this() {
            image.sprite = Sprite.Create(
                texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
        }
    }
}
