using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UILib.Events;

namespace UILib.Behaviours {
    /**
     * <summary>
     * A class which handles mouse input events.
     * </summary>
     */
    internal class MouseHandler: MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler,
        IPointerClickHandler,
        IBeginDragHandler, IDragHandler
    {

#region Enable/Disable Events

        internal UnityEvent onEnable { get; } = new UnityEvent();
        internal UnityEvent onDisable { get; } = new UnityEvent();

        /**
         * <summary>
         * Executes when this behaviour is enabled.
         * </summary>
         */
        private void OnEnable() {
            onEnable.Invoke();
        }

        /**
         * <summary>
         * Executes when this behaviour is disabled.
         * </summary>
         */
        private void OnDisable() {
            onDisable.Invoke();
        }

#endregion

#region Pointer Events

        internal UnityEvent onPointerEnter { get; } = new UnityEvent();
        internal UnityEvent onPointerExit { get; } = new UnityEvent();
        internal UnityEvent onPointerDown { get; } = new UnityEvent();
        internal UnityEvent onPointerUp { get; } = new UnityEvent();

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

        /**
         * <summary>
         * Executes when the pointer is held down on an object.
         * </summary>
         */
        public void OnPointerDown(PointerEventData eventData) {
            onPointerDown.Invoke();
        }

        /**
         * <summary>
         * Executes when the pointer is no longer held down on an object.
         * </summary>
         */
        public void OnPointerUp(PointerEventData eventData) {
            onPointerUp.Invoke();
        }

#endregion

#region Click Events

        internal UnityEvent onClick          { get; } = new UnityEvent();
        internal UnityEvent onDoubleClick    { get; } = new UnityEvent();
        internal MouseEvent onClickPos       { get; } = new MouseEvent();
        internal MouseEvent onDoubleClickPos { get; } = new MouseEvent();

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
                    onClickPos.Invoke(eventData.position);
                    onClick.Invoke();
                    break;
                case 2:
                    onDoubleClickPos.Invoke(eventData.position);
                    onDoubleClick.Invoke();
                    break;
            }
        }

#endregion

#region Drag Events

        internal MouseEvent onBeginDrag { get; } = new MouseEvent();
        internal MouseEvent onDrag      { get; } = new MouseEvent();
        internal MouseEvent onEndDrag   { get; } = new MouseEvent();

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
