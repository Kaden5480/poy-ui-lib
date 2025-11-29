using System.Collections.Generic;

using UILib.Behaviours;
using UILib.Layouts;
using UILib.Patches;

namespace UILib.Components {
    /**
     * <summary>
     * A component which displays a dropdown
     * of multiple options.
     * </summary>
     */
    public class Dropdown<T> : UIComponent {
        private float defaultHeight;

        private int fontSize;
        private int optionCount = 3;

        private Button caption;
        private ScrollView scrollView;

        /**
         * <summary>
         * The current value of the dropdown.
         * </summary>
         */
        public T value { get; private set; }

        /**
         * <summary>
         * An event which invokes listeners
         * whenever a new value is selected by the user.
         * Passes the value which was selected.
         * </summary>
         */
        public ValueEvent<T> onValueChanged { get; } = new ValueEvent<T>();

        /**
         * <summary>
         * Initializes a dropdown with a specified default value.
         *
         * If `defaultDisplayName` is null, the `ToString` method of
         * the default option will be used instead.
         * </summary>
         * <param name="defaultValue">The default value to display</param>
         * <param name="fontSize">The font size to use in this dropdown</param>
         * <param name="defaultDisplayName">The display name of the default value to show</param>
         */
        public Dropdown(T defaultValue, int fontSize, string defaultDisplayName = null) {
            this.value = defaultValue;
            this.fontSize = fontSize;

            if (defaultDisplayName == null) {
                defaultDisplayName = defaultValue.ToString();
            }

            SetContentLayout(LayoutType.Vertical);

            // The initial option to display
            caption = new Button(defaultDisplayName, 20);
            Add(caption);

            // The scroll view of all options
            scrollView = new ScrollView(ScrollType.Vertical, 10f);
            scrollView.SetFill(FillType.All);
            scrollView.SetContentLayout(LayoutType.Vertical);
            scrollView.scrollRect.horizontal = false;
            scrollView.Hide();
            Add(scrollView);

            caption.onClick.AddListener(() => {
                scrollView.ToggleVisibility();
                ResizeArea();
            });

            // Update the theme
            SetTheme(theme);
        }

        /**
         * <summary>
         * Resizes the area depending on whether the
         * dropdown is active or not.
         * </summary>
         */
        private void ResizeArea() {
            if (scrollView.isVisible == false) {
                base.SetSize(width, defaultHeight);
            }
            else {
                base.SetSize(width, (defaultHeight*(optionCount+1)));
            }
        }

        /**
         * <summary>
         * Updates the currently selected option.
         * </summary>
         * <param name="option">The option to set</param>
         * <param name="displayName">The display name of this option</param>
         */
        private void SetOption(T option, string displayName) {
            // Hide the scroll view
            scrollView.Hide();
            ResizeArea();

            this.value = option;
            caption.SetText(displayName);

            this.onValueChanged.Invoke(option);
        }

        /**
         * <summary>
         * Sets the size of this dropdown.
         * This also sets the size of all child options.
         * </summary>
         * <param name="width">The width to set</param>
         * <param name="height">The height to set</param>
         */
        public override void SetSize(float width, float height) {
            base.SetSize(width, height);

            defaultHeight = height;

            caption.SetSize(width, height);
            caption.layoutElement.minHeight = height;
            scrollView.SetSize(width, (height*(optionCount+2)));
        }

        /**
         * <summary>
         * Set how many options you want to display
         * in the dropdown (without needing to scroll).
         * </summary>
         * <param name="optionCount">The number of options to display</param>
         */
        public void SetOptionCount(int optionCount) {
            this.optionCount = optionCount;
        }

        /**
         * <summary>
         * Allows setting the theme of this dropdown
         * and all children.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        public override void SetTheme(Theme theme) {
            base.SetTheme(theme);
        }

#region Adding Options

        /**
         * <summary>
         * Adds an option to this dropdown.
         *
         * If `displayName` is `null`, the option's
         * `ToString` method will be called instead.
         * </summary>
         * <param name="option">The option to add</param>
         * <param name="displayName">A custom display name</param>
         */
        public void AddOption(T option, string displayName = null) {
            if (displayName == null) {
                displayName = option.ToString();
            }

            Button button = new Button(displayName, fontSize);
            button.SetSize(width, height);
            button.onClick.AddListener(() => {
                SetOption(option, displayName);
            });

            scrollView.Add(button);
        }

        /**
         * <summary>
         * Adds many options to this dropdown.
         * </summary>
         * <param name="options">The options to add</param>
         */
        public void AddOptions(IList<T> options) {
            foreach (T option in options) {
                AddOption(option);
            }
        }

        /**
         * <summary>
         * Adds many options to this dropdown.
         * The string `key` indicates the display names
         * these options should have.
         * </summary>
         * <param name="options">The options to add</param>
         */
        public void AddOptions(Dictionary<string, T> options) {
            foreach (KeyValuePair<string, T> entry in options) {
                AddOption(entry.Value, entry.Key);
            }
        }

#endregion

    }
}
