using UnityEngine;

namespace UILib {
    /**
     * <summary>
     * A class for helping with generating colors
     * using a variety of color representations.
     * </summary>
     */
    public static class Colors {

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
        public static Color RGBA(float r, float g, float b, float a) {
            return new Color(
                r/255f,
                g/255f,
                b/255f,
                a/100f
            );
        }

        /**
         * <summary>
         * Creates a `Color` from red, green, and blue
         * components.
         * </summary>
         * <param name="r">The red value (0-255)</param>
         * <param name="g">The green value (0-255)</param>
         * <param name="b">The blue value (0-255)</param>
         * <returns>The `Color`</returns>
         */
        public static Color RGB(float r, float g, float b) {
            return RGBA(r, g, b, 100);
        }

#endregion

#region Hex

        /**
         * <summary>
         * Creates a `Color` from a hex value of the form
         * 0xRRGGBBAA.
         * </summary>
         * <param name="hexA">The hex value (including the alpha component)</param>
         * <returns>The `Color`</returns>
         */
        public static Color HexA(int hexA) {
            int r = 0xff & (hexA >> 24);
            int g = 0xff & (hexA >> 16);
            int b = 0xff & (hexA >> 8);
            int a = 0xff & (hexA >> 0);

            return RGBA(r, g, b, 100*(a/255));
        }

        /**
         * <summary>
         * Creates a `Color` from a hex value of the form
         * 0xRRGGBB.
         * </summary>
         * <param name="hex">The hex value</param>
         * <returns>The `Color`</returns>
         */
        public static Color Hex(int hex) {
            return HexA((hex << 8) | 0xff);
        }

        /**
         * <summary>
         * Converts RGB values to a hex value in the form 0xRRGGBB.
         * </summary>
         * <param name="r">The red component (0-255)</param>
         * <param name="g">The green component (0-255)</param>
         * <param name="b">The blue component (0-255)</param>
         * <returns>The hex value</returns>
         */
        public static int RGBToHex(int r, int g, int b) {
            return r << 16
                | g << 8
                | b << 0;
        }

        /**
         * <summary>
         * Converts RGB values to a hex value in the form 0xRRGGBBAA.
         * </summary>
         * <param name="r">The red component (0-255)</param>
         * <param name="g">The green component (0-255)</param>
         * <param name="b">The blue component (0-255)</param>
         * <param name="a">The alpha component (0-100)</param>
         * <returns>The hex value</returns>
         */
        public static int RGBAToHex(int r, int g, int b, int a) {
            return r << 24
                | g << 16
                | b << 8
                | (255*(a/100)) << 0;
        }

#endregion

#region HSL

        /**
         * <summary>
         * Function for helping with hsl -> rgb conversions.
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

        /**
         * <summary>
         * Converts RGB to HSL.
         *
         * The returned HSL will be:
         * H: [0, 360]
         * S: [0, 100]
         * L: [0, 100]
         *
         * See:
         * https://en.wikipedia.org/wiki/HSL_and_HSV#Color_conversion_formulae
         * </summary>
         * <param name="r">The red component (0-255)</param>
         * <param name="g">The green component (0-255)</param>
         * <param name="b">The blue component (0-255)</param>
         * <returns>The HSL values</returns>
         */
        public static Vector3 RGBToHSL(float r, float g, float b) {
            r /= 255f;
            g /= 255f;
            b /= 255f;

            float cMax = Mathf.Max(Mathf.Max(r, g), b);
            float cMin = Mathf.Min(Mathf.Min(r, g), b);

            float c = cMax - cMin;

            float hue = 0f;
            float saturation = 0f;
            float lightness = (cMax + cMin)/2f;

            // Calculate hue
            if (c == 0f) { hue = 0f; }
            else if (cMax == r) { hue = 60f * ((g - b)/c % 6f); }
            else if (cMax == g) { hue = 60f * ((b - r)/c + 2f); }
            else if (cMax == b) { hue = 60f * ((r - g)/c + 4f); }

            if (hue < 0) { hue += 360f; }

            // Calculate saturation
            if (c == 0f || lightness == 0f || lightness == 1f) {
                saturation = 0f;
            }
            else {
                saturation = c / (1f - Mathf.Abs(2f*lightness - 1f));
            }

            return new Vector3(hue, 100f*saturation, 100f*lightness);
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

        /**
         * <summary>
         * Converts RGB to HSV.
         *
         * The returned HSV will be:
         * H: [0, 360]
         * S: [0, 100]
         * V: [0, 100]
         *
         * See:
         * https://en.wikipedia.org/wiki/HSL_and_HSV#Color_conversion_formulae
         * </summary>
         * <param name="r">The red component (0-255)</param>
         * <param name="g">The green component (0-255)</param>
         * <param name="b">The blue component (0-255)</param>
         * <returns>The HSV values</returns>
         */
        public static Vector3 RGBToHSV(float r, float g, float b) {
            r /= 255f;
            g /= 255f;
            b /= 255f;

            float cMax = Mathf.Max(Mathf.Max(r, g), b);
            float cMin = Mathf.Min(Mathf.Min(r, g), b);

            float c = cMax - cMin;

            float hue = 0f;
            float saturation = 0f;
            float lightness = (cMax + cMin)/2f;

            // Calculate hue
            if (c == 0f) { hue = 0f; }
            else if (cMax == r) { hue = 60f * ((g - b)/c % 6f); }
            else if (cMax == g) { hue = 60f * ((b - r)/c + 2f); }
            else if (cMax == b) { hue = 60f * ((r - g)/c + 4f); }

            if (hue < 0) { hue += 360f; }

            // Calculate saturation
            if (cMax == 0f) { saturation = 0f; }
            else { saturation = c / cMax; }

            return new Vector3(hue, 100f*saturation, 100f*cMax);
        }

#endregion

    }
}
