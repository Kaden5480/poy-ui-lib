namespace UILib {
    /**
     * <summary>
     * An object which is like a button,
     * but can be toggled on or off.
     *
     * Also displays a checkmark indicating its current state.
     * </summary>
     */
    public class Toggle : Button {
        public bool value { get; private set; } = false;

        /**
         * <summary>
         * Initializes a Toggle.
         * </summary>
         */
        public Toggle() : base(Resources.checkMark) {
            AddListener(() => {
                SetValue(!value);
            });
            SetValue(false);
        }

        /**
         * <summary>
         * Sets the value of the toggle.
         * </summary>
         * <param name="value">The value to set the toggle to</param>
         */
        public void SetValue(bool value) {
            if (value == true) {
                image.Show();
            }
            else {
                image.Hide();
            }

            this.value = value;
        }
    }
}
