using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UEImage = UnityEngine.UI.Image;
using UEInputField = UnityEngine.UI.InputField;

using UILib.Behaviours;
using UILib.Layouts;

namespace UILib.Components {
    /**
     * <summary>
     * A component which holds text that
     * a user can edit.
     * </summary>
     */
    public class TextField : UIComponent {
        // Some fixes
        internal static TextField currentSelected { get; private set; } = null;
        internal bool wasDeselected = false;
        private bool isPointerInside = false;
        private bool setInput = false;

        // The previous value the user typed
        private string previousValue = "";

        private Label placeholder;
        private Label input;

        private UEImage background;
        private CustomInputField _inputField;

        // The predicate used for validating the user's input
        private Func<string, bool> predicate;

        /**
         * <summary>
         * The underlying Unity `InputField`.
         * </summary>
         */
        public UEInputField inputField { get => _inputField; }

        /**
         * <summary>
         * The current value stored in this text field.
         * </summary>
         */
        public string value { get; private set; }

        /**
         * <summary>
         * The current value the user is entering.
         * </summary>
         */
        public string userInput {
            get => input.text.text;
        }

        /**
         * <summary>
         * Invokes listeners when this text field is selected.
         * </summary>
         */
        public UnityEvent onSelect { get => _inputField.onSelect; }

        /**
         * <summary>
         * Invokes listeners when this text field is deselected.
         * </summary>
         */
        public UnityEvent onDeselect { get => _inputField.onDeselect; }

        /**
         * <summary>
         * Invokes listeners when the user
         * starts typing something in the input field.
         *
         * The value listeners receive is the text the user is typing
         * not the value which is actually stored in this text field.
         * </summary>
         */
        public ValueEvent<string> onInputChanged { get; } = new ValueEvent<string>();

        /**
         * <summary>
         * Invokes listeners when the user cancels entering text.
         * </summary>
         */
        public UnityEvent onCancel { get; } = new UnityEvent();

        /**
         * <summary>
         * Invokes listeners with the user's input when they
         * submit something invalid.
         * </summary>
         */
        public ValueEvent<string> onInvalidSubmit { get; } = new ValueEvent<string>();

        /**
         * <summary>
         * Invokes listeners with the current value stored
         * when the user submits it.
         *
         * The value listeners receive is the value stored in this text field.
         *
         * If you have set a predicate, this only triggers if
         * the user's input was valid.
         * </summary>
         */
        public ValueEvent<string> onSubmit { get; } = new ValueEvent<string>();

        /**
         * <summary>
         * Initializes a text field.
         * </summary>
         * <param name="text">The placeholder text</param>
         * <param name="fontSize">The font size to use</param>
         */
        public TextField(string text, int fontSize) {
            placeholder = new Label(text, 2 * fontSize);
            placeholder.SetFill(FillType.All);
            Add(placeholder);

            input = new Label("", 2 * fontSize);
            input.SetFill(FillType.All);
            Add(input);

            background = gameObject.AddComponent<UEImage>();
            _inputField = gameObject.AddComponent<CustomInputField>();

            _inputField.placeholder = placeholder.text;
            placeholder.text.alignByGeometry = false;

            _inputField.textComponent = input.text;
            input.text.alignByGeometry = false;

            onPointerEnter.AddListener(() => { isPointerInside = true; });
            onPointerExit.AddListener(() => { isPointerInside = false; });

            _inputField.onSelect.AddListener(() => {
                currentSelected = this;
            });

            _inputField.onDeselect.AddListener(() => {
                if (EventSystem.current.currentSelectedGameObject
                    == currentSelected.gameObject
                ) {
                    currentSelected = null;
                }
            });

            // spaghetti makes the world go round
            _inputField.onEndEdit.AddListener((string userInput) => {
                userInput = userInput.Trim();

                if (wasDeselected == false) {
                    EventSystem.current.SetSelectedGameObject(null);
                }

                // Check for cancels
                if (_inputField.wasCanceled == true
                    || wasDeselected == true
                ) {
                    setInput = true;
                    wasDeselected = false;

                    // Overwrite with last internal data
                    _inputField.text = this.value;
                    onCancel.Invoke();
                    return;
                }

                // Check for submitted data
                if (DidSubmit() == false) {
                    return;
                }

                // Use the user's input as the new internal data
                if (Validate(userInput) == true) {
                    this.value = userInput;
                    onSubmit.Invoke(this.value);
                }
                // Or restore
                else {
                    setInput = true;
                    _inputField.text = this.value;
                    onInvalidSubmit.Invoke(userInput);
                }
            });

            _inputField.onValueChanged.AddListener((string value) => {
                // Ignore internal updates
                if (setInput == true) {
                    setInput = false;
                    return;
                }

                if (value.Equals(previousValue) == false) {
                    onInputChanged.Invoke(value);
                }

                previousValue = value;
            });

            // Set the theme
            SetThisTheme(theme);
        }

        /**
         * <summary>
         * Checks if the user just submitted a value.
         * </summary>
         * <returns>Whether the user submitted</returns>
         */
        private bool DidSubmit() {
            return setInput == false
                && wasDeselected == false
                && _inputField.wasCanceled == false;
        }

        /**
         * <summary>
         * Checks if the provided input is valid.
         * </summary>
         * <param name="value">The value to validate</param>
         * <returns>True if it's valid, false otherwise</returns>
         */
        private bool Validate(string value) {
            if (predicate == null) {
                return true;
            }

            return predicate(value) == true;
        }

        /**
         * <summary>
         * Sets the predicate to use for validating
         * the user's input.
         * </summary>
         * <param name="predicate">The predicate to use</param>
         */
        public void SetPredicate(Func<string, bool> predicate) {
            this.predicate = predicate;
        }

        /**
         * <summary>
         * Sets the current value stored in this
         * text field. Updates the internal value
         * and the value displayed in the input field.
         * </summary>
         */
        public void SetValue(string value) {
            setInput = true;
            this.value = value;
            _inputField.text = value;
        }

        /**
         * <summary>
         * Allows setting the theme of this text field.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {
            background.color = theme.selectNormal;
            placeholder.text.color = theme.selectAltNormal;
            placeholder.text.font = theme.fontAlt;
            input.text.font = theme.fontAlt;
        }

        /**
         * <summary>
         * Sets the alignment of the text.
         * </summary>
         * <param name="alignment">The alignment to use</param>
         */
        public void SetAlignment(TextAnchor alignment) {
            placeholder.text.alignment = alignment;
            input.text.alignment = alignment;
        }

        /**
         * <summary>
         * Runs every frame to handle deselecting when
         * clicking outside of a text field.
         * </summary>
         */
        internal static void Update() {
            if (currentSelected == null) {
                return;
            }

            if (Input.GetMouseButtonDown(0) == false) {
                return;
            }

            if (currentSelected.isPointerInside == true) {
                return;
            }

            // - There is a text field selected
            // - The pointer clicked
            // - The pointer is located outside the text field

            // So unfocus it if it is the current selected GameObject
            if (EventSystem.current.currentSelectedGameObject
                == currentSelected.gameObject
            ) {
                currentSelected.wasDeselected = true;
                EventSystem.current.SetSelectedGameObject(null);
                currentSelected = null;
            }
        }
    }
}
