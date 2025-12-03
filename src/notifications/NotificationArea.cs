using UnityEngine;
using Image = UnityEngine.UI.Image;

using UILib.Components;
using UILib.Layouts;

namespace UILib.Notifications {
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
         * Initializes an area to display notifications.
         * </summary>
         */
        internal NotificationArea() {
            float margin = 20;

            canvas = new Canvas();
            canvas.Add(this);

            SetAnchor(AnchorType.TopRight);
            SetFill(FillType.Vertical);

            area = new Area();
            area.SetAnchor(AnchorType.BottomMiddle);
            area.SetContentLayout(LayoutType.Vertical);
            area.SetElementSpacing(margin);

            Add(gameObject, area);

            SetOffset(-margin, 0f);
            SetSize(size, -(2f * margin));

            // Use the area as the content instead
            SetContent(area);
        }

        /**
         * <summary>
         * Allows setting the theme of this notification area.
         * Does nothing.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {}
    }
}
