using UnityEngine;
using UnityEngine.EventSystems;

using UEImage = UnityEngine.UI.Image;
using UESlider = UnityEngine.UI.Slider;

namespace UILib {
    public class Slider : UIObject {
        private UESlider slider;

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
         * <param name="direction>The increasing direction for this slider</param>
         */
        public Slider(
            float min = 0f,
            float max = 1f,
            UESlider.Direction direction = UESlider.Direction.LeftToRight
        ) {
            background = new Image();

            fillArea = new Area();
            fillArea.Fill();

            handleArea = new Area();
            handleArea.Fill();

            fillImage = new Image();
            fillImage.image.type = UEImage.Type.Sliced;
            fillArea.Add(fillImage);

            handleImage = new Image();
            handleImage.SetSize(15f, 10f);
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
            if (IsVerticalDirection(direction)
                != IsVerticalDirection(slider.direction)
            ) {
                // Invert width and height
                SetSize(height, width);
            }

            slider.direction = direction;
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
