using System.Collections;

using UnityEngine;

namespace UILib {
    /**
     * <summary>
     * Handles destroying a Notification
     * after a given amount of time.
     * </summary>
     */
    internal class FadeDestroy : MonoBehaviour {
        // How much time to wait before fading
        private const float timeBeforeFade = 3f;

        // How long the fade should last
        private const float fadeTime = 1f;

        private IEnumerator coroutine;

        /**
         * <summary>
         * Starts fading the notification.
         * </summary>
         * <param name="notification">The notification to fade</param>
         */
        internal void StartFade(Notification notification) {
            if (coroutine != null) {
                return;
            }

            coroutine = FadeToDestroy(notification);
            StartCoroutine(coroutine);
        }

        /**
         * <summary>
         * Fades the notification.
         * </summary>
         * <param name="notification">The notification to fade</param>
         */
        private IEnumerator FadeToDestroy(Notification notification) {
            float timer = 0f;
            while (timer < timeBeforeFade) {
                timer += Time.deltaTime;
                yield return null;
            }

            float opacity = 1f;
            while (opacity > 0) {
                opacity = Mathf.MoveTowards(opacity, 0f, Time.deltaTime / fadeTime);
                notification.SetOpacity(opacity);
                yield return null;
            }

            notification.Destroy();
            yield break;
        }
    }
}
