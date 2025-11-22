using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UILib {
    internal class MouseHandler: MonoBehaviour,
        IPointerClickHandler,
        IBeginDragHandler, IDragHandler
    {

#region Click Events

        private UnityEvent clickEvent = new UnityEvent();

        public void AddClickListener(UnityAction callback) {
            clickEvent.AddListener(callback);
        }

        public void OnPointerClick(PointerEventData eventData) {
            clickEvent.Invoke();
        }

#endregion

#region Drag Events

        internal class DragEvent : UnityEvent<Vector2> {}
        private DragEvent beginDragEvent = new DragEvent();
        private DragEvent dragEvent      = new DragEvent();
        private DragEvent endDragEvent   = new DragEvent();

        public void AddBeginDragListener(UnityAction<Vector2> callback) {
            beginDragEvent.AddListener(callback);
        }

        public void AddDragListener(UnityAction<Vector2> callback) {
            dragEvent.AddListener(callback);
        }

        public void AddEndDragListener(UnityAction<Vector2> callback) {
            endDragEvent.AddListener(callback);
        }

        public void OnBeginDrag(PointerEventData eventData) {
            beginDragEvent.Invoke(eventData.position);
        }

        public void OnDrag(PointerEventData eventData) {
            dragEvent.Invoke(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData) {
            endDragEvent.Invoke(eventData.position);
        }

#endregion

    }
}
