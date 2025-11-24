using UnityEngine;

using UnityEngine.Events;
using UnityEngine.EventSystems;

using ColorBlock = UnityEngine.UI.ColorBlock;
using UEButton = UnityEngine.UI.Button;

using UILib.Layout;

namespace UILib.Components {
    /**
     * <summary>
     * A button object.
     *
     * Buttons can have textures or text on them
     * and handle click events.
     * </summary>
     */
    public class Button : UIComponent {
        private Image background;
        private UEButton button;

        public Label label { get; private set; }
        public Image image { get; private set; }

        /**
         * <summary>
         * Initializes a Button.
         * </summary>
         */
        public Button() {
            background = new Image();
            background.DestroyMouseHandler();
            Add(background);

            button = gameObject.AddComponent<UEButton>();
            button.colors = Colors.colorBlock;
            button.targetGraphic = background.image;
            AddListener(() => {
                EventSystem.current.SetSelectedGameObject(null);
            });
        }

        /**
         * <summary>
         * Initializes a Button with the specified text.
         * </summary>
         * <param name="text">The text to add to this button</param>
         * <param name="fontSize">The font size for the text</param>
         */
        public Button(string text, int fontSize) : this() {
            label = new Label(text, fontSize);
            label.DestroyMouseHandler();
            label.SetFill(FillType.All);
            Add(label);
        }

        /**
         * <summary>
         * Initializes a Button with a texture.
         * </summary>
         * <param name="texture">The texture to add to this button</param>
         */
        public Button(Texture2D texture) : this() {
            image = new Image(texture);
            image.DestroyMouseHandler();
            Add(image);
        }

        /**
         * <summary>
         * Allows changing the background image of the button.
         * </summary>
         * <param name="texture">The texture to use instead</param>
         */
        public void SetBackground(Texture2D texture) {
            background.SetTexture(texture);
        }

        /**
         * <summary>
         * Sets the color block of this button.
         * </summary>
         * <param name="colorBlock">The color block to set</param>
         */
        public void SetColorBlock(ColorBlock colorBlock) {
            button.colors = colorBlock;
        }

        /**
         * <summary>
         * Adds a callback to this button.
         * The callback will be called whenever the button is clicked.
         * </summary>
         * <param name="callback">The callback to add</param>
         */
        public void AddListener(UnityAction callback) {
            button.onClick.AddListener(callback);
        }
    }
}
