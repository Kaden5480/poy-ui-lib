using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UILib {
    internal class DragHandler : MonoBehaviour,
        IBeginDragHandler, IDragHandler
    {
        private UnityAction<Vector2> beginDragCallback;
        private UnityAction<Vector2> dragCallback;

        public void SetBeginListener(UnityAction<Vector2> callback) {
            beginDragCallback = callback;
        }

        public void SetDragListener(UnityAction<Vector2> callback) {
            dragCallback = callback;
        }

        public void OnBeginDrag(PointerEventData eventData) {
            beginDragCallback.Invoke(eventData.position);
        }

        public void OnDrag(PointerEventData eventData) {
            dragCallback.Invoke(eventData.position);
        }
    }
}
