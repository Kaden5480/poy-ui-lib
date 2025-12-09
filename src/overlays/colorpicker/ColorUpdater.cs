using UEImage = UnityEngine.UI.Image;

using UILib.Components;

namespace UILib.ColorPicker {
    /**
     * <summary>
     * The types of color updates that can happen.
     * </summary>
     */
    internal enum ColorUpdate {
        RGB,
        HSV,
        HSL,
    }

    /**
     * <summary>
     * Holds a single value and allows referencing it.
     * </summary>
     */
    internal class RefValue<T> {
        internal T value;
    }

    /**
     * <summary>
     * A class for dealing with updating colors in the UI.
     * </summary>
     */
    internal class ColorUpdater {
        // RGB (0-255)
        internal RefValue<float> refRed = new RefValue<float>();
        internal RefValue<float> refGreen = new RefValue<float>();
        internal RefValue<float> refBlue = new RefValue<float>();

        // Hue (0-360)
        internal RefValue<float> refHue = new RefValue<float>();

        // HSV specific (0-100)
        internal RefValue<float> refVSat = new RefValue<float>();
        internal RefValue<float> refValue = new RefValue<float>();

        // HSL specific (0-100)
        internal RefValue<float> refLSat = new RefValue<float>();
        internal RefValue<float> refLightness = new RefValue<float>();

        // Alpha/Opacity (0-100)
        internal RefValue<float> refOpacity = new RefValue<float>();


        internal float red       { get => refRed.value;       set => refRed.value = value; }
        internal float green     { get => refGreen.value;     set => refGreen.value = value; }
        internal float blue      { get => refBlue.value;      set => refBlue.value = value; }
        internal float hue       { get => refHue.value;       set => refHue.value = value; }
        internal float vSat      { get => refVSat.value;      set => refVSat.value = value; }
        internal float lSat      { get => refLSat.value;      set => refLSat.value = value; }
        internal float val       { get => refValue.value;     set => refValue.value = value; }
        internal float lightness { get => refLightness.value; set => refLightness.value = value; }
        internal float opacity   { get => refOpacity.value;   set => refOpacity.value = value; }

        // The UI components that need updating
        internal Picker svPicker;
        internal Slider hueSlider;
        internal Slider opacitySlider;

        internal ColorArea rgbArea;
        internal ColorArea hsvArea;
        internal ColorArea hslArea;

        internal UEImage hsvRect     { get => svPicker.background.image; }
        internal UEImage hsvSpectrum { get => hueSlider.background.image; }
        internal UEImage hsvOpacity  { get => opacitySlider.background.image; }

        /**
         * <summary>
         * Recalculates values based upon the type
         * of update which happened.
         * </summary>
         * <param name="update">The update which happened</param>
         */
        internal void Recalculate(ColorUpdate update) {
            // RGB updates can be ignored in this switch,
            // as the values are all derived from it
            switch (update) {
                case ColorUpdate.HSV: {
                    Color rgb = Colors.HSV(hue, vSat, val);
                    red = rgb.r;
                    green = rgb.g;
                    blue = rgb.b;
                }; break;
                case ColorUpdate.HSL: {
                    Color rgb = Colors.HSL(hue, lSat, lightness);
                    red = rgb.r;
                    green = rgb.g;
                    blue = rgb.b;
                }; break;
            }

            // Recalculate HSV and HSL
            Vector3 hsv = Colors.RGBToHSV(red, green, blue);
            hue = hsv.x;
            vSat = hsv.y;
            val = hsv.z;

            Vector3 hsl = Colors.RGBToHSL(red, green, blue);
            lSat = hsl.y;
            lightness = hsl.z;
        }

        /**
         * <summary>
         * Tells the color updater that a certain
         * update happened.
         * </summary>
         * <param name="update">The update which happened</param>
         */
        internal void Update(ColorUpdate update) {
            Recalculate(update);

            // Update picker/slider positions
            svPicker.SetValue(vSat, val);
            hueSlider.SetValue(hue);

            // Update visuals
            hsvRect.material.SetFloat("_Hue", hue);

            hsvOpacity.material.SetFloat("_Hue", hue);
            hsvOpacity.material.SetFloat("_Saturation", vSat);
            hsvOpacity.material.SetFloat("_Value", val);
        }
    }
}
