using UnityEngine;
using UnityEngine.EventSystems;

using UEToggle = UnityEngine.UI.Toggle;

using UILib.Events;
using UILib.Layouts;

namespace UILib.Components {
    /**
     * <summary>
     * A component which is like a <see cref="Button"/>,
     * but can be toggled on or off.
     *
     * Also displays a checkmark indicating its current state.
     * </summary>
     */
    public class Toggle : UIComponent {
        private bool internalChange = false;

        /**
         * <summary>
         * The underlying Unity `Toggle`.
         * </summary>
         */
        public UEToggle toggle { get; private set; }

        /**
         * <summary>
         * The background of the toggle.
         * </summary>
         */
        public Image background { get; private set; }

        /**
         * <summary>
         * The image which displays when the toggle is on.
         * </summary>
         */
        public Image onImage { get; private set; }

        /**
         * <summary>
         * The image which displays when the toggle is off.
         * </summary>
         */
        public Image offImage { get; private set; }

        /**
         * <summary>
         * The current value of the toggle.
         * </summary>
         */
        public bool value { get => toggle.isOn; }

        /**
         * <summary>
         * Invokes listeners with the current value stored
         * in the toggle whenever it's updated.
         * </summary>
         */
        public ValueEvent<bool> onValueChanged { get; } = new ValueEvent<bool>();

        /**
         * <summary>
         * Whether this toggle will play the navigation sound when clicked.
         * </summary>
         */
        public bool playSound { get; private set; } = true;

        /**
         * <summary>
         * Initializes this toggle.
         * </summary>
         * <param name="value">The default value</param>
         */
        public Toggle(bool value = false) {
            toggle = gameObject.AddComponent<UEToggle>();
            toggle.isOn = value;

            background = new Image();
            background.SetFill(FillType.All);
            Add(background);

            toggle.targetGraphic = background.image;

            // Destroy mouse handlers
            background.DestroyMouseHandler();

            // Add listeners
            onClick.AddListener(() => {
                EventSystem.current.SetSelectedGameObject(null);
            });

            toggle.onValueChanged.AddListener((bool val) => {
                if (internalChange == true) {
                    internalChange = false;
                    return;
                }

                UpdateOffImage(val);

                if (playSound == true) {
                    Audio.PlayNavigation(theme);
                }

                onValueChanged.Invoke(val);
            });

            // Set the default "on" image
            Image onImage = new Image(Resources.checkMark);
            onImage.SetFill(FillType.All);
            SetOnImage(onImage);

            SetThisTheme(theme);
        }

        /**
         * <summary>
         * Updates the state of the off image.
         * </summary>
         * <param name="value">The current value of the toggle</param>
         */
        private void UpdateOffImage(bool value) {
            if (offImage == null) {
                return;
            }

            if (value == true) {
                offImage.Hide();
            }
            else {
                offImage.Show();
            }
        }

        /**
         * <summary>
         * Sets the current value of this toggle.
         * </summary>
         */
        public void SetValue(bool value) {
            if (value == toggle.isOn) {
                return;
            }

            internalChange = true;
            toggle.isOn = value;
            UpdateOffImage(value);
        }

        /**
         * <summary>
         * Sets the image to display in the "on" state.
         * </summary>
         * <param name="image">The image to display</param>
         */
        public void SetOnImage(Image onImage) {
            this.onImage = onImage;
            Add(onImage);

            onImage.DestroyMouseHandler();

            toggle.graphic = onImage.image;
        }

        /**
         * <summary>
         * Sets the image to display in the "off" state.
         * </summary>
         * <param name="image">The image to display</param>
         */
        public void SetOffImage(Image offImage) {
            this.offImage = offImage;
            Add(offImage);

            offImage.DestroyMouseHandler();

            UpdateOffImage(value);
        }

        /**
         * <summary>
         * Sets whether this toggle should play the navigation sound.
         * </summary>
         * <param name="playSound">Whether this toggle should play a sound</param>
         */
        public void SetPlaySound(bool playSound) {
            this.playSound = playSound;
        }

        /**
         * <summary>
         * Allows setting the theme of this toggle.
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
            base.SetThisTheme(theme);

            toggle.colors = theme.blockSelect;

            if (onImage != null) {
                onImage.SetColor(theme.foreground);
            }

            if (offImage != null) {
                offImage.SetColor(theme.foreground);
            }
        }
    }
}
