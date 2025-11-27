using UnityEngine;
using UEImage = UnityEngine.UI.Image;

using UILib.Layouts;

namespace UILib.Components {
    /**
     * <summary>
     * A component which just displays an image.
     * </summary>
     */
    public class Image : UIComponent {
        public UEImage image { get; private set; }

        /**
         * <summary>
         * Initializes an image.
         * </summary>
         */
        public Image() {
            image = gameObject.AddComponent<UEImage>();
        }

        /**
         * <summary>
         * Initializes an image with the specified color.
         * </summary>
         * <param name="color">The color to set on the image</param>
         */
        public Image(Color color) : this() {
            SetColor(color);
        }

        /**
         * <summary>
         * Initializes an image with the specified texture.
         * </summary>
         * <param name="texture">The texture to display</param>
         */
        public Image(Texture2D texture) : this() {
            SetTexture(texture);
        }

        /**
         * <summary>
         * Changes the color of this image.
         * </summary>
         * <param name="color">The color to use</param>
         */
        public void SetColor(Color color) {
            image.color = color;
        }

        /**
         * <summary>
         * Changes this image to use a different texture.
         * </summary>
         * <param name="texture">The texture to apply</param>
         */
        public void SetTexture(Texture2D texture) {
            image.sprite = Sprite.Create(
                texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
        }
    }
}
