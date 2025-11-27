using UnityEngine.Events;
using UnityEngine.EventSystems;

using UEInputField = UnityEngine.UI.InputField;

namespace UILib.Components {
    /**
     * <summary>
     * A custom InputField which implements events
     * that can invoke listeners on select and deselect.
     * </summary>
     */
    internal class CustomInputField : UEInputField {
        internal UnityEvent onSelect { get; } = new UnityEvent();
        internal UnityEvent onDeselect { get; } = new UnityEvent();

        public override void OnSelect(BaseEventData data) {
            base.OnSelect(data);
            onSelect.Invoke();
        }

        public override void OnDeselect(BaseEventData data) {
            base.OnDeselect(data);
            onDeselect.Invoke();
        }
    }
}
