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

            // A simple toggle, like a button but
            // stores an on/off state
            Toggle toggle = new Toggle();
            toggle.SetSize(40f, 40f);
            toggle.onValueChanged.AddListener((bool value) => {
                Notifier.Notify("Toggle", $"Value was changed to: {value}");
            });
            window.Add(toggle);

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
                sliderVLabel.SetText($"Slider: {value}");
            });
            window.Add(verticalSlider);

#endregion

#region Input Fields

            // A simple field for receiving some text
            // from the user
            TextField textField = new TextField("Enter text", 20);
            textField.SetSize(200f, 40f);
            textField.onEndEdit.AddListener((string value) => {
                Notifier.Notify("TextField", $"You entered: {value}");
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
        }
    }
}
