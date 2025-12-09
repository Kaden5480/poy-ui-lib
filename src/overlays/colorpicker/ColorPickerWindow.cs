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
            window = new Window("Color Picker", 550f, 600f);
            window.SetMinSize(550f, 600f);
            window.SetContentLayout(LayoutType.Vertical);
            window.SetElementSpacing(20);
            window.scrollView.background.color = UIRoot.defaultTheme.accentAlt;

            Area mainArea = CreateMainArea(250f);

            // Initialize controls
            svPicker.SetValue(100f, 100f);
            hueSlider.SetValue(0f);
            opacitySlider.SetValue(100f);
            UpdateColors();

            window.Add(mainArea);

            Area inputs = new Area();
            inputs.SetContentLayout(LayoutType.Horizontal);
            inputs.SetElementSpacing(50);
            inputs.SetSize(500f, 150f);

            Area rgbArea = CreateInputArea(new[] {
                new InputInfo("R", 0, 255),
                new InputInfo("G", 0, 255),
                new InputInfo("B", 0, 255),
            });
            inputs.Add(rgbArea);

            Area hsvArea = CreateInputArea(new[] {
                new InputInfo("H", 0, 360),
                new InputInfo("S", 0, 100),
                new InputInfo("V", 0, 100),
            });
            inputs.Add(hsvArea);

            Area hslArea = CreateInputArea(new[] {
                new InputInfo("H", 0, 360),
                new InputInfo("S", 0, 100),
                new InputInfo("L", 0, 100),
            });
            inputs.Add(hslArea);

            window.Add(inputs);
            window.Show();
        }

        /**
         * <summary>
         * A class holding information for a single input.
         * </summary>
         */
        private class InputInfo {
            internal string name;
            internal int min;
            internal int max;

            internal InputInfo(string name, int min, int max) {
                this.name = name;
                this.min = min;
                this.max = max;
            }
        }

        /**
         * <summary>
         * Creates one input control.
         * </summary>
         * <param name="input">The input to make</param>
         * <returns>The input control</returns>
         */
        private Area CreateInput(InputInfo input) {
            Area area = new Area();
            area.SetSize(90f, 26f);
            area.SetContentLayout(LayoutType.Horizontal);

            Label label = new Label($"{input.name}", 20);
            label.SetSize(30f, 26f);
            area.Add(label);

            TextField textField = new TextField("", 20);
            textField.SetSubmitMode(
                TextField.SubmitMode.Click
                | TextField.SubmitMode.Escape
            );
            textField.SetSize(60f, 26f);
            area.Add(textField);

            return area;
        }

        /**
         * <summary>
         * Creates a set of text field controls.
         * </summary>
         * <param name="inputs">The inputs to make controls with</param>
         * <returns>The controls</returns>
         */
        private Area CreateInputArea(InputInfo[] inputs) {
            Area area = new Area();
            area.SetSize(90f, 26f*inputs.Length + 10*(inputs.Length-1));
            area.SetContentLayout(LayoutType.Vertical);
            area.SetElementSpacing(10);

            foreach (InputInfo input in inputs) {
                area.Add(CreateInput(input));
            }

            return area;
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
