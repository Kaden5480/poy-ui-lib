using System.Collections.Generic;

using UnityEngine;
using SliderDirection = UnityEngine.UI.Slider.Direction;

using UILib;
using UILib.Components;
using UILib.Layouts;
using UILib.Notifications;
using UIButton = UILib.Components.Button;

namespace UILibExamples {
    /**
     * <summary>
     * A somewhat detailed example showing
     * how the different inputs work.
     *
     * This is not a complete example, it's
     * worth referring to the API reference
     * for the full details. It will give you
     * a good headstart on handling inputs though.
     * </summary>
     */
    public class Inputs : Example {
        private Window window;

        public Inputs() {
            window = new Window("Inputs", 800f, 600f);
            window.SetContentLayout(LayoutType.Vertical);
            window.SetContentPadding(top: 50, bottom: 50);
            window.SetElementSpacing(20);

#region Buttons/Toggles

            // A button, can receive click events
            UIButton button = new UIButton("Button", 20);
            button.onClick.AddListener(() => {
                Notifier.Notify("Button", "Button was clicked");
            });
            button.SetSize(200f, 40f);
            window.Add(button);

            UIButton notifyButton = new UIButton("Send big notification", 20);
            notifyButton.onClick.AddListener(() => {
                Notifier.Notify(
                    "Here is an unnecessarily long title to see how UILib handles"
                    + " overly large notification titles. This is important for testing",

                    "Here is the beginning of a really long message to send through Notifier.Notify."
                    + " This is to ensure that UILib can handle messages which are really long."
                    + " In theory, the message should be truncated and end with three \".\" instead."
                    + " If this isn't the case, then something is clearly going wrong with Notifier's"
                    + " logic, and that would be a bug that needs fixing."
                    + " Themes can also configure their own max message limits as well, so that's useful.",

                    NotificationType.Error
                );
            });
            notifyButton.SetSize(200f, 40f);
            window.Add(notifyButton);

            // A simple toggle, like a button but
            // stores an on/off state
            Toggle toggle = new Toggle();
            toggle.SetSize(40f, 40f);
            toggle.onValueChanged.AddListener((bool value) => {
                Notifier.Notify("Toggle", $"Value was changed to: {value}");
            });
            window.Add(toggle);

#endregion

#region Color Field

            Label colorLabel = new Label("Color: #ffffffff", 20);
            colorLabel.SetSize(200f, 40f);
            window.Add(colorLabel);

            ColorField colorField = new ColorField(Color.white);
            colorField.SetSize(40f, 40f);
            colorField.onValueChanged.AddListener((Color color) => {
                string hex = ColorUtility.ToHtmlStringRGBA(color).ToLower();
                colorLabel.SetText($"Color: #{hex}");
            });
            colorField.onSubmit.AddListener((Color color) => {
                string hex = ColorUtility.ToHtmlStringRGBA(color).ToLower();
                Notifier.Notify("ColorField", $"You picked: #{hex}");
            });
            window.Add(colorField);

            // Themes can also be inherited from color fields
            // to the color picker
            Theme customTheme = new Theme() {
                foreground = Colors.HSL(230, 60, 60),
                accentAlt = Colors.HSL(230, 40, 40),
            };

            ColorField colorField2 = new ColorField(Color.red);
            colorField2.SetSize(40f, 40f);
            colorField2.SetTheme(customTheme);

            // You can also prevent opacity from being modified
            colorField2.AllowOpacity(false);

            window.Add(colorField2);

#endregion

#region Sliders

            // The default slider, horizontal and from 0-1
            Label sliderLabel = new Label("Slider: 0", 20);
            sliderLabel.SetSize(200f, 40f);
            window.Add(sliderLabel);

            Slider slider = new Slider();
            slider.SetSize(200f, 10f);
            // Update the label whenever the value changes
            slider.onValueChanged.AddListener((float value) => {
                sliderLabel.SetText($"Slider: {value}");
            });
            window.Add(slider);

            // A custom slider, which has different limits
            // and appears vertically
            Label sliderVLabel = new Label("Vertical Slider: 0", 20);
            sliderVLabel.SetSize(200f, 40f);
            window.Add(sliderVLabel);

            // Min of 10, max of 100, bottom to top
            Slider verticalSlider = new Slider(
                10f, 100f, SliderDirection.BottomToTop
            );
            verticalSlider.SetSize(10f, 200f);
            // Update the label whenever the value changes
            verticalSlider.onValueChanged.AddListener((float value) => {
                sliderVLabel.SetText($"Vertical Slider: {value}");
            });
            window.Add(verticalSlider);

            // A 2D picker
            Label pickerLabel = new Label("Picker: (0, 0)", 20);
            pickerLabel.SetSize(200f, 40f);
            window.Add(pickerLabel);

            Picker picker = new Picker();
            picker.SetSize(200f, 200f);

            // You can even specify custom limits (the default ranges from
            // (0, 0) to (1, 1))
            picker.SetMinValues(10f, 20f);
            picker.SetMaxValues(15f, 80f);

            picker.onValueChanged.AddListener((Vector2 position) => {
                pickerLabel.SetText($"Picker: {position}");
            });
            window.Add(picker);

#endregion

#region Input Fields

            // A simple field for receiving some text
            // from the user
            TextField textField = new TextField("Enter text", 20);
            textField.SetSize(200f, 40f);
            textField.SetPredicate((string value) => {
                if (value.Length > 4) {
                    return true;
                }

                Notifier.Notify("TextField", "You must enter at least 4 characters!");
                return false;
            });
            textField.onInputChanged.AddListener((string value) => {
                Notifier.Notify("TextField Changed", $"You are entering: {value}");
            });
            textField.onCancel.AddListener(() => {
                Notifier.Notify("TextField Cancel", "You cancelled the input");
            });
            textField.onInvalidSubmit.AddListener((string value) => {
                Notifier.Notify("TextField Invalid", $"Invalid data: {value}");
            });
            textField.onValidSubmit.AddListener((string value) => {
                Notifier.Notify("TextField Submitted", $"Submitted value: {value}");
            });
            window.Add(textField);

            // A field which is specialised to handle
            // KeyCode inputs
            // When this is selected, an overlay pops
            // up and waits for an input
            KeyCodeField keyInput = new KeyCodeField(KeyCode.A, 20);
            keyInput.SetSize(200f, 40f);
            // This will only run if the user entered a key/mouse button
            // Not when the input was cancelled
            keyInput.onValueChanged.AddListener((KeyCode value) => {
                Notifier.Notify("KeyCodeField", $"You entered: {value}");
            });

            // If you want to see when the user cancelled
            // an input, use this listener
            keyInput.onCancel.AddListener(() => {
                Notifier.Notify("KeyCodeField", "You cancelled entering a key");
            });
            window.Add(keyInput);

#endregion

#region Dropdowns

            // Dropdowns can accept any type
            Dropdown<string> dropdown = new Dropdown<string>(
                "Option A", 20
            );
            dropdown.SetSize(200f, 40f);

            // Don't forget to add the default option as well
            dropdown.AddOption("Option A");

            // You can add options individually
            dropdown.AddOption("Option B");

            // Or as a list
            dropdown.AddOptions(new[] {
                "Option C", "Option D", "Option E",
            });

            // You can also choose how many options you want
            // the dropdown to display
            dropdown.SetOptionCount(2);
            window.Add(dropdown);

            // Dropdown of numbers
            Dropdown<int> dropdownInt = new Dropdown<int>(
                1, 20, "One"
            );
            dropdownInt.SetSize(200f, 40f);
            dropdownInt.SetOptionCount(4);

            // You can even specify dictionaries if you need
            // the options to have custom display names
            dropdownInt.AddOptions(new Dictionary<string, int>() {
                { "One",   1 },
                { "Two",   2 },
                { "Three", 3 },
                { "Four",  4 },
                { "Five",  5 },
                { "Six",   6 },
            });

            dropdownInt.onValueChanged.AddListener((int value) => {
                Notifier.Notify("Int Dropdown", $"You selected an option with this value: {value}");
            });
            window.Add(dropdownInt);

#endregion

        }

        public override void Toggle() {
            window.ToggleVisibility();

            if (window.isVisible == true) {
                window.ScrollToTop();
            }
        }
    }
}
