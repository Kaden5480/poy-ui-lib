namespace UILib.ColorPicker {
    /**
     * <summary>
     * A class holding information for a single input.
     * </summary>
     */
    internal class InputInfo {
        internal string name;
        internal int min;
        internal int max;
        internal RefValue<float> value;

        /**
         * <summary>
         * Initializes the input info.
         * </summary>
         * <param name="name">The name to display</param>
         * <param name="min">The minimum value</param>
         * <param name="max">The maximum value</param>
         * <param name="value">The value this input should update</param>
         */
        internal InputInfo(string name, int min, int max, RefValue<float> value) {
            this.name = name;
            this.min = min;
            this.max = max;
            this.value = value;
        }
    }
}
