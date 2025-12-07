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
        // Whether focus should be retained
        private bool retainFocus = false;
        private bool retainText = false;

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
         *
         * This is not the same as the text the user is entering.
         *
         * This updates when the user submits a valid string,
         * or when this is changed manually through <see cref="SetValue"/>
         * </summary>
         */
        public string value { get; private set; }

        /**
         * <summary>
         * The current text the user is entering.
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
         * Invokes listeners when the user cancels entering text.
         * </summary>
         */
        public UnityEvent onCancel { get; } = new UnityEvent();

        /**
         * <summary>
         * Invokes listeners when the user
         * starts typing something in the input field.
         *
         * The value listeners receive is the text the user is typing,
         * rather than the internally saved <see cref="value"/>.
         * </summary>
         */
        public ValueEvent<string> onInputChanged { get; } = new ValueEvent<string>();

        /**
         * <summary>
         * Invokes listeners with the user's input when they
         * submit something invalid.
         *
         * The value listeners receive is the text the user typed,
         * rather than the internally saved <see cref="value"/>.
         * </summary>
         */
        public ValueEvent<string> onInvalidSubmit { get; } = new ValueEvent<string>();

        /**
         * <summary>
         * Invokes listeners with the current value stored
         * when the user submits it.
         *
         * The value listeners receive is the text the user typed,
         * which will be the same as the internally saved <see cref="value"/>.
         *
         * If you have set a predicate, this only triggers if
         * the user's input was valid.
         * </summary>
         */
        public ValueEvent<string> onValidSubmit { get; } = new ValueEvent<string>();

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

            InitEvents();

            // Set the theme
            SetThisTheme(theme);
        }

#region Dealing with input events

        /**
         * Unity's input fields are a finnicky nightmare.
         *
         * They do basically nothing you'd actually want
         * them to do by default, so all of this code
         * is just working around it to make them actually usable.
         */

        private enum InputUpdateType {
            Cancelled,
            InvalidSubmit,
            ValidSubmit,
        }

        private class InputUpdate {
            internal InputUpdateType type;
            internal string userInput;

            internal InputUpdate(InputUpdateType type, string userInput) {
                this.type = type;
                this.userInput = userInput;
            }
        }

        // Most recent input update
        private InputUpdate update;

        // Whether the user clicked outside of this
        // text field to deselect it
        internal bool clickedOutside = false;

        // Whether the value was set internally
        private bool internalChange = false;

        /**
         * <summary>
         * Sets up the different input related events.
         * </summary>
         */
        private void InitEvents() {
            // Keep track of which input field is currently selected
            _inputField.onSelect.AddListener(() => {
                currentSelected = this;
            });

            _inputField.onValueChanged.AddListener((string value) => {
                // Ignore internal changes
                if (internalChange == false) {
                    onInputChanged.Invoke(value);
                }
                internalChange = false;
            });

            // Deal with garbage
            _inputField.onDeselect.AddListener(HandleDeselect);
            _inputField.onEndEdit.AddListener(HandleEndEdit);
        }

#region Input update handling

        /**
         * <summary>
         * Handles onCancel events.
         * </summary>
         */
        private void HandleCancel(string userInput) {
            // Restore the last valid value manually
            SetText(value);
            onCancel.Invoke();
        }

        /**
         * <summary>
         * Handles onSubmit events.
         * </summary>
         */
        private void HandleValidSubmit(string userInput) {
            this.value = userInput;
            onValidSubmit.Invoke(userInput);
        }

        /**
         * <summary>
         * Handles onInvalidSubmit events.
         * </summary>
         */
        private void HandleInvalidSubmit(string userInput) {
            // Restore to last good value
            SetText(value);
            onInvalidSubmit.Invoke(userInput);
        }

        /**
         * <summary>
         * Handles onDeselect events.
         * </summary>
         */
        private void HandleDeselect() {
            // Reset state
            clickedOutside = false;

            // Ignore if no updates
            if (update == null) {
                return;
            }

            // Process the update
            switch (update.type) {
                case InputUpdateType.Cancelled:
                    HandleCancel(update.userInput);
                    break;
                case InputUpdateType.InvalidSubmit:
                    HandleInvalidSubmit(update.userInput);
                    break;
                case InputUpdateType.ValidSubmit:
                    HandleValidSubmit(update.userInput);
                    break;
            }

            // Clear the update
            update = null;
        }

#endregion

#region Prepare Updates for HandleDeselect

        /**
         * <summary>
         * Checks whether the input was cancelled.
         * </summary>
         * <returns>Whether the input was cancelled</returns>
         */
        private bool DidCancel() {
            return _inputField.wasCanceled == true
                || clickedOutside == true;
        }

        /**
         * <summary>
         * Checks if the user just submitted a value.
         * </summary>
         * <returns>Whether the user submitted</returns>
         */
        private bool DidSubmit() {
            return internalChange == false
                && clickedOutside == false
                && _inputField.wasCanceled == false;
        }

        /**
         * <summary>
         * Handles onEndEdit events.
         *
         * This prepares an update which is handled in the onDeselect event.
         * </summary>
         */
        private void HandleEndEdit(string userInput) {
            userInput = userInput.Trim();

            // Check if input was cancelled
            if (DidCancel() == true) {
                update = new InputUpdate(
                    InputUpdateType.Cancelled,
                    userInput
                );
            }

            // Check if something was submitted
            else if (DidSubmit() == true) {
                // Validate the input
                if (Validate(userInput) == true) {
                    update = new InputUpdate(
                        InputUpdateType.ValidSubmit,
                        userInput
                    );
                }
                else {
                    update = new InputUpdate(
                        InputUpdateType.InvalidSubmit,
                        userInput
                    );
                }
            }

            // Nothing happened somehow
            else {
                logger.LogDebug("Nothing happened");
            }

            // Need to manually deselect in some cases
            if (clickedOutside == false) {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

#endregion
#endregion

#region Dealing with Deselections

        // Tracks whether the pointer is currently
        // over this text field
        private bool isPointerInside = false;

        // The currently selected text field
        internal static TextField currentSelected { get; private set; } = null;

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
                currentSelected.clickedOutside = true;
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

#endregion

        /**
         * <summary>
         * Sets whether this text field should retain
         * focus after a value is submitted.
         *
         * If `retain` is `false` the text field will lose focus
         * after a value is submitted. This is the default behaviour.
         *
         * If `retain` is `true` the text field will remain focused
         * and users can continue writing values to it even
         * after submitting.
         * </summary>
         * <param name="retainFocus">Whether focus should be retained</param>
         */
        public void RetainFocus(bool retainFocus) {
            this.retainFocus = retainFocus;
        }

        /**
         * <summary>
         * Whether this text field should retain its
         * currently displayed text
         *
         * By default, text fields restore to their
         * last submitted value when deselected.
         *
         * Passing `true` allows the displayed text to be retained instead.
         * </summary>
         * <param name="retainText">Whether the text should be retained</param>
         */
        public void RetainValue(bool retainText) {
            this.retainText = retainText;
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
         * Sets the displayed text without updating
         * the internally stored value.
         * </summary>
         * <param name="value">The value to change the displayed text to</param>
         */
        public void SetText(string value) {
            // This is set so onValueChanged knows
            // a non-user update happened
            if (_inputField.text != value) {
                internalChange = true;
                _inputField.text = value;
            }
        }

        /**
         * <summary>
         * Sets the current value stored in this
         * text field. Updates the internal value
         * and the text displayed in the input field.
         * </summary>
         * <param name="value">The value to update to</param>
         */
        public void SetValue(string value) {
            SetText(value);
            this.value = value;
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
    }
}
