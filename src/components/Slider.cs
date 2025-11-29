using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UEImage = UnityEngine.UI.Image;
using UESlider = UnityEngine.UI.Slider;

using UILib.Behaviours;
using UILib.Layouts;

namespace UILib.Components {
    /**
     * <summary>
     * A component which can be dragged between
     * a lower and upper limit.
     * </summary>
     */
    public class Slider : UIComponent {
        private CustomSlider _slider;

        /**
         * <summary>
         * The underlying Unity `Slider`.
         * </summary>
         */
        public UESlider slider { get => (UESlider) _slider; }

        private Image background;

        private Area fillArea;
        private Area handleArea;
        private Image fillImage;
        private Image handleImage;

        /**
         * <summary>
         * Invokes listeners when this slider is selected.
         * </summary>
         */
        public UnityEvent onSelect { get => _slider.onSelect; }

        /**
         * <summary>
         * Invokes listeners when this slider is deselected.
         * </summary>
         */
        public UnityEvent onDeselect { get => _slider.onDeselect; }

        /**
         * <summary>
         * The current value of this slider.
         * </summary>
         */
        public float value { get => _slider.value; }

        /**
         * <summary>
         * Invokes listeners when the value of this slider changes.
         * Passes the current value of the slider to listeners.
         * </summary>
         */
        public ValueEvent<float> onValueChanged { get; } = new ValueEvent<float>();

        /**
         * <summary>
         * Initializes a slider.
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
            background.SetFill(FillType.All);

            fillArea = new Area();
            fillArea.SetFill(FillType.All);

            handleArea = new Area();
            handleArea.SetFill(FillType.All);

            fillImage = new Image();
            fillImage.SetFill(FillType.All);
            fillImage.image.type = UEImage.Type.Sliced;
            fillArea.Add(fillImage);

            handleImage = new Image(Resources.circle);
            handleImage.SetFill(FillType.All);
            handleArea.Add(handleImage);

            Add(background);
            Add(fillArea);
            Add(handleArea);

            _slider = gameObject.AddComponent<CustomSlider>();
            slider.fillRect = fillImage.rectTransform;
            slider.handleRect = handleImage.rectTransform;
            slider.targetGraphic = handleImage.image;
            slider.direction = direction;

            SetLimits(min, max);

            _slider.onValueChanged.AddListener((float value) => {
                onValueChanged.Invoke(value);
            });

            _slider.onDeselect.AddListener(() => {
                Notifications.Notifier.Notify("Deselect", "Deselected slider");
            });

            DestroyHandlers();

            // Set the theme
            SetTheme(theme);
        }

        /**
         * <summary>
         * Allows setting the theme of this slider
         * and all children.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        public override void SetTheme(Theme theme) {
            base.SetTheme(theme);

            slider.colors = theme.blockSelectLight;
            background.SetColor(theme.selectNormal);
            fillImage.SetColor(theme.selectHighlight);
        }

        /**
         * <summary>
         * Sets the size of this slider.
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
         * Sets the current value of this slider.
         * </summary>
         * <param name="value">The value to set</param>
         */
        public void SetValue(float value) {
            _slider.value = value;
        }

        /**
         * <summary>
         * Sets the limits for this slider.
         * </summary>
         * <param name="min">The minimum value</param>
         * <param name="max">The maximum value</param>
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
         * Sets the direction of this slider.
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
