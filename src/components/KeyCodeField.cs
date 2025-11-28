using UnityEngine;

using UILib.Behaviours;
using UIButton = UILib.Components.Button;

namespace UILib.Components {
    /**
     * <summary>
     * A component which allows you to get
     * a `KeyCode` from the user.
     * This could be a key or mouse button.
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
         * An event for listening to when the user
         * actually inputs a key or presses a mouse button.
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
            : base(KeyAsString(value), fontSize)
        {
            this.value = value;

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
                        this.value = key;
                        SetText(KeyAsString(key));
                    }
                });
            });
        }

        /**
         * <summary>
         * Translates a `KeyCode` into a more user
         * friendly string.
         * </summary>
         * <param name="key">The `KeyCode` to translate</param>
         * <returns>The key as a user friendly value</returns>
         */
        public static string KeyAsString(KeyCode key) {
            switch (key) {
                case KeyCode.Alpha0: return "0";
                case KeyCode.Alpha1: return "1";
                case KeyCode.Alpha2: return "2";
                case KeyCode.Alpha3: return "3";
                case KeyCode.Alpha4: return "4";
                case KeyCode.Alpha5: return "5";
                case KeyCode.Alpha6: return "6";
                case KeyCode.Alpha7: return "7";
                case KeyCode.Alpha8: return "8";
                case KeyCode.Alpha9: return "9";

                // Special keys
                case KeyCode.BackQuote:    return "`";
                case KeyCode.Minus:        return "-";
                case KeyCode.Equals:       return "=";
                case KeyCode.LeftBracket:  return "[";
                case KeyCode.RightBracket: return "]";
                case KeyCode.Backslash:    return "\\";
                case KeyCode.Semicolon:    return ";";
                case KeyCode.Quote:        return "'";
                case KeyCode.Comma:        return ",";
                case KeyCode.Period:       return ".";
                case KeyCode.Slash:        return "/";
                case KeyCode.PageUp:       return "Page Up";
                case KeyCode.PageDown:     return "Page Down";

                // Modifiers
                case KeyCode.LeftControl:  return "Left Control";
                case KeyCode.RightControl: return "Right Control";
                case KeyCode.LeftShift:    return "Left Shift";
                case KeyCode.RightShift:   return "Right Shift";
                case KeyCode.LeftAlt:      return "Left Alt";
                case KeyCode.RightAlt:     return "Right Alt";
                case KeyCode.CapsLock:     return "Caps Lock";
                case KeyCode.ScrollLock:   return "Scroll Lock";

                // Arrows
                case KeyCode.UpArrow:      return "Up";
                case KeyCode.DownArrow:    return "Down";
                case KeyCode.LeftArrow:    return "Left";
                case KeyCode.RightArrow:   return "Right";

                // Mouse
                case KeyCode.Mouse0:       return "Left Mouse";
                case KeyCode.Mouse1:       return "Right Mouse";
                case KeyCode.Mouse2:       return "Middle Mouse";

                default: return key.ToString();
            }
        }
    }
}
