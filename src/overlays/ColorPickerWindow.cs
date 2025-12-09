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
            window.SetContentLayout(LayoutType.Horizontal);
            window.SetElementSpacing(20);
            window.scrollView.background.color = Colors.RGB(41, 41, 41);

            // Saturation/value picker
            svPicker = new Picker();
            svPicker.background.image.material = new Material(
                Resources.hsvRect
            );
            svPicker.SetSize(250f, 250f);
            svPicker.SetMinValues(0f, 0f);
            svPicker.SetMaxValues(100f, 100f);
            svPicker.onValueChanged.AddListener(delegate {
                UpdateColors();
            });
            window.Add(svPicker);

            hueSlider = new Slider(
                0f, 360f, UESlider.Direction.BottomToTop
            );
            hueSlider.background.image.material = new Material(
                Resources.hsvSpectrum
            );
            hueSlider.SetSize(50f, 250f);
            hueSlider.onValueChanged.AddListener(delegate {
                UpdateColors();
            });
            window.Add(hueSlider);

            opacitySlider = new Slider(
                0f, 100f, UESlider.Direction.BottomToTop
            );
            opacitySlider.background.image.material = new Material(
                Resources.hsvOpacity
            );
            opacitySlider.SetSize(50f, 250f);
            opacitySlider.onValueChanged.AddListener(delegate {
                UpdateColors();
            });
            window.Add(opacitySlider);

            // Set up images and some customisations
            hueSlider.handleImage.image.sprite = null;
            hueSlider.handleImage.SetSize(10f, 10f);
            hueSlider.fillImage.SetColor(Color.clear);

            opacitySlider.background.image.SetMaterialDirty();
            opacitySlider.handleImage.image.sprite = null;
            opacitySlider.handleImage.SetSize(10f, 10f);
            opacitySlider.fillImage.SetColor(Color.clear);

            hsvRect     = svPicker.background.image;
            hsvSpectrum = hueSlider.background.image;
            hsvOpacity  = opacitySlider.background.image;

            // Fix masking
            hsvRect.maskable = false;
            hsvSpectrum.maskable = false;
            hsvOpacity.maskable = false;

            window.Show();
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
