using UnityEngine;
using UnityEngine.UI;

namespace UILib {
    public static class Colors {
        public static Color black       = new Color(0f, 0f, 0f);
        public static Color grey        = new Color(12f/255f, 20f/255f, 20f/255f);
        public static Color lightGrey   = new Color(134f/255f, 134f/255f, 134f/255f);
        public static Color lighterGrey = new Color(164f/255f, 164f/255f, 164f/255f);

        public static ColorBlock colorBlock = new ColorBlock() {
                fadeDuration     = 0.1f,
                colorMultiplier  = 1f,
                normalColor      = Colors.lightGrey,
                selectedColor    = Colors.lightGrey,
                disabledColor    = Colors.lightGrey,
                highlightedColor = Colors.lighterGrey,
                pressedColor     = Colors.lighterGrey,
            };
    }
}
