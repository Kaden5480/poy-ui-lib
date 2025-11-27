using UnityEngine.EventSystems;
using UEScrollRect = UnityEngine.UI.ScrollRect;

namespace UILib.Components {
    /**
     * <summary>
     * Overrides Unity's ScrollRect to ignore being dragged.
     * </summary>
     */
    internal class CustomScrollRect : UEScrollRect {
        public override void OnBeginDrag(PointerEventData eventData) {
            eventData.Use();
        }

        public override void OnDrag(PointerEventData eventData) {
            eventData.Use();
        }

        public override void OnEndDrag(PointerEventData eventData) {
            eventData.Use();
        }
    }
}
