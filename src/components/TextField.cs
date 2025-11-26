using UnityEngine;
using UnityEngine.EventSystems;

using UEImage = UnityEngine.UI.Image;
using UEInputField = UnityEngine.UI.InputField;

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
        private UEInputField inputField;

        public StringEvent onEndEdit { get; } = new StringEvent();
        public StringEvent onValueChanged { get; } = new StringEvent();

        /**
         * <summary>
         * Initializes a TextField.
         * </summary>
         * <param name="text">The placeholder text</param>
         * <param name="fontSize">The font size to use</param>
         */
        public TextField(string text, int fontSize) {
            placeholder = new Label(text, 2 * fontSize);
            placeholder.text.color = Colors.lighterGrey;
            placeholder.SetFill(FillType.All);
            Add(placeholder);

            input = new Label("", 2 * fontSize);
            input.SetFill(FillType.All);
            Add(input);

            background = gameObject.AddComponent<UEImage>();
            background.color = Colors.grey;

            inputField = gameObject.AddComponent<UEInputField>();

            inputField.placeholder = placeholder.text;
            placeholder.text.alignByGeometry = false;
            placeholder.text.font = Resources.gameFontScuffed;

            inputField.textComponent = input.text;
            input.text.alignByGeometry = false;
            input.text.font = Resources.gameFontScuffed;

            inputField.onEndEdit.AddListener((string value) => {
                EventSystem.current.SetSelectedGameObject(null);
                onEndEdit.Invoke(value);
            });

            inputField.onValueChanged.AddListener((string value) => {
                onValueChanged.Invoke(value);
            });
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
