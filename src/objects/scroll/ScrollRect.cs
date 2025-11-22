using UnityEngine.EventSystems;
using UEScrollRect = UnityEngine.UI.ScrollRect;

namespace UILib {
    internal class ScrollRect : UEScrollRect {
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
