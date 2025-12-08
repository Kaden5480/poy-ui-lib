using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UEImage = UnityEngine.UI.Image;
using UEInputField = UnityEngine.UI.InputField;

using UILib.Behaviours;
using UILib.Layouts;
using UILib.Notifications;

namespace UILib.Components {
    /**
     * <summary>
     * A component which holds text that
     * a user can edit.
     * </summary>
     */
    public class TextField : UIComponent {
        private Label placeholder;
        private Label input;

        private UEImage background;
        private CustomInputField inputField;

        // The predicate used for validating the user's input
        private Func<string, bool> predicate;

        /**
         * <summary>
         * The current value stored in this text field.
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
            inputField = gameObject.AddComponent<CustomInputField>();

            inputField.placeholder = placeholder.text;
            placeholder.text.alignByGeometry = false;

            inputField.textComponent = input.text;
            input.text.alignByGeometry = false;

            inputField.onSelect.AddListener(() => {
                currentSelected = this;
            });
            inputField.onDeselect.AddListener(() => {
                if (currentSelected == this) {
                    currentSelected = null;
                    SetText(value);
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
         * Sets the text displayed by this text field.
         * </summary>
         * <param name="text">The text to display</param>
         */
        public void SetText(string text) {
            if (text != inputField.text) {
                internalChange = true;
            }
            inputField.text = text;
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

#region Patches

        // Some fixes
        internal static TextField currentSelected { get; private set; } = null;
        private bool isPointerInside = false;

        private string userInput     = "";
        private bool internalChange  = false;

        private void CheckDeselect() {
            if (isPointerInside == true) {
                return;
            }

            if (EventSystem.current.currentSelectedGameObject == gameObject) {
                HandleCancel();
            }
        }

        private void HandleCancel() {
            currentSelected = null;
            EventSystem.current.SetSelectedGameObject(null);
            SetText(value);
            onCancel.Invoke();
        }

        private void HandleSubmit() {
            currentSelected = null;
            SetValue(userInput);
            EventSystem.current.SetSelectedGameObject(null);
            onValidSubmit.Invoke(value);
            EventSystem.current.SetSelectedGameObject(gameObject);
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

            if (Input.GetMouseButtonDown(0) == true) {
                currentSelected.CheckDeselect();
            }

            if (Input.GetKeyDown(KeyCode.Escape) == true) {
                currentSelected.HandleCancel();
            }

            if (Input.GetKeyDown(KeyCode.Return) == true) {
                currentSelected.HandleSubmit();
            }
        }

#endregion

    }
}
