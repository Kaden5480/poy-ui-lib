using UnityEngine;
using UEImage = UnityEngine.UI.Image;
using UESlider = UnityEngine.UI.Slider;

using UILib.Behaviours;
using UILib.Components;
using UILib.Layouts;
using UIButton = UILib.Components.Button;

namespace UILib.ColorPicker {
    /**
     * <summary>
     * The window containing the color picker tools.
     * </summary>
     */
    internal class ColorPickerWindow : Window {
        private ColorUpdater updater;

        private Slider hueSlider;
        private Area opacityArea;
        private Slider opacitySlider;

        internal static Shortcut toggleShortcut;

        internal static void UpdateShortcut(KeyCode shortcut) {
            toggleShortcut.SetShortcut(new[] { shortcut });
        }

        /**
         * <summary>
         * Initializes the color picker window.
         * </summary>
         */
        internal ColorPickerWindow() : base("Color Picker", 520f, 640f) {
            updater = new ColorUpdater();

            SetMinSize(520f, 640f);
            SetContentLayout(LayoutType.Vertical);
            SetElementSpacing(15);

            Area mainArea = CreateMainArea(250f);
            Add(mainArea);

            Area inputs = new Area();
            inputs.SetContentLayout(LayoutType.Horizontal);
            inputs.SetContentPadding(left: -26);
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
            }, updater, ColorUpdate.HSVFields);
            inputs.Add(updater.hsvArea);

            updater.hslArea = new ColorArea(new[] {
                new InputInfo("H", 0, 360, updater.refHue),
                new InputInfo("S", 0, 100, updater.refLSat),
                new InputInfo("L", 0, 100, updater.refLightness),
            }, updater, ColorUpdate.HSL);
            inputs.Add(updater.hslArea);

            Add(inputs);

            Area extraInputs = new Area();
            extraInputs.SetContentLayout(LayoutType.Horizontal);
            extraInputs.SetElementSpacing(70);
            extraInputs.SetSize(500f, 26f);

            (_, updater.hexField) = CreateExtra(extraInputs, "Hex");
            (opacityArea, updater.opacityField) = CreateExtra(extraInputs, "Opacity");

            Add(extraInputs);

            Add(new Area(0f, 15f));

            UIButton doneButton = new UIButton("Done", 20);
            doneButton.SetSize(100f, 30f);
            doneButton.onClick.AddListener(Hide);
            Add(doneButton);

            updater.Init();

            // Register shortcut
            toggleShortcut = UIRoot.AddShortcut(new[] { Config.openColorPicker.Value });
            toggleShortcut.onTrigger.AddListener(() => {
                if (isVisible == false || updater.current != null) {
                    OpenDetached();
                }
                else {
                    Hide();
                }
            });

            // Set the theme
            SetThisTheme(theme);
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
         * Opens the color picker in a "detached" mode.
         * </summary>
         */
        internal void OpenDetached() {
            Unlink();
            SetTheme(UIRoot.defaultTheme);

            updater.SetColor(Color.white);

            opacitySlider.Show();
            opacityArea.Show();

            SetName("Color Picker (Detached)");

            Show();
        }

        /**
         * <summary>
         * Updates the currently selected color field to
         * be a different one.
         * </summary>
         * <param name="field">The new color field to use</param>
         * <param name="theme">The theme to use</param>
         */
        internal void Link(ColorField field) {
            Unlink();
            updater.current = field;
            updater.SetColor(field.value);
            SetTheme(field.theme);

            if (field.allowOpacity == true) {
                opacitySlider.Show();
                opacityArea.Show();
            }
            else {
                opacitySlider.Hide();
                opacityArea.Hide();
            }

            SetName("Color Picker");
            Show();
        }

        /**
         * <summary>
         * Unlinks the currently selected color field.
         * </summary>
         */
        internal void Unlink() {
            if (updater.current == null) {
                return;
            }

            Color color = Colors.RGBA(
                updater.red, updater.green,
                updater.blue, updater.opacity
            );

            updater.current.SetValue(color);
            updater.current.onSubmit.Invoke(color);

            updater.current = null;
            updater.SetColor(Color.white);
        }

        /**
         * <summary>
         * Unlinks on hide.
         * </summary>
         */
        public override void Hide() {
            base.Hide();
            Unlink();
        }

        /**
         * <summary>
         * Checks to see if the color field is hidden.
         * </summary>
         */
        internal void Update() {
            if (updater.current == null) {
                return;
            }

            if (updater.current.gameObject.activeInHierarchy == false) {
                Hide();
            }
        }

#region UI Creation

        /**
         * <summary>
         * Creates an extra text field input.
         * </summary>
         * <param name="extraArea">The area to add this input to</param>
         * <param name="name">The name of the input</param>
         * <returns>The area and the text field within it</returns>
         */
        private (Area, TextField) CreateExtra(Area extraArea, string name) {
            Area area = new Area();
            area.SetSize(80f, 60f);
            area.SetContentLayout(LayoutType.Vertical);

            Label label = new Label(name, 20);
            label.SetSize(80f, 30f);
            area.Add(label);

            TextField textField = new TextField("", 20);
            textField.SetSize(80f, 30f);
            textField.SetSubmitMode(
                TextField.SubmitMode.Click
                | TextField.SubmitMode.Escape
            );
            area.Add(textField);

            extraArea.Add(area);

            return (area, textField);
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

#endregion

    }
}
