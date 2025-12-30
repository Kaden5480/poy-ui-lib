using UnityEngine;
using Image = UnityEngine.UI.Image;

using UILib.Components;
using UILib.Layouts;
using UILib.Patches;

namespace UILib {
    /**
     * <summary>
     * The overlay which holds all notifications.
     * </summary>
     */
    internal class NotificationOverlay : Overlay {
        private Area area;

        /**
         * <summary>
         * Initializes an overlay to display notifications.
         * </summary>
         */
        internal NotificationOverlay() : base(400f, 0f) {
            float margin = 20;

            SetSortingMode(Overlay.SortingMode.Static);
            SetLockMode(LockMode.None);

            SetAnchor(AnchorType.TopRight);
            SetFill(FillType.Vertical);

            area = new Area();
            area.SetAnchor(AnchorType.BottomMiddle);
            area.SetContentLayout(LayoutType.Vertical);
            area.SetElementSpacing(margin);

            Add(area);

            SetOffset(-margin, 0f);
            SetSize(width, -(2f * margin));

            // Use the area as the content instead
            SetContent(area);

            // Show by default
            Show(true);
        }
    }
}
