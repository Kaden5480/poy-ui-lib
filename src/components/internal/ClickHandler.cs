using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UILib {
    internal class ClickHandler : MonoBehaviour, IPointerClickHandler {
        private UnityEvent uEvent = new UnityEvent();

        public void AddListener(UnityAction callback) {
            uEvent.AddListener(callback);
        }

        public void OnPointerClick(PointerEventData eventData) {
            uEvent.Invoke();
        }
    }
}
