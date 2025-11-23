using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UILib {
    /**
     * <summary>
     * A class which handles mouse input events.
     * </summary>
     */
    internal class MouseHandler: MonoBehaviour,
        IPointerClickHandler,
        IBeginDragHandler, IDragHandler
    {

#region Click Events

        private UnityEvent clickEvent = new UnityEvent();
        private UnityEvent doubleClickEvent = new UnityEvent();

        /**
         * <summary>
         * Adds a callback to run on single clicks.
         * </summary>
         * <param name="callback">The callback to add</param>
         */
        public void AddClickListener(UnityAction callback) {
            clickEvent.AddListener(callback);
        }

        /**
         * <summary>
         * Adds a callback to run on double clicks.
         * </summary>
         * <param name="callback">The callback to add</param>
         */
        public void AddDoubleClickListener(UnityAction callback) {
            doubleClickEvent.AddListener(callback);
        }

        /**
         * <summary>
         * Executes when clicked.
         * </summary>
         * <param name="eventData">The event data</param>
         */
        public void OnPointerClick(PointerEventData eventData) {
            // Depending on the click count,
            // dispatch to different callbacks
            switch (eventData.clickCount) {
                case 1:
                    clickEvent.Invoke();
                    break;
                case 2:
                    doubleClickEvent.Invoke();
                    break;
            }
        }

#endregion

#region Drag Events

        internal class DragEvent : UnityEvent<Vector2> {}
        private DragEvent beginDragEvent = new DragEvent();
        private DragEvent dragEvent      = new DragEvent();
        private DragEvent endDragEvent   = new DragEvent();

        /**
         * <summary>
         * Adds a callback to run when dragging starts.
         * </summary>
         * <param name="callback">The callback to add</param>
         */
        public void AddBeginDragListener(UnityAction<Vector2> callback) {
            beginDragEvent.AddListener(callback);
        }

        /**
         * <summary>
         * Adds a callback to run when being dragged.
         * </summary>
         * <param name="callback">The callback to add</param>
         */
        public void AddDragListener(UnityAction<Vector2> callback) {
            dragEvent.AddListener(callback);
        }

        /**
         * <summary>
         * Adds a callback to run when dragging ends.
         * </summary>
         * <param name="callback">The callback to add</param>
         */
        public void AddEndDragListener(UnityAction<Vector2> callback) {
            endDragEvent.AddListener(callback);
        }

        /**
         * <summary>
         * Executes when dragging begins.
         * </summary>
         * <param name="eventData">The event data</param>
         */
        public void OnBeginDrag(PointerEventData eventData) {
            beginDragEvent.Invoke(eventData.position);
        }

        /**
         * <summary>
         * Executes when dragging.
         * </summary>
         * <param name="eventData">The event data</param>
         */
        public void OnDrag(PointerEventData eventData) {
            dragEvent.Invoke(eventData.position);
        }

        /**
         * <summary>
         * Executes when dragging ends.
         * </summary>
         * <param name="eventData">The event data</param>
         */
        public void OnEndDrag(PointerEventData eventData) {
            endDragEvent.Invoke(eventData.position);
        }

#endregion

    }
}
