using UnityEngine;

using UILib.Events;
using UILib.Layouts;

namespace UILib.Components {
    /**
     * <summary>
     * A 2D picker component.
     *
     * Kind of like a <see cref="Slider"/>, but allows you
     * to handle `x` and `y` values.
     *
     * By default:
     * (0, 0) is the bottom left
     * (1, 1) is the top right
     * </summary>
     */
    public class Picker : UIComponent {
        /**
         * <summary>
         * The current value stored in the picker.
         *
         * The position of the handle is first normalized
         * and then the minimum and maximum x and y values are used
         * to calculate the current value of the handle.
         *
         * This value will always be between (`minX`, `minY`) and (`maxX`, `maxY`).
         * </summary>
         */
        public Vector2 value {
            get => PositionToValue(handle.rectTransform.localPosition);
        }

        /**
         * <summary>
         * The minimum x value.
         * </summary>
         */
        public float minX { get; private set; } = 0f;

        /**
         * <summary>
         * The minimum y value.
         * </summary>
         */
        public float minY { get; private set; } = 0f;

        /**
         * <summary>
         * The maximum x value.
         * </summary>
         */
        public float maxX { get; private set; } = 1f;

        /**
         * <summary>
         * The maximum y value.
         * </summary>
         */
        public float maxY { get; private set; } = 1f;

        /**
         * <summary>
         * Invokes listeners with the current value
         * when the position of the handle is changed
         * by the user.
         * </summary>
         */
        public ValueEvent<Vector2> onValueChanged { get; } = new ValueEvent<Vector2>();

        /**
         * <summary>
         * Invokes listeners with the current value
         * when the user stops moving the handle.
         * </summary>
         */
        public ValueEvent<Vector2> onSubmit { get; } = new ValueEvent<Vector2>();

        /**
         * <summary>
         * The background of this picker.
         * </summary>
         */
        public Image background { get; private set; }

        /**
         * <summary>
         * The picker's handle.
         * </summary>
         */
        public Image handle { get; private set; }

        /**
         * <summary>
         * Initializes a picker.
         * </summary>
         */
        public Picker() {
            eventHandler.onClickPos.AddListener(OnClickPos);

            background = new Image();
            background.SetFill(FillType.All);
            Add(background);

            handle = new Image(Resources.circle);
            handle.SetSize(15f, 15f);
            Add(handle);

            // Handle pointer up
            onPointerUp.AddListener(() => {
                onSubmit.Invoke(value);
            });

            // Set the theme
            SetThisTheme(theme);

            background.DestroyEventHandler();
            handle.DestroyEventHandler();
        }

        /**
         * <summary>
         * Allows setting the theme of this picker.
         *
         * This handles setting the theme specifically for this component,
         * not its children. It's protected to allow overriding if you
         * were to create a subclass.
         *
         * In most cases, you'd probably want to use
         * <see cref="UIObject.SetTheme"/> instead.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {
            background.SetColor(theme.selectNormal);
            handle.SetColor(theme.selectAltNormal);
        }

        /**
         * <summary>
         * Sets the current value of this picker.
         *
         * These values must be within your configured min and max
         * values.
         * </summary>
         * <param name="x">The x value to set the picker to</param>
         * <param name="y">The y value to set the picker to</param>
         */
        public void SetValue(float x, float y) {
            x = Mathf.Clamp(x, minX, maxX);
            y = Mathf.Clamp(y, minY, maxY);

            Vector2 position = ValueToPosition(x, y);
            handle.rectTransform.localPosition = position;
        }

        /**
         * <summary>
         * Sets the minimum values of this picker.
         * </summary>
         * <param name="minX">The minimum x value</param>
         * <param name="minY">The minimum y value</param>
         */
        public void SetMinValues(float minX, float minY) {
            this.minX = minX;
            this.minY = minY;
        }

        /**
         * <summary>
         * Sets the maximum values of this picker.
         * </summary>
         * <param name="maxX">The maximum x value</param>
         * <param name="maxY">The maximum y value</param>
         */
        public void SetMaxValues(float maxX, float maxY) {
            this.maxX = maxX;
            this.maxY = maxY;
        }

        /**
         * <summary>
         * Converts a value into a local position.
         * </summary>
         * <param name="x">The x value to convert</param>
         * <param name="y">The y value to convert</param>
         */
        private Vector2 ValueToPosition(float x, float y) {
            Vector2 pivot = rectTransform.pivot;
            Vector2 size = rectTransform.sizeDelta;

            float normX = (x - minX) / (maxX - minX);
            float normY = (y - minY) / (maxY - minY);

            float posX = size.x * (normX - pivot.x);
            float posY = size.y * (normY - pivot.y);

            return new Vector2(posX, posY);
        }

        /**
         * <summary>
         * Converts a provided local position into a
         * value within the min and max limits.
         * </summary>
         * <param name="position">The local position to convert</param>
         * <returns>The value</returns>
         */
        private Vector2 PositionToValue(Vector2 position) {
            Vector2 pivot = rectTransform.pivot;
            Vector2 size = rectTransform.sizeDelta;

            float normX = (position.x/size.x) + pivot.x;
            float normY = (position.y/size.y) + pivot.y;

            float valueX = minX + normX * (maxX - minX);
            float valueY = minY + normY * (maxY - minY);

            return new Vector2(
                Mathf.Clamp(valueX, minX, maxX),
                Mathf.Clamp(valueY, minY, maxY)
            );
        }

        /**
         * <summary>
         * Moves the handle to a new position, clamping it to
         * be within bounds of the picker.
         * </summary>
         * <param name="position">The new global position to move to</param>
         */
        private void MoveHandle(Vector2 position) {
            Vector2 local = ToLocalPosition(position);

            Vector2 pivot = rectTransform.pivot;
            Vector2 size = rectTransform.sizeDelta;

            handle.rectTransform.localPosition = new Vector2(
                Mathf.Clamp(local.x, -(pivot.x*size.x), (1-pivot.x)*size.x),
                Mathf.Clamp(local.y, -(pivot.y*size.y), (1-pivot.y)*size.y)
            );

            onValueChanged.Invoke(
                PositionToValue(handle.rectTransform.localPosition)
            );
        }

        /**
         * <summary>
         * Handles this picker being clicked.
         * </summary>
         * <param name="position">The position clicked at</param>
         */
        private void OnClickPos(Vector2 position) {
            MoveHandle(position);
        }

        /**
         * <summary>
         * Handles this picker being dragged.
         * </summary>
         * <param name="position">The position dragged to</param>
         */
        protected override void OnDrag(Vector2 position) {
            base.OnDrag(position);

            if (Input.GetKey(KeyCode.LeftAlt) == true) {
                return;
            }

            MoveHandle(position);
        }
    }
}
