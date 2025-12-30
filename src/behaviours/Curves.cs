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
         * <returns>The result of applying this function</returns>
         */
        public static float Linear(float value) {
            return Mathf.Clamp(value, 0f, 1f);
        }

#region Trigonometric

        /**
         * <summary>
         * Applies a sine easing curve which speeds up over time.
         * </summary>
         * <param name="value">The value to apply the function to</param>
         * <returns>The result of applying this function</returns>
         */
        public static float EaseInSine(float value) {
            float result = 1f - Mathf.Cos((value * Mathf.PI) / 2f);
            return Mathf.Clamp(result, 0f, 1f);
        }

        /**
         * <summary>
         * Applies a sine easing curve which slows down over time.
         * </summary>
         * <param name="value">The value to apply the function to</param>
         * <returns>The result of applying this function</returns>
         */
        public static float EaseOutSine(float value) {
            float result = Mathf.Sin((value * Mathf.PI) / 2);
            return Mathf.Clamp(result, 0f, 1f);
        }

        /**
         * <summary>
         * Applies a sine easing curve which speeds up and then
         * slows down again.
         * </summary>
         * <param name="value">The value to apply the function to</param>
         * <returns>The result of applying this function</returns>
         */
        public static float EaseInOutSine(float value) {
            float result = -((Mathf.Cos(value * Mathf.PI) - 1f) / 2f);
            return Mathf.Clamp(result, 0f, 1f);
        }

#endregion

#region Exponential

        /**
         * <summary>
         * Applies an exponential easing curve which speeds up over time.
         * </summary>
         * <param name="value">The value to apply the function to</param>
         * <returns>The result of applying this function</returns>
         */
        public static float EaseInExp(float value) {
            if (value <= 0f) { return 0f; }

            float result = Mathf.Pow(2f, 10f * value - 10);
            return Mathf.Clamp(result, 0f, 1f);
        }

        /**
         * <summary>
         * Applies a exponential easing curve which slows down over time.
         * </summary>
         * <param name="value">The value to apply the function to</param>
         * <returns>The result of applying this function</returns>
         */
        public static float EaseOutExp(float value) {
            if (value >= 1f) { return 1f; }

            float result = 1 - Mathf.Pow(2f, -10f * value);
            return Mathf.Clamp(result, 0f, 1f);
        }

        /**
         * <summary>
         * Applies a exponential easing curve which starts and ends slowly.
         * </summary>
         * <param name="value">The value to apply the function to</param>
         * <returns>The result of applying this function</returns>
         */
        public static float EaseInOutExp(float value) {
            if (value <= 0f) { return 0f; }
            if (value >= 1f) { return 1f; }

            float result;

            if (value < 0.5) { result = Mathf.Pow(2f, 20f * value - 10f) / 2f; }
            else             { result = (2 - Mathf.Pow(2f, -20f * value + 10f)) / 2f; }

            return Mathf.Clamp(result, 0f, 1f);
        }

#endregion

#region Polynomial

        /**
         * <summary>
         * Applies a quadratic easing curve which starts slowly
         * and speeds up over time.
         * </summary>
         * <param name="value">The value to apply the function to</param>
         * <returns>The result of applying this function</returns>
         */
        public static float EaseInQuad(float value) {
            return Mathf.Clamp(value * value, 0f, 1f);
        }

        /**
         * <summary>
         * Applies a quadratic easing curve which starts quickly
         * and slows down over time.
         * </summary>
         * <param name="value">The value to apply the function to</param>
         * <returns>The result of applying this function</returns>
         */
        public static float EaseOutQuad(float value) {
            return Mathf.Clamp(
                1f - (1f - value) * (1f - value),
                0f, 1f
            );
        }

        /**
         * <summary>
         * Applies a quadratic/exponential easing curve which starts slowly,
         * speeds up, and then slows down again.
         * </summary>
         * <param name="value">The value to apply the function to</param>
         * <returns>The result of applying this function</returns>
         */
        public static float EaseInOutQuad(float value) {
            float result;

            if (value < 0.5f) { result = 2f * value * value; }
            else              { result = 1 - Mathf.Pow(-2f * value + 2f, 2f) / 2f; }

            return Mathf.Clamp(result, 0f, 1f);
        }

        /**
         * <summary>
         * Applies a cubic easing curve which starts slowly
         * and speeds up over time.
         * </summary>
         * <param name="value">The value to apply the function to</param>
         * <returns>The result of applying this function</returns>
         */
        public static float EaseInCubic(float value) {
            return Mathf.Clamp(value * value * value, 0f, 1f);
        }

        /**
         * <summary>
         * Applies a cubic easing curve which starts quickly
         * and slows down over time.
         * </summary>
         * <param name="value">The value to apply the function to</param>
         * <returns>The result of applying this function</returns>
         */
        public static float EaseOutCubic(float value) {
            return Mathf.Clamp(
                1f - Mathf.Pow(1f - value, 3f),
                0f, 1f
            );
        }

        /**
         * <summary>
         * Applies a cubic easing curve which starts quickly
         * and slows down over time.
         * </summary>
         * <param name="value">The value to apply the function to</param>
         * <returns>The result of applying this function</returns>
         */
        public static float EaseInOutCubic(float value) {
            float result;

            if (value < 0.5f) { result = 4 * value * value * value; }
            else              { result = 1 - Mathf.Pow(-2f * value + 2, 3f) / 2f; }

            return Mathf.Clamp(result, 0f, 1f);
        }


#endregion

        /**
         * <summary>
         * Applies a elastic easing curve which slows down over time and
         * bounces at the end.
         * </summary>
         * <param name="value">The value to apply the function to</param>
         * <returns>The result of applying this function</returns>
         */
        public static float EaseOutElastic(float value) {
            if (value <= 0f) { return 0f; }
            if (value >= 1f) { return 1f; }

            float c = (2f * Mathf.PI) / 3f;
            float result = Mathf.Pow(2f, -10f * value)
                * Mathf.Sin((10f * value - 0.75f) * c) + 1f;

            return Mathf.Clamp(result, 0f, 1f);
        }
    }
}
