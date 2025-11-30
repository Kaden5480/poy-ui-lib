using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UILib.Behaviours {
    /**
     * <summary>
     * A class which handles mouse input events.
     * </summary>
     */
    internal class MouseHandler: MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerClickHandler,
        IBeginDragHandler, IDragHandler
    {

#region Hover Events

        internal UnityEvent onPointerEnter { get; } = new UnityEvent();
        internal UnityEvent onPointerExit { get; } = new UnityEvent();

        /**
         * <summary>
         * Executes when the pointer enters this object.
         * </summary>
         */
        public void OnPointerEnter(PointerEventData eventData) {
            onPointerEnter.Invoke();
        }

        /**
         * <summary>
         * Executes when the pointer exits this object.
         * </summary>
         */
        public void OnPointerExit(PointerEventData eventData) {
            onPointerExit.Invoke();
        }

#endregion

#region Click Events

        internal UnityEvent onClick       { get; } = new UnityEvent();
        internal UnityEvent onDoubleClick { get; } = new UnityEvent();

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
                    onClick.Invoke();
                    break;
                case 2:
                    onDoubleClick.Invoke();
                    break;
            }
        }

#endregion

#region Drag Events

        internal DragEvent onBeginDrag { get; } = new DragEvent();
        internal DragEvent onDrag      { get; } = new DragEvent();
        internal DragEvent onEndDrag   { get; } = new DragEvent();

        /**
         * <summary>
         * Executes when dragging begins.
         * </summary>
         * <param name="eventData">The event data</param>
         */
        public void OnBeginDrag(PointerEventData eventData) {
            onBeginDrag.Invoke(eventData.position);
        }

        /**
         * <summary>
         * Executes when dragging.
         * </summary>
         * <param name="eventData">The event data</param>
         */
        public void OnDrag(PointerEventData eventData) {
            onDrag.Invoke(eventData.position);
        }

        /**
         * <summary>
         * Executes when dragging ends.
         * </summary>
         * <param name="eventData">The event data</param>
         */
        public void OnEndDrag(PointerEventData eventData) {
            onEndDrag.Invoke(eventData.position);
        }

#endregion

    }
}
