using UnityEngine;
using UnityEngine.UI;

namespace UILib {
    public static class Colors {
        public static Color white        = new Color(1f, 1f, 1f);
        public static Color black        = new Color(0f, 0f, 0f);
        public static Color darkGrey     = new Color(22f/255f, 22f/255f, 22f/255f);
        public static Color grey         = new Color(50f/255f, 50f/255f, 50f/255f);
        public static Color lightGrey    = new Color(80f/255f, 80f/255f, 80f/255f);
        public static Color lighterGrey  = new Color(134f/255f, 134f/255f, 134f/255f);
        public static Color lightestGrey = new Color(164f/255f, 164f/255f, 164f/255f);

        public static Color red          = new Color(204f/255f, 51f/255f, 51f/255f);
        public static Color lightRed     = new Color(166f/255f, 89f/255f, 89f/255f);

        public static ColorBlock colorBlock      = MakeBlock(grey, lightGrey);
        public static ColorBlock lightColorBlock = MakeBlock(lightGrey, lighterGrey);
        public static ColorBlock darkColorBlock  = MakeBlock(darkGrey, grey);
        public static ColorBlock redColorBlock   = MakeBlock(lightRed, red);

        private static ColorBlock MakeBlock(Color normal, Color selected) {
            return new ColorBlock() {
                    fadeDuration     = 0.1f,
                    colorMultiplier  = 1f,
                    normalColor      = normal,
                    selectedColor    = normal,
                    disabledColor    = normal,
                    highlightedColor = selected,
                    pressedColor     = selected,
            };
        }
    }
}
