using UnityEngine;

namespace UILib {
    /**
     * <summary>
     * A class for helping with generating colors
     * using a variety of color representations.
     * </summary>
     */
    public static class Colors {

#region HSL

        /**
         * <summary>
         * Function for helping with hsl conversions.
         *
         * See:
         * https://en.wikipedia.org/wiki/HSL_and_HSV#Color_conversion_formulae
         * </summary>
         */
        private static float HSLF(float h, float s, float l, int n) {
            float a = s * Mathf.Min(l, 1 - l);
            float k = (n + h/30) % 12;

            return l - a * Mathf.Max(-1,
                Mathf.Min(Mathf.Min(k-3, 9-k), 1)
            );
        }

        /**
         * <summary>
         * Creates a `Color` from hue, saturation, lightness, and alpha.
         * </summary>
         * <param name="h">The hue (0-360)</param>
         * <param name="s">The saturation (0-100)</param>
         * <param name="l">The lightness (0-100)</param>
         * <param name="a">The alpha (0-100)</param>
         * <returns>The `Color`</returns>
         */
        public static Color HSLA(float h, float s, float l, float a) {
            s /= 100f;
            l /= 100f;
            a /= 100f;

            float r = HSLF(h, s, l, 0);
            float g = HSLF(h, s, l, 8);
            float b = HSLF(h, s, l, 4);

            return new Color(r, g, b, a);
        }

        /**
         * <summary>
         * Creates a `Color` from hue, saturation, and lightness.
         * </summary>
         * <param name="h">The hue (0-360)</param>
         * <param name="s">The saturation (0-100)</param>
         * <param name="l">The lightness (0-100)</param>
         * <returns>The `Color`</returns>
         */
        public static Color HSL(float h, float s, float l) {
            return HSLA(h, s, l, 100f);
        }


#endregion

#region HSV

        /**
         * <summary>
         * Function for helping with hsv conversions.
         *
         * See:
         * https://en.wikipedia.org/wiki/HSL_and_HSV#Color_conversion_formulae
         * </summary>
         */
        private static float HSVF(float h, float s, float v, int n) {
            float k = (n + h/60) % 6;

            return (v - v * s * Mathf.Max(0,
                Mathf.Min(Mathf.Min(k, 4 - k), 1)
            ));
        }

        /**
         * <summary>
         * Creates a `Color` from hue, saturation, value, and alpha.
         * </summary>
         * <param name="h">The hue (0-360)</param>
         * <param name="s">The saturation (0-100)</param>
         * <param name="v">The value (0-100)</param>
         * <param name="a">The alpha (0-100)</param>
         * <returns>The `Color`</returns>
         */
        public static Color HSVA(float h, float s, float v, float a) {
            s /= 100f;
            v /= 100f;
            a /= 100f;

            float r = HSVF(h, s, v, 5);
            float g = HSVF(h, s, v, 3);
            float b = HSVF(h, s, v, 1);

            return new Color(r, g, b, a);
        }

        /**
         * <summary>
         * Creates a `Color` from hue, saturation, and value.
         * </summary>
         * <param name="h">The hue (0-360)</param>
         * <param name="s">The saturation (0-100)</param>
         * <param name="v">The value (0-100)</param>
         * <returns>The `Color`</returns>
         */
        public static Color HSV(float h, float s, float v) {
            return HSVA(h, s, v, 100f);
        }

#endregion

#region RGB

        /**
         * <summary>
         * Creates a `Color` from red, green, blue, and alpha
         * components.
         * </summary>
         * <param name="r">The red value (0-255)</param>
         * <param name="g">The green value (0-255)</param>
         * <param name="b">The blue value (0-255)</param>
         * <param name="a">The alpha value (0-100)</param>
         * <returns>The `Color`</returns>
         */
        public static Color RGBA(int r, int g, int b, int a) {
            return new Color(
                ((float) r)/255f,
                ((float) g)/255f,
                ((float) b)/255f,
                ((float) a)/100f
            );
        }

        /**
         * <summary>
         * Creates a Color from red, green, blue, and alpha
         * components.
         * </summary>
         * <param name="r">The red value (0-255)</param>
         * <param name="g">The green value (0-255)</param>
         * <param name="b">The blue value (0-255)</param>
         * <returns>The `Color`</returns>
         */
        public static Color RGB(int r, int g, int b) {
            return RGBA(r, g, b, 100);
        }

#endregion

    }
}
