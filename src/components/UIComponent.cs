using System;

namespace UILib {
    /**
     * <summary>
     * The base class which components that can be placed
     * on the UI inherit from.
     * </summary>
     */
    public abstract class UIComponent : UIObject {
        /**
         * <summary>
         * A helper method for cutting down the size of strings.
         *
         * If ellipses are added, the text will be cut down even more
         * to ensure it doesn't exceed `max` characters, i.e. `max` characters
         * is the maximum possible size of the string.
         * </summary>
         * <param name="text">The text to clamp</param>
         * <param name="max">The maximum permitted length of the text</param>
         * <param name="addEllipses">Whether to add ellipses</param>
         */
        public string ClampString(string text, int max, bool addEllipses = true) {
            string ellipses = "...";

            if (String.IsNullOrEmpty(text) == true) {
                return text;
            }

            // If the string is already within bounds, just return it
            if (text.Length <= max) {
                return text;
            }

            // Handle ellipses
            if (addEllipses == true) {
                return text.Substring(0,
                    Math.Min(text.Length, max - ellipses.Length)
                ) + ellipses;
            }

            return text.Substring(0,
                Math.Min(text.Length, max)
            );
        }
    }
}
