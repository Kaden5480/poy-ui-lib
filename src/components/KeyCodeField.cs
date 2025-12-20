using UnityEngine;
using UnityEngine.Events;

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
         * cancels inputting a key/mouse button.
         * </summary>
         */
        public UnityEvent onCancel { get; } = new UnityEvent();

        /**
         * <summary>
         * An event for listening to when the user
         * actually inputs a key or presses a mouse button.
         *
         * Passes the `KeyCode` that was read to listeners.
         * </summary>
         */
        public ValueEvent<KeyCode> onValueChanged { get; } = new ValueEvent<KeyCode>();

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
                ValueEvent<KeyCode> ev = UIRoot.inputOverlay.Request(theme);
                if (ev == null) {
                    return;
                }

                // Forward to listeners and
                // update text if a valid key was
                // received
                ev.AddListener((KeyCode key) => {
                    if (key == KeyCode.None) {
                        onCancel.Invoke();
                    }
                    else {
                        SetValue(key);
                        onValueChanged.Invoke(key);
                    }
                });
            });
        }

        /**
         * <summary>
         * Sets the current value stored in this field.
         * </summary>
         * <param name="value">The value to set</param>
         */
        public void SetValue(KeyCode value) {
            this.value = value;
            SetText(KeyAsString(value));
        }

        /**
         * <summary>
         * Translates a `KeyCode` into a more user
         * friendly string.
         *
         * This is used internally to update the key
         * being displayed, but it's public because
         * it's a useful method.
         * </summary>
         * <param name="key">The `KeyCode` to translate</param>
         * <returns>The key as a user friendly value</returns>
         */
        public static string KeyAsString(KeyCode key) {
            switch (key) {
                case KeyCode.None:   return "";
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
                case KeyCode.Mouse3:       return "Mouse Button 4";
                case KeyCode.Mouse4:       return "Mouse Button 5";
                case KeyCode.Mouse5:       return "Mouse Button 6";
                case KeyCode.Mouse6:       return "Mouse Button 7";

                default: return key.ToString();
            }
        }
    }
}
