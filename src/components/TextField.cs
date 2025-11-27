using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UEImage = UnityEngine.UI.Image;

using UILib.Behaviours;
using UILib.Layout;

namespace UILib.Components {
    /**
     * <summary>
     * A component which holds text that
     * a user can edit.
     * </summary>
     */
    public class TextField : UIComponent {
        public Label placeholder { get; private set; }
        public Label input        { get; private set; }

        private UEImage background;
        private CustomInputField inputField;

        /**
         * <summary>
         * The current value stored in this text field.
         * </summary>
         */
        public string value {
            get => input.text.text;
        }

        /**
         * <summary>
         * Invokes listeners when this text field is selected.
         * </summary>
         */
        public UnityEvent onSelect { get => inputField.onSelect; }

        /**
         * <summary>
         * Invokes listeners when this text field is deselected.
         * </summary>
         */
        public UnityEvent onDeselect { get => inputField.onDeselect; }

        /**
         * <summary>
         * Invokes listeners with the current value stored
         * in the text field whenever it's updated.
         * </summary>
         */
        public StringEvent onValueChanged { get; } = new StringEvent();

        /**
         * <summary>
         * Invokes listeners with the current value stored
         * in the text field when the user stops editing it.
         * </summary>
         */
        public StringEvent onEndEdit { get; } = new StringEvent();

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
                Patches.InputFieldFix.current = this;
            });

            inputField.onEndEdit.AddListener((string value) => {
                EventSystem.current.SetSelectedGameObject(null);
                onEndEdit.Invoke(value);
            });

            inputField.onValueChanged.AddListener((string value) => {
                onValueChanged.Invoke(value);
            });

            // Set the theme
            SetTheme(theme);
        }

        /**
         * <summary>
         * Allows setting the theme of this text field
         * and all children.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        public override void SetTheme(Theme theme) {
            base.SetTheme(theme);

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
