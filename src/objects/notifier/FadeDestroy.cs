using System.Collections;

using UnityEngine;

namespace UILib {
    public class FadeDestroy : MonoBehaviour {
        private const float timeBeforeFade = 3f;
        private const float fadeTime = 1f;
        private IEnumerator coroutine;

        internal void StartFade(Notification notification) {
            if (coroutine != null) {
                return;
            }

            coroutine = FadeToDestroy(notification);
            StartCoroutine(coroutine);
        }

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
