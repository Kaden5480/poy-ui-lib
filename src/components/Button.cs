using UnityEngine;

using UnityEngine.Events;
using UnityEngine.EventSystems;

using ColorBlock = UnityEngine.UI.ColorBlock;
using UEButton = UnityEngine.UI.Button;

using UILib.Layouts;

namespace UILib.Components {
    /**
     * <summary>
     * A button component.
     *
     * Buttons can have textures or text on them
     * and handle click events.
     * </summary>
     */
    public class Button : UIComponent {
        /**
         * <summary>
         * The underlying Unity `Button`.
         * </summary>
         */
        public UEButton button { get; private set; }

        private Image background;

        /**
         * <summary>
         * The label attached to this button, if any.
         * </summary>
         */
        public Label label { get; private set; }

        /**
         * <summary>
         * The image attached to this button, if any.
         * </summary>
         */
        public Image image { get; private set; }

        public override UnityEvent onClick {
            get => (button == null) ? base.onClick : button.onClick;
        }

        /**
         * <summary>
         * Initializes a button.
         * </summary>
         */
        public Button() {
            background = new Image();
            background.SetFill(FillType.All);
            background.DestroyMouseHandler();
            Add(background);

            button = gameObject.AddComponent<UEButton>();
            button.targetGraphic = background.image;
            button.onClick.AddListener(() => {
                EventSystem.current.SetSelectedGameObject(null);
            });

            // Apply the theme
            SetThisTheme(theme);
        }

        /**
         * <summary>
         * Initializes a button with the specified text.
         * </summary>
         * <param name="text">The text to add to this button</param>
         * <param name="fontSize">The font size for the text</param>
         */
        public Button(string text, int fontSize) : this() {
            AddLabel(text, fontSize);
        }

        /**
         * <summary>
         * Initializes a button with a texture.
         * </summary>
         * <param name="texture">The texture to add to this button</param>
         */
        public Button(Texture2D texture) : this() {
            AddImage(texture);
        }

        /**
         * <summary>
         * Adds a label to this button with the given text.
         * </summary>
         * <param name="text">The text to add</param>
         * <param name="fontSize">The size of the font to use</param>
         */
        internal void AddLabel(string text, int fontSize) {
            label = new Label(text, fontSize);
            label.DestroyMouseHandler();
            label.SetFill(FillType.All);
            Add(label);
        }

        /**
         * <summary>
         * Adds an image to this button.
         * </summary>
         * <param name="texture">The texture to add</param>
         */
        internal void AddImage(Texture2D texture) {
            image = new Image(texture);
            image.SetFill(FillType.All);
            image.DestroyMouseHandler();
            Add(image);
        }

        /**
         * <summary>
         * Allows setting the theme of this button
         * and all children.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {
            button.colors = theme.blockSelect;
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
         * Sets the texture on this button.
         * </summary>
         */
        public void SetTexture(Texture2D texture) {
            if (image == null) {
                return;
            }

            image.SetTexture(texture);
        }

        /**
         * <summary>
         * Sets the text on this button.
         * </summary>
         * <param name="text">The text to set</param>
         */
        public void SetText(string text) {
            if (label == null) {
                return;
            }

            label.SetText(text);
        }
    }
}
