using UnityEngine;
using Image = UnityEngine.UI.Image;

using UILib.Components;
using UILib.Layout;

namespace UILib {
    /**
     * <summary>
     * The area which holds all notifications.
     * </summary>
     */
    internal class NotificationArea : UIObject {
        internal const float size = 400f;
        internal Canvas canvas;

        private Area area;

        /**
         * <summary>
         * Initializes a NotificationArea.
         * </summary>
         */
        internal NotificationArea() {
            float margin = 20;

            canvas = new Canvas();
            canvas.Add(this);
            canvas.canvas.sortingOrder = 9999;

            SetAnchor(AnchorType.TopRight);
            SetFill(FillType.Vertical);

            area = new Area();
            area.SetAnchor(AnchorType.BottomMiddle);
            area.SetLayout(LayoutType.Vertical);
            area.SetElementSpacing(margin);

            Add(gameObject, area);

            rectTransform.anchoredPosition = new Vector2(-(margin), 0f);
            SetSize(size, -(2f * margin));

            // Use the area as the content instead
            SetContent(area);
        }
    }
}
