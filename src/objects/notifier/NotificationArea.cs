using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace UILib {
    internal class NotificationArea : UIObject {
        internal const float size = 400f;

        internal Canvas canvas;

        private Area area;

        /**
         * <summary>
         * Initializes a Window.
         * </summary>
         * <param name="name">The name of this window</param>
         * <param name="width">The width of this Window</param>
         * <param name="height">The height of this Window</param>
         */
        internal NotificationArea() {
            float margin = 20;

            canvas = new Canvas();
            canvas.Add(this);
            canvas.canvas.sortingOrder = 9999;

            SetAnchor(AnchorType.TopRight, FillType.FillVertical);

            area = new Area();
            area.SetAnchor(AnchorType.BottomMiddle);
            area.SetLayout(LayoutType.Vertical, margin);

            Add(gameObject, area);

            rectTransform.anchoredPosition = new Vector2(-(margin), 0f);
            SetSize(size, -(2f * margin));
        }

        /**
         * <summary>
         * Adds the provided component as a child to this one.
         * </summary>
         * <param name="child">The object which should be a child of this object</param>
         */
        public override void Add(UIObject child) {
            area.Add(child);
        }

        /**
         * <summary>
         * Sets the layout to be used on this NotificationArea.
         * </summary>
         * <param name="layoutType">The type of layout to use</param>
         * <param name="spacing">The spacing to use</param>
         */
        public override void SetLayout(LayoutType layoutType, float spacing=0) {
            area.SetLayout(layoutType, spacing);
        }
    }
}
