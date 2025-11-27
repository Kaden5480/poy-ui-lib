using UIButton = UILib.Components.Button;

namespace UILib.Components {
    /**
     * <summary>
     * A component which is like a <see cref="Button"/>,
     * but can be toggled on or off.
     *
     * Also displays a checkmark indicating its current state.
     * </summary>
     */
    public class Toggle :  UIButton {
        /**
         * <summary>
         * The current value of the toggle.
         * </summary>
         */
        public bool value { get; private set; } = false;

        /**
         * <summary>
         * Initializes a toggle.
         * </summary>
         * <param name="initial">The initial value of the toggle</param>
         */
        public Toggle(bool initial = false) : base(Resources.checkMark) {
            onClick.AddListener(() => {
                SetValue(!value);
            });
            SetValue(initial);
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
