using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UEImage = UnityEngine.UI.Image;
using UEInputField = UnityEngine.UI.InputField;

using UILib.Behaviours;
using UILib.Layouts;
using UILib.Patches.UI;
using UILib.Notifications;

namespace UILib.Components {
    /**
     * <summary>
     * A component which holds text that
     * a user can edit.
     * </summary>
     */
    public class TextField : UIComponent {
        /**
         * <summary>
         * An enum of modes which determine in which
         * cases a <see cref="TextField"/> will clear
         * the input and its internal value.
         * </summary>
         */
        [Flags]
        public enum ClearMode {
            /**
             * <summary>
             * Don't clear.
             * </summary>
             */
            None = 0,

            /**
             * <summary>
             * Clear when the user presses escape.
             * </summary>
             */
            Escape = 1 << 1,

            /**
             * <summary>
             * Clear when the user clicks outside the text field.
             * </summary>
             */
            Click = 1 << 2,

            /**
             * <summary>
             * Clear on an invalid submit.
             * </summary>
             */
            InvalidSubmit = 1 << 3,

            /**
             * <summary>
             * Clear on an valid submit.
             * </summary>
             */
            ValidSubmit = 1 << 4,

            /**
             * <summary>
             * Always clear.
             * </summary>
             */
            Always = Escape | Click | InvalidSubmit | ValidSubmit,
        }

        /**
         * <summary>
         * An enum of modes which determine in which
         * cases a <see cref="TextField"/> will retain
         * the current user input.
         *
         * The input will always be retained on a valid submit.
         * </summary>
         */
        [Flags]
        public enum RetainMode {
            /**
             * <summary>
             * Don't retain input in any other special cases.
             * </summary>
             */
            None = 0,

            /**
             * <summary>
             * When the user presses escape, the input is retained.
             * </summary>
             */
            Escape = 1 << 0,

            /**
             * <summary>
             * When the user clicks something else, the input is retained.
             * </summary>
             */
            Click = 1 << 1,

            /**
             * <summary>
             * Submitting something invalid retains the input.
             * </summary>
             */
            InvalidSubmit = 1 << 2,
        }

        /**
         * <summary>
         * An enum of modes which determine which interactions
         * will count as a submit.
         *
         * Pressing `Return` will always count as a submit
         * </summary>
         */
        [Flags]
        public enum SubmitMode {
            /**
             * <summary>
             * Nothing extra counts as a submit.
             * </summary>
             */
            None = 0,

            /**
             * <summary>
             * Pressing escape will count as a submit.
             * </summary>
             */
            Escape = 1 << 0,

            /**
             * <summary>
             * Clicking something other than this field counts as a submit.
             * </summary>
             */
            Click = 1 << 1,
        }

        private Label placeholder;
        private Label input;

        private UEImage background;
        private CustomInputField inputField;

        // Whether the pointer is currently inside
        internal bool isPointerInside { get; private set; } = false;

        // Whether the text was updated internally
        private bool internalChange = false;

        // The predicate used for validating the user's input
        private Func<string, bool> predicate;

        /**
         * <summary>
         * Whether this text field will retain its focus
         * after the user submits an input.
         *
         * False by default.
         * </summary>
         */
        public bool retainFocus { get; private set; } = false;

        /**
         * <summary>
         * In which cases will this text field wipe its input and value.
         *
         * Note:
         * Clear modes will override equivalent retain modes.
         *
         * Default: None
         * </summary>
         */
        public ClearMode clearMode { get; private set; } = ClearMode.None;

        /**
         * <summary>
         * In which extra cases will this text field retain the
         * current user input.
         *
         * The input will always be retained on a valid submit.
         *
         * Default: None
         * </summary>
         */
        public RetainMode retainMode { get; private set; } = RetainMode.None;

        /**
         * <summary>
         * Which extra interactions with the text field will
         * count as a submit.
         *
         * Default: None
         * </summary>
         */
        public SubmitMode submitMode { get; private set; } = SubmitMode.None;

        /**
         * <summary>
         * The current user input (displayed text).
         * </summary>
         */
        public string userInput { get; private set; } = "";

        /**
         * <summary>
         * The current value stored in this text field.
         * This only updates whenever the user submits a valid input.
         * </summary>
         */
        public string value { get; private set; }

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
        public ValueEvent<string> onValidSubmit { get; } = new ValueEvent<string>();

        // The configured font size
        private int fontSize;

        /**
         * <summary>
         * Initializes a text field.
         * </summary>
         * <param name="text">The placeholder text</param>
         * <param name="fontSize">The font size to use</param>
         */
        public TextField(string text, int fontSize) {
            this.fontSize = fontSize;

            placeholder = new Label(text, fontSize);
            placeholder.SetFill(FillType.All);
            Add(placeholder);

            input = new Label("", fontSize);
            input.SetFill(FillType.All);
            Add(input);

            background = gameObject.AddComponent<UEImage>();
            inputField = gameObject.AddComponent<CustomInputField>();

            inputField.placeholder = placeholder.text;
            placeholder.text.alignByGeometry = false;

            inputField.textComponent = input.text;
            input.text.alignByGeometry = false;

            // A lot of magic is happening in InputFieldFix
            inputField.onSelect.AddListener(() => {
                InputFieldFix.Select(this);
            });
            inputField.onDeselect.AddListener(() => {
                if (InputFieldFix.current == this) {
                    InputFieldFix.Deselect();

                    if (submitMode.HasFlag(SubmitMode.Click) == true) {
                        string input = userInput;
                        if (Validate(input) == true) {
                            onValidSubmit.Invoke(input);
                        }
                        else {
                            onInvalidSubmit.Invoke(input);
                        }

                        return;
                    }

                    // Clear mode overrides
                    if (clearMode.HasFlag(ClearMode.Click) == true) {
                        SetValue("");
                    }
                    else if (retainMode.HasFlag(RetainMode.Click) == true) {
                        SetText(userInput);
                    }
                    else {
                        SetText(value);
                    }

                    onCancel.Invoke();
                }

            });
            inputField.onValueChanged.AddListener((string value) => {
                if (internalChange == true || inputField.wasCanceled == true) {
                    internalChange = false;
                    return;
                }

                userInput = value;
                onInputChanged.Invoke(userInput);
            });

            onPointerEnter.AddListener(() => { isPointerInside = true; });
            onPointerExit.AddListener(() => { isPointerInside = false; });

            // Set the theme
            SetThisTheme(theme);
        }

        /**
         * <summary>
         * Sets when this text field should clear the user input
         * and its value.
         *
         * Note:
         * Clear modes will override equivalent retain modes.
         * </summary>
         * <param name="clearMode">When to clear</param>
         */
        public void SetClearMode(ClearMode clearMode) {
            this.clearMode = clearMode;
        }

        /**
         * <summary>
         * Sets whether this text field should retain focus.
         * </summary>
         * <param name="retainFocus">Whether to retain focus</param>
         */
        public void SetRetainFocus(bool retainFocus) {
            this.retainFocus = retainFocus;
        }

        /**
         * <summary>
         * Sets in which cases this text field will retain
         * the current user input.
         *
         * The input will always be retained on a valid submit.
         * </summary>
         * <param name="retainMode">Which cases the input should be retained</param>
         */
        public void SetRetainMode(RetainMode retainMode) {
            this.retainMode = retainMode;
        }

        /**
         * <summary>
         * Sets which interactions with this text field
         * will count as a submit.
         * </summary>
         * <param name="submitMode">Which cases will count as a submit</param>
         */
        public void SetSubmitMode(SubmitMode submitMode) {
            this.submitMode = submitMode;
        }

        /**
         * <summary>
         * Checks if the user input is valid.
         * </summary>
         * <param name="userInput">The input to validate</param>
         * <returns>Whether the user's input was valid</returns>
         */
        internal bool Validate(string userInput) {
            bool valid = true;

            if (predicate != null) {
                valid = predicate(userInput);
            }

            // Handle valid inputs
            if (valid == true) {
                if (clearMode.HasFlag(ClearMode.ValidSubmit) == true) {
                    SetValue("");
                }
                else {
                    SetValue(userInput);
                }

                return valid;
            }

            // Clear mode overrides anything else
            if (clearMode.HasFlag(ClearMode.InvalidSubmit) == true) {
                SetValue("");
                return valid;
            }

            // Handle other invalid states
            if (retainMode.HasFlag(RetainMode.InvalidSubmit) == true) {
                SetText(userInput);
            }
            else {
                SetText(value);
            }

            return valid;
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
         * Sets the text displayed by this text field.
         * </summary>
         * <param name="text">The text to display</param>
         */
        public void SetText(string text) {
            if (text != inputField.text) {
                internalChange = true;
            }
            inputField.text = text;
            userInput = text;
        }

        /**
         * <summary>
         * Sets the current value stored in this
         * text field. Updates the internal value
         * and the text displayed in the input field.
         * </summary>
         * <param name="value">The value to set</param>
         */
        public void SetValue(string value) {
            this.value = value;
            SetText(value);
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

            input.text.fontSize = (int) theme.fontScalerAlt * fontSize;
            placeholder.text.fontSize = (int) theme.fontScalerAlt * fontSize;
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
