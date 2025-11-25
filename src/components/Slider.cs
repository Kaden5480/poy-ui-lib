using UnityEngine;
using UnityEngine.EventSystems;

using UEImage = UnityEngine.UI.Image;
using UESlider = UnityEngine.UI.Slider;

using UILib.Layout;

namespace UILib.Components {
    public class Slider : UIComponent {
        public UESlider slider { get; private set; }

        public Image background { get; private set; }

        public Area fillArea     { get; private set; }
        public Area handleArea   { get; private set; }
        public Image fillImage   { get; private set; }
        public Image handleImage { get; private set; }

        /**
         * <summary>
         * Initializes a Slider.
         * </summary>
         * <param name="min">The minimum value of this slider</param>
         * <param name="max">The maximum value of this slider</param>
         * <param name="direction">The increasing direction for this slider</param>
         */
        public Slider(
            float min = 0f,
            float max = 1f,
            UESlider.Direction direction = UESlider.Direction.LeftToRight
        ) {
            background = new Image();

            fillArea = new Area();
            fillArea.SetFill(FillType.All);

            handleArea = new Area();
            handleArea.SetFill(FillType.All);

            fillImage = new Image();
            fillImage.image.type = UEImage.Type.Sliced;
            fillArea.Add(fillImage);

            handleImage = new Image(Resources.circle);
            handleArea.Add(handleImage);

            Add(background);
            Add(fillArea);
            Add(handleArea);

            slider = gameObject.AddComponent<UESlider>();
            slider.colors = Colors.lighterColorBlock;
            slider.fillRect = fillImage.rectTransform;
            slider.handleRect = handleImage.rectTransform;
            slider.targetGraphic = handleImage.image;
            slider.direction = direction;

            SetLimits(min, max);

            background.SetColor(Colors.grey);
            fillImage.SetColor(Colors.lightGrey);

            DestroyHandlers();
        }

        /**
         * <summary>
         * Sets the size of this Slider.
         * </summary>
         * <param name="width">The width to set</param>
         * <param name="height">The height to set</param>
         */
        public override void SetSize(float width, float height) {
            base.SetSize(width, height);

            if (IsVerticalDirection(slider.direction) == true) {
                handleImage.SetSize(width, 2*width);
            }
            else {
                handleImage.SetSize(2*height, height);
            }
        }

        /**
         * <summary>
         * Checks if the provided direction is vertical.
         * </summary>
         * <param name="direction">The direction to check</param>
         */
        private bool IsVerticalDirection(UESlider.Direction direction) {
            return direction == UESlider.Direction.BottomToTop
                || direction == UESlider.Direction.TopToBottom;
        }

        /**
         * <summary>
         * Sets the limits for this slider.
         * </summary>
         */
        public void SetLimits(float min, float max) {
            slider.minValue = min;
            slider.maxValue = max;
        }

        /**
         * <summary>
         * Sets this slider to use whole numbers
         * </summary>
         * <param name="use">Whether whole numbers should be used</param>
         */
        public void UseWholeNumbers(bool use = true) {
            slider.wholeNumbers = use;
        }

        /**
         * <summary>
         * Sets the direction of this scrollbar.
         * </summary>
         * <param name="direction">The direction to use</param>
         */
        public void SetDirection(UESlider.Direction direction) {
            UESlider.Direction oldDirection = slider.direction;
            slider.direction = direction;

            if (IsVerticalDirection(direction)
                != IsVerticalDirection(oldDirection)
            ) {
                // Invert width and height
                SetSize(height, width);
            }
        }

        /**
         * <summary>
         * Destroys some mouse handlers which would otherwise
         * be problematic.
         * </summary>
         */
        private void DestroyHandlers() {
            background.DestroyMouseHandler();
            fillArea.DestroyMouseHandler();
            fillImage.DestroyMouseHandler();
            handleArea.DestroyMouseHandler();
            handleImage.DestroyMouseHandler();
        }
    }
}
