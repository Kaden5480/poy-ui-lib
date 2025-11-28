using UnityEngine;

using UILib.Behaviours;
using UIButton = UILib.Components.Button;

namespace UILib.Components {
    /**
     * <summary>
     * A component which allows you to get
     * a `KeyCode` from the user.
     * </summary>
     */
    public class KeyCodeField : UIButton {
        /**
         * <summary>
         * The current value stored in this field.
         * </summary>
         */
        public KeyCode value { get; private set; }

        /**
         * <summary>
         * An event for listening to when
         * the user actually inputs a key.
         *
         * Passes the `KeyCode` that was read to listeners.
         * Will pass `KeyCode.None` if the user cancelled
         * the input
         * </summary>
         */
        public KeyCodeEvent onValueChanged { get; } = new KeyCodeEvent();

        /**
         * <summary>
         * Initializes the keycode field.
         * </summary>
         * <param name="value">The default value of this field</param>
         * <param name="fontSize">The font size to use</param>
         */
        public KeyCodeField(KeyCode value, int fontSize)
            : base(value.ToString(), fontSize)
        {
            // When clicked, request an input
            onClick.AddListener(() => {
                KeyCodeEvent ev = UIRoot.inputOverlay.Request();
                if (ev == null) {
                    return;
                }

                // Forward to listeners and
                // update text if a valid key was
                // received
                ev.AddListener((KeyCode key) => {
                    onValueChanged.Invoke(key);
                    if (key != KeyCode.None) {
                        SetText(key.ToString());
                    }
                });
            });
        }
    }
}
