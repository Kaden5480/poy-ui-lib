using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UEImage = UnityEngine.UI.Image;
using UESlider = UnityEngine.UI.Slider;

using UILib.Behaviours;
using UILib.Layouts;
using UILib.Patches.UI;

namespace UILib.Components {
    /**
     * <summary>
     * A component which can be dragged between
     * a lower and upper limit.
     * </summary>
     */
    public class Slider : UIComponent {
        private CustomSlider _slider;
        private bool internalChange = false;
        private Area fillArea;
        private Area handleArea;

        /**
         * <summary>
         * The underlying Unity `Slider`.
         * </summary>
         */
        public UESlider slider { get => (UESlider) _slider; }

        /**
         * <summary>
         * The normal background color of the slider.
         * </summary>
         */
        public Image background { get; private set; }

        /**
         * <summary>
         * The image which covers the slider's
         * background as the value increases.
         * </summary>
         */
        public Image fill { get; private set; }

        /**
         * <summary>
         * The slider's handle.
         * </summary>
         */
        public Image handle { get; private set; }

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

            fill = new Image();
            fill.SetFill(FillType.All);
            fill.image.type = UEImage.Type.Sliced;
            fillArea.Add(fill);

            handle = new Image(Resources.circle);
            handle.SetFill(FillType.All);
            handleArea.Add(handle);

            Add(background);
            Add(fillArea);
            Add(handleArea);

            _slider = gameObject.AddComponent<CustomSlider>();
            slider.fillRect = fill.rectTransform;
            slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle.image;
            slider.direction = direction;

            SetLimits(min, max);

            _slider.onValueChanged.AddListener((float value) => {
                if (internalChange == true) {
                    internalChange = false;
                    return;
                }
                onValueChanged.Invoke(value);
            });

            DestroyHandlers();

            // Set the theme
            SetThisTheme(theme);
        }

        /**
         * <summary>
         * Allows setting the theme of this slider.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {
            slider.colors = theme.blockSelectLight;
            background.SetColor(theme.selectNormal);
            fill.SetColor(theme.selectHighlight);
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
                handle.SetSize(width, 2*width);
            }
            else {
                handle.SetSize(2*height, height);
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
            internalChange = true;
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
            fill.DestroyMouseHandler();
            handleArea.DestroyMouseHandler();
            handle.DestroyMouseHandler();
        }
    }
}
