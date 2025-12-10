using System;

using UILib.Components;
using UILib.Layouts;

namespace UILib.ColorPicker {
    internal class ColorArea : Area {
        private ColorUpdater updater;
        private ColorUpdate updateType;
        private TextField[] fields;

        /**
         * <summary>
         * Initializes a color area.
         * </summary>
         * <param name="inputs">The inputs for this area</param>
         * <param name="updater">The updater to dispatch to</param>
         * <param name="updateType">The type of updates this area makes</param>
         */
        internal ColorArea(InputInfo[] inputs, ColorUpdater updater, ColorUpdate updateType) {
            this.updater = updater;
            this.updateType = updateType;

            SetSize(90f, 26f*inputs.Length + 10*(inputs.Length-1));
            SetContentLayout(LayoutType.Vertical);
            SetElementSpacing(10);

            fields = new TextField[inputs.Length];

            for (int i = 0; i < inputs.Length; i++) {
                fields[i] = CreateInput(inputs[i]);
            }
        }

        /**
         * <summary>
         * Updates this area using the provided values.
         * </summary>
         * <param name="values">The values to update the area with</param>
         */
        internal void Update(float[] values) {
            for (int i = 0; i < values.Length; i++) {
                fields[i].SetValue(
                    Math.Round(values[i], 2).ToString()
                );
            }
        }

        /**
         * <summary>
         * Creates one input control.
         * </summary>
         * <param name="input">The input to make</param>
         * <returns>The text field input</returns>
         */
        private TextField CreateInput(InputInfo input) {
            Area area = new Area();
            area.SetSize(90f, 26f);
            area.SetContentLayout(LayoutType.Horizontal);

            Label label = new Label(input.name, 20);
            label.SetSize(30f, 26f);
            area.Add(label);

            TextField textField = new TextField("", 20);
            textField.SetSize(60f, 26f);
            textField.SetSubmitMode(
                TextField.SubmitMode.Click
                | TextField.SubmitMode.Escape
            );

            textField.SetPredicate((string value) => {
                if (ColorUpdater.Validate(value, input.min, input.max, out float result) == true) {
                    input.value = result;
                    updater.Update(updateType);
                    return true;
                }
                return false;
            });
            textField.onInputChanged.AddListener((string value) => {
                if (ColorUpdater.Validate(value, input.min, input.max, out float result) == true) {
                    input.value = result;
                    updater.Update(updateType);
                }
            });

            area.Add(textField);

            Add(area);

            return textField;
        }
    }
}
