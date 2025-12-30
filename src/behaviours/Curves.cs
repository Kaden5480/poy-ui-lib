using UnityEngine;

namespace UILib.Behaviours {
    /**
     * <summary>
     * A collection of curves for easing.
     *
     * All of these curves expect input values from 0 to 1
     * and will return a value between 0 and 1.
     *
     * See: https://easings.net/
     * </summary>
     */
    public static class Curves {
        /**
         * <summary>
         * Applies a linear function.
         * </summary>
         * <param name="value">The value to apply the function to</param>
         * <returns>The result of applying this function</param>
         */
        public static float Linear(float value) {
            return Mathf.Clamp(value, 0f, 1f);
        }

        /**
         * <summary>
         * Applies an exponential easing curve which speeds up over time.
         * </summary>
         * <param name="value">The value to apply the function to</param>
         * <returns>The result of applying this function</param>
         */
        public static float EaseInExp(float value) {
            if (value <= 0f) {
                return 0f;
            }

            float result = Mathf.Pow(2f, 10*value - 10);
            return Mathf.Clamp(result, 0f, 1f);
        }

        /**
         * <summary>
         * Applies a exponential easing curve which slows down over time.
         * </summary>
         * <param name="value">The value to apply the function to</param>
         * <returns>The result of applying this function</param>
         */
        public static float EaseOutExp(float value) {
            if (value >= 1f) {
                return 1f;
            }

            float result = 1 - Mathf.Pow(2f, -10f * value);
            return Mathf.Clamp(result, 0f, 1f);
        }

        /**
         * <summary>
         * Applies a exponential easing curve which starts and ends slowly.
         * </summary>
         * <param name="value">The value to apply the function to</param>
         * <returns>The result of applying this function</param>
         */
        public static float EaseInOutExp(float value) {
            if (value <= 0f) {
                return 0f;
            }

            if (value >= 1f) {
                return 1f;
            }

            float result;

            if (value < 0.5) {
                result = Mathf.Pow(2f, 20f * value - 10f) / 2f;
            }
            else {
                result = Mathf.Pow(2f, -20f * value + 10f) / 2f;
            }

            return Mathf.Clamp(result, 0f, 1f);
        }
    }
}
