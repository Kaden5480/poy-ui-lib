using UnityEngine;

using UILib.Behaviours;
using UILib.Layouts;
using UIButton = UILib.Components.Button;

namespace UILib.Components {
    /**
     * <summary>
     * </summary>
     */
    public class ColorField : UIButton {
        private Image preview;

        /**
         * <summary>
         * The current value of the color field.
         * </summary>
         */
        public Color value { get; private set; }

        /**
         * <summary>
         * Invokes listeners when the color is changed.
         * </summary>
         */
        public ValueEvent<Color> onValueChanged { get; } = new ValueEvent<Color>();

        /**
         * <summary>
         * Invokes listeners when the user has finished
         * selecting a color.
         * </summary>
         */
        public ValueEvent<Color> onSubmit { get; } = new ValueEvent<Color>();

        /**
         * <summary>
         * Initializes this color field.
         * </summary>
         * <param name="color">The default color</param>
         */
        public ColorField(Color color) {
            this.value = color;

            preview = new Image(color);
            preview.SetFill(FillType.All);
            preview.SetSize(-6f, -6f);
            Add(preview);

            onClick.AddListener(() => {
                UIRoot.colorPickerWindow.Link(this);
            });

            preview.DestroyMouseHandler();
        }

        /**
         * <summary>
         * Sets the color of this field.
         * </summary>
         * <param name="color">The color to set</param>
         */
        public void SetValue(Color color) {
            this.value = color;
            preview.SetColor(color);
        }
    }
}
