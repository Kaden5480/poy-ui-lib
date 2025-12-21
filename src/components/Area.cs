using UILib.Layouts;

namespace UILib.Components {
    /**
     * <summary>
     * A blank area, does nothing special.
     * </summary>
     */
    public class Area : UIComponent {
        /**
         * <summary>
         * Initializes an area.
         * </summary>
         */
        public Area() {}

        /**
         * <summary>
         * Initializes an area with a given size and fill.
         * </summary>
         */
        public Area(float width = 0f, float height = 0f, FillType fillType = FillType.None) {
            SetSize(width, height);

            if (fillType != FillType.None) {
                SetFill(fillType);
            }
        }

        /**
         * <summary>
         * Allows setting the theme of this area.
         * Does nothing.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {}
    }
}
