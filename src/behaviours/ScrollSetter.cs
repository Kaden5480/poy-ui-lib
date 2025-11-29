using System.Collections;

using UnityEngine;
using UEScrollbar = UnityEngine.UI.Scrollbar;

namespace UILib.Behaviours {
    /**
     * <summary>
     * A behaviour which handles setting scrollbars
     * to specific values.
     * </summary>
     */
    internal class ScrollSetter : MonoBehaviour {
        private IEnumerator coroutine;

        /**
         * <summary>
         * Coroutine for setting a scrollbar to a given value.
         * </summary>
         * <param name="bar">The scrollbar to set</param>
         * <param name="value">The value to set</param>
         */
        private IEnumerator SetScrollCoroutine(UEScrollbar bar, float value) {
            yield return new WaitForSeconds(0.02f);

            bar.value = value;
            coroutine = null;

            yield break;
        }

        /**
         * <summary>
         * Sets a scrollbar to a specific value.
         * </summary>
         * <param name="bar">The scrollbar to set</param>
         * <param name="value">The value to set</param>
         */
        internal void SetScroll(UEScrollbar bar, float value) {
            if (coroutine != null) {
                StopCoroutine(coroutine);
            }

            coroutine = SetScrollCoroutine(bar, value);
            StartCoroutine(coroutine);
        }
    }
}
