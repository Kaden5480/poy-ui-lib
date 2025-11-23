using UnityEngine;
using UEImage = UnityEngine.UI.Image;

namespace UILib {
    /**
     * <summary>
     * An object which just displays an image.
     * </summary>
     */
    public class Image : UIObject {
        public UEImage image { get; private set; }

        /**
         * <summary>
         * Initializes an image.
         * </summary>
         */
        public Image() {
            image = gameObject.AddComponent<UEImage>();
            Fill();
        }

        /**
         * <summary>
         * Initializes an image with the specified color.
         * </summary>
         * <param name="color">The color to set on the image</param>
         */
        public Image(Color color) : this() {
            image.color = color;
        }

        /**
         * <summary>
         * Initializes an image with the specified texture.
         * </summary>
         * <param name="texture">The texture to display</param>
         */
        public Image(Texture2D texture) : this() {
            image.sprite = Sprite.Create(
                texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
        }
    }
}
