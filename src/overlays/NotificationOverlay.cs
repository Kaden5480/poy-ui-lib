using UnityEngine;
using Image = UnityEngine.UI.Image;

using UILib.Components;
using UILib.Layouts;

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

        /**
         * <summary>
         * Allows setting the theme of the notification overlay.
         * Does nothing.
         *
         * This handles setting the theme specifically for this object,
         * not its children. It's protected to allow overriding if you
         * were to create a subclass.
         *
         * In most cases, you'd probably want to use
         * <see cref="UIObject.SetTheme"/> instead.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {}
    }
}
