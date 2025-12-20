using UnityEngine;
using UnityEngine.EventSystems;

using UEToggle = UnityEngine.UI.Toggle;

using UILib.Behaviours;
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
        /**
         * <summary>
         * The underlying Unity `Toggle`.
         * </summary>
         */
        public UEToggle toggle { get; private set; }

        private bool internalChange = false;
        private Image background;
        private Image checkMark;

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

            checkMark = new Image(Resources.checkMark);
            checkMark.SetFill(FillType.All);
            Add(checkMark);

            toggle.targetGraphic = background.image;
            toggle.graphic = checkMark.image;

            // Destroy mouse handlers
            background.DestroyMouseHandler();
            checkMark.DestroyMouseHandler();

            // Add listeners
            onClick.AddListener(() => {
                EventSystem.current.SetSelectedGameObject(null);
            });

            toggle.onValueChanged.AddListener((bool val) => {
                if (internalChange == true) {
                    internalChange = false;
                    return;
                }

                onValueChanged.Invoke(val);
            });

            SetThisTheme(theme);
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
        }

        /**
         * <summary>
         * Allows setting the theme of this toggle.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {
            toggle.colors = theme.blockSelect;
            checkMark.SetColor(theme.foreground);
        }
    }
}
