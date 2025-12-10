using System;

using UnityEngine;
using UEImage = UnityEngine.UI.Image;

using UILib.Components;

namespace UILib.ColorPicker {
    /**
     * <summary>
     * The types of color updates that can happen.
     * </summary>
     */
    internal enum ColorUpdate {
        None,
        RGB,
        HSVSliders,
        HSVFields,
        HSL,
        Hex,
        OpacitySlider,
        OpacityField,
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

        internal TextField hexField;
        internal TextField opacityField;

        internal UEImage hsvRect     { get => svPicker.background.image; }
        internal UEImage hsvSpectrum { get => hueSlider.background.image; }
        internal UEImage hsvOpacity  { get => opacitySlider.background.image; }

        /**
         * <summary>
         * Initializes some extra updates.
         * <see cref="ColorAreas"/> handle their own updates.
         * </summary>
         */
        internal void Init() {
            svPicker.onValueChanged.AddListener((Vector2 position) => {
                hue = hueSlider.value;
                vSat = position.x;
                val = position.y;
                Update(ColorUpdate.HSVSliders);
            });

            hueSlider.onValueChanged.AddListener((float value) => {
                hue = value;
                vSat = svPicker.value.x;
                val = svPicker.value.y;
                Update(ColorUpdate.HSVSliders);
            });

            opacitySlider.onValueChanged.AddListener((float value) => {
                opacity = value;
                Update(ColorUpdate.OpacitySlider);
            });

            // Extra inputs
            hexField.onInputChanged.AddListener((string value) => {
                if (value.StartsWith("#") == false) {
                    value = $"#{value}";
                }

                if (ColorUtility.TryParseHtmlString(value, out Color color) == true) {
                    red = (float) Math.Round(255f*color.r, 2);
                    green = (float) Math.Round(255f*color.g, 2);
                    blue = (float) Math.Round(255f*color.b, 2);
                    Update(ColorUpdate.Hex);
                }
            });
            hexField.SetPredicate((string value) => {
                if (value.StartsWith("#") == false) {
                    value = $"#{value}";
                }

                if (ColorUtility.TryParseHtmlString(value, out Color color) == true) {
                    red = (float) Math.Round(255f*color.r, 2);
                    green = (float) Math.Round(255f*color.g, 2);
                    blue = (float) Math.Round(255f*color.b, 2);
                    // Trigger RGB update on purpose to prefix with "#"
                    Update(ColorUpdate.RGB);
                    return true;
                }
                return false;
            });

            opacityField.onInputChanged.AddListener((string value) => {
                if (Validate(value, 0f, 100f, out float result) == true) {
                    opacity = result;
                    Update(ColorUpdate.OpacityField);
                }
            });
            opacityField.SetPredicate((string value) => {
                if (Validate(value, 0f, 100f, out float result) == true) {
                    opacity = result;
                    Update(ColorUpdate.OpacityField);
                    return true;
                }
                return false;
            });

            // Initialize
            red = 255f;
            opacity = 100f;
            Update(ColorUpdate.None);
        }

        /**
         * <summary>
         * Recalculates values based upon the type
         * of update which happened.
         * </summary>
         * <param name="update">The update which happened</param>
         */
        internal void Recalculate(ColorUpdate update) {
            // Opacity updates require nothing special
            if (update == ColorUpdate.OpacitySlider
                || update == ColorUpdate.OpacityField
            ) {
                return;
            }

            // RGB/Hex updates can be ignored in this switch,
            // as the values are all derived from it
            switch (update) {
                case ColorUpdate.HSVSliders:
                case ColorUpdate.HSVFields: {
                    Color rgb = Colors.HSV(hue, vSat, val);
                    red = 255f*rgb.r;
                    green = 255f*rgb.g;
                    blue = 255f*rgb.b;
                }; break;

                case ColorUpdate.HSL: {
                    Color rgb = Colors.HSL(hue, lSat, lightness);
                    red = 255f*rgb.r;
                    green = 255f*rgb.g;
                    blue = 255f*rgb.b;
                }; break;
            }

            // Recalculate RGB, HSV, and HSL
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
         * Tells the color updater that a certain update happened.
         * </summary>
         * <param name="update">The update which happened</param>
         */
        internal void Update(ColorUpdate update) {
            Recalculate(update);

            if (update != ColorUpdate.OpacitySlider) {
                opacitySlider.SetValue(opacity);
            }

            if (update != ColorUpdate.OpacityField) {
                opacityField.SetValue(opacity.ToString());
            }

            // Don't overwrite the hex input
            if (update != ColorUpdate.Hex) {
                hexField.SetValue(
                    "#" + ColorUtility.ToHtmlStringRGB(
                        Colors.RGB(red, green, blue)
                    ).ToLower()
                );
            }

            // Limit updates if just opacity
            if (update != ColorUpdate.OpacitySlider
                && update != ColorUpdate.OpacityField
            ) {
                // Update picker/slider positions
                if (update != ColorUpdate.HSVSliders) {
                    svPicker.SetValue(vSat, val);
                    hueSlider.SetValue(hue);
                }

                // Update input areas
                if (update != ColorUpdate.RGB) {
                    rgbArea.Update(new[] { red, green, blue });
                }
                if (update != ColorUpdate.HSVFields) {
                    hsvArea.Update(new[] { hue, vSat, val });
                }
                if (update != ColorUpdate.HSL) {
                    hslArea.Update(new[] { hue, lSat, lightness });
                }

                // Update visuals
                hsvRect.material.SetFloat("_Hue", hue);

                hsvOpacity.material.SetFloat("_Hue", hue);
                hsvOpacity.material.SetFloat("_Saturation", vSat);
                hsvOpacity.material.SetFloat("_Value", val);
            }
        }

        /**
         * <summary>
         * Validates the provided string is a float within
         * min and max.
         * </summary>
         * <param name="input">The input to validate</param>
         * <param name="min">The min value</param>
         * <param name="max">The max value</param>
         * <param name="result">The converted result if it is valid</param>
         */
        internal static bool Validate(string input, float min, float max, out float result) {
            if (float.TryParse(input, out result) == false) {
                return false;
            }

            result = (float) Math.Round(result, 2);

            if (result < min || result > max) {
                return false;
            }

            return true;
        }
    }
}
