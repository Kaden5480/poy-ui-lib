using UnityEngine;

using UILib.Behaviours;
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
         * This value will always be between (minX, minY) and (maxX, maxY).
         * </summary>
         */
        public Vector2 value { get => Calculate(); }

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
         * when the position of the handle changes.
         * </summary>
         */
        public ValueEvent<Vector2> onValueChanged { get; } = new ValueEvent<Vector2>();

        private Image background;
        private Image handle;

        /**
         * <summary>
         * Initializes a picker.
         * </summary>
         */
        public Picker() {
            mouseHandler.onClickPos.AddListener(OnClickPos);

            background = new Image();
            background.SetFill(FillType.All);
            Add(background);

            handle = new Image(Resources.circle);
            handle.SetSize(15f, 15f);
            Add(handle);

            // Set the theme
            SetThisTheme(theme);

            background.DestroyMouseHandler();
            handle.DestroyMouseHandler();
        }

        /**
         * <summary>
         * Allows setting the theme of this picker.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {
            background.SetColor(theme.selectNormal);
            handle.SetColor(theme.selectAltNormal);
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
         * Calculates the current coordinates based upon
         * the size and pivot of the container.
         * </summary>
         * <returns>The coordinates</returns>
         */
        private Vector2 Calculate() {
            Vector2 handlePos = handle.rectTransform.localPosition;
            Vector2 pivot = rectTransform.pivot;
            Vector2 size = rectTransform.sizeDelta;

            float normX = (handlePos.x/size.x) + pivot.x;
            float normY = (handlePos.y/size.y) + pivot.y;

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

            onValueChanged.Invoke(Calculate());
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
            MoveHandle(position);
        }

#region Stopping Overrides

        /**
         * <summary>
         * Handles this picker being clicked.
         * </summary>
         */
        protected override void OnClick() {}

        /**
         * <summary>
         * Handles the start of this picker being dragged.
         * </summary>
         * <param name="position">The position the drag started at</param>
         */
        protected override void OnBeginDrag(Vector2 position) {}

        /**
         * <summary>
         * Handles the end of this picker being dragged.
         * </summary>
         * <param name="position">The position the drag ended at</param>
         */
        protected override void OnEndDrag(Vector2 position) {}

#endregion

    }
}
