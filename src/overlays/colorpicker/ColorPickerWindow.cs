using UnityEngine;
using UEImage = UnityEngine.UI.Image;
using UESlider = UnityEngine.UI.Slider;

using UILib.Components;
using UILib.Layouts;

namespace UILib.ColorPicker {
    /**
     * <summary>
     * The window containing the color picker tools.
     * </summary>
     */
    internal class ColorPickerWindow : Window {
        private ColorUpdater updater;

        private Slider hueSlider;
        private Slider opacitySlider;

        /**
         * <summary>
         * Initializes the color picker window.
         * </summary>
         */
        internal ColorPickerWindow() : base("Color Picker", 550f, 600f) {
            updater = new ColorUpdater();

            SetMinSize(550f, 600f);
            SetContentLayout(LayoutType.Vertical);
            SetElementSpacing(20);

            Area mainArea = CreateMainArea(250f);
            Add(mainArea);

            Area inputs = new Area();
            inputs.SetContentLayout(LayoutType.Horizontal);
            inputs.SetElementSpacing(50);
            inputs.SetSize(500f, 150f);

            updater.rgbArea = new ColorArea(new[] {
                new InputInfo("R", 0, 255, updater.refRed),
                new InputInfo("G", 0, 255, updater.refGreen),
                new InputInfo("B", 0, 255, updater.refBlue),
            }, updater, ColorUpdate.RGB);
            inputs.Add(updater.rgbArea);

            updater.hsvArea = new ColorArea(new[] {
                new InputInfo("H", 0, 360, updater.refHue),
                new InputInfo("S", 0, 100, updater.refVSat),
                new InputInfo("V", 0, 100, updater.refValue),
            }, updater, ColorUpdate.HSV);
            inputs.Add(updater.hsvArea);

            updater.hslArea = new ColorArea(new[] {
                new InputInfo("H", 0, 360, updater.refHue),
                new InputInfo("S", 0, 100, updater.refLSat),
                new InputInfo("L", 0, 100, updater.refLightness),
            }, updater, ColorUpdate.HSL);
            inputs.Add(updater.hslArea);

            Add(inputs);

            updater.Init();

            // Set the theme
            SetThisTheme(theme);

            Show();
        }

        /**
         * <summary>
         * Allows setting the theme of the color picker window.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {
            base.SetThisTheme(theme);

            if (scrollView == null || hueSlider == null) {
                return;
            }

            Color bg = theme.accentAlt;
            bg.a = theme.windowOpacity;
            scrollView.background.color = bg;

            hueSlider.background.SetColor(Color.white);
            hueSlider.fill.SetColor(Color.clear);

            opacitySlider.background.SetColor(Color.white);
            opacitySlider.fill.SetColor(Color.clear);
        }

        /**
         * <summary>
         * Creates the main color picker area.
         * </summary>
         * <returns>The area</returns>
         */
        private Area CreateMainArea(float height) {
            Area area = new Area();
            area.SetSize(390f, 250f);
            area.SetContentLayout(LayoutType.Horizontal);
            area.SetElementSpacing(20);

            // Saturation/value picker
            Picker svPicker = new Picker();
            svPicker.background.image.material = new Material(
                Resources.hsvRect
            );
            svPicker.SetSize(height, height);
            svPicker.SetMinValues(0f, 0f);
            svPicker.SetMaxValues(100f, 100f);
            area.Add(svPicker);

            // Hue slider
            hueSlider = new Slider(
                0f, 359.99f, UESlider.Direction.BottomToTop
            );
            hueSlider.background.image.material = new Material(
                Resources.hsvSpectrum
            );
            hueSlider.SetSize(50f, height);
            area.Add(hueSlider);

            // Opacity slider
            opacitySlider = new Slider(
                0f, 100f, UESlider.Direction.BottomToTop
            );
            opacitySlider.background.image.material = new Material(
                Resources.hsvOpacity
            );
            opacitySlider.SetSize(50f, height);
            area.Add(opacitySlider);

            // Set up images and some customisations
            hueSlider.handle.image.sprite = null;
            hueSlider.handle.SetSize(10f, 10f);

            opacitySlider.handle.image.sprite = null;
            opacitySlider.handle.SetSize(10f, 10f);

            updater.svPicker = svPicker;
            updater.hueSlider = hueSlider;
            updater.opacitySlider = opacitySlider;

            // Masking is disabled for now because
            // shaders are kind of a nightmare
            updater.hsvRect.maskable = false;
            updater.hsvSpectrum.maskable = false;
            updater.hsvOpacity.maskable = false;

            return area;
        }
    }
}
