using UnityEngine;
using UEImage = UnityEngine.UI.Image;
using UESlider = UnityEngine.UI.Slider;

using UILib.Components;
using UILib.Layouts;

namespace UILib {
    /**
     * <summary>
     * The window containing the color picker tools.
     * </summary>
     */
    internal class ColorPickerWindow {
        private Window window;

        private Picker svPicker;
        private Slider hueSlider;
        private Slider opacitySlider;

        private UEImage hsvRect;
        private UEImage hsvSpectrum;
        private UEImage hsvOpacity;

        /**
         * <summary>
         * Initializes the color picker window.
         * </summary>
         */
        internal ColorPickerWindow() {
            window = new Window("Color Picker", 500f, 400f);
            window.SetMinSize(500f, 400f);
            window.SetContentLayout(LayoutType.Vertical);
            window.scrollView.background.color = Colors.RGB(41, 41, 41);

            Area mainArea = CreateMainArea(250f);
            mainArea.SetSize(390f, 250f);

            // Initialize controls
            svPicker.SetValue(100f, 100f);
            hueSlider.SetValue(0f);
            opacitySlider.SetValue(100f);
            UpdateColors();

            window.Add(mainArea);

            window.Show();
        }

        /**
         * <summary>
         * Creates the main color picker area.
         * </summary>
         * <returns>The area</returns>
         */
        private Area CreateMainArea(float height) {
            Area area = new Area();
            area.SetContentLayout(LayoutType.Horizontal);
            area.SetElementSpacing(20);

            // Saturation/value picker
            svPicker = new Picker();
            svPicker.background.image.material = new Material(
                Resources.hsvRect
            );
            svPicker.SetSize(height, height);
            svPicker.SetMinValues(0f, 0f);
            svPicker.SetMaxValues(100f, 100f);
            svPicker.onValueChanged.AddListener(delegate {
                UpdateColors();
            });
            area.Add(svPicker);

            // Hue slider
            hueSlider = new Slider(
                0f, 360f, UESlider.Direction.BottomToTop
            );
            hueSlider.background.image.material = new Material(
                Resources.hsvSpectrum
            );
            hueSlider.SetSize(50f, height);
            hueSlider.onValueChanged.AddListener(delegate {
                UpdateColors();
            });
            area.Add(hueSlider);

            // Opacity slider
            opacitySlider = new Slider(
                0f, 100f, UESlider.Direction.BottomToTop
            );
            opacitySlider.background.image.material = new Material(
                Resources.hsvOpacity
            );
            opacitySlider.SetSize(50f, height);
            opacitySlider.onValueChanged.AddListener(delegate {
                UpdateColors();
            });
            area.Add(opacitySlider);

            // Set up images and some customisations
            hueSlider.handle.image.sprite = null;
            hueSlider.handle.SetSize(10f, 10f);
            hueSlider.fill.SetColor(Color.clear);

            opacitySlider.handle.image.sprite = null;
            opacitySlider.handle.SetSize(10f, 10f);
            opacitySlider.fill.SetColor(Color.clear);

            // Track the images for later updates
            hsvRect = svPicker.background.image;
            hsvSpectrum = hueSlider.background.image;
            hsvOpacity = opacitySlider.background.image;

            // Masking is disabled for now because
            // shaders are kind of a nightmare
            hsvRect.maskable = false;
            hsvSpectrum.maskable = false;
            hsvOpacity.maskable = false;

            return area;
        }

        /**
         * <summary>
         * Updates the displayed colors.
         * </summary>
         */
        private void UpdateColors() {
            hsvRect.material.SetFloat("_Hue", hueSlider.value);
            hsvOpacity.material.SetFloat("_Hue", hueSlider.value);

            Vector2 sv = svPicker.value;
            hsvOpacity.material.SetFloat("_Saturation", sv.x);
            hsvOpacity.material.SetFloat("_Value", sv.y);
        }
    }
}
