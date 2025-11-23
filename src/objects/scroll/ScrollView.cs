using UnityEngine;

using ContentSizeFitter = UnityEngine.UI.ContentSizeFitter;
using VerticalLayoutGroup = UnityEngine.UI.VerticalLayoutGroup;

using UEImage = UnityEngine.UI.Image;
using UEMask = UnityEngine.UI.Mask;
using UEScrollRect = UnityEngine.UI.ScrollRect;

namespace UILib {
    public class ScrollView : UIObject {
        private GameObject viewport;
        private GameObject content;

        internal ScrollRect scrollRect { get; private set; }
        internal ScrollBar scrollBarH  { get; private set; }
        internal ScrollBar scrollBarV  { get; private set; }

        /**
         * <summary>
         * Initializes a ScrollView.
         * </summary>
         */
        public ScrollView() {
            scrollRect = gameObject.AddComponent<ScrollRect>();

            viewport = new GameObject("Viewport",
                typeof(RectTransform), typeof(UEMask), typeof(UEImage)
            );
            SetParent(gameObject, viewport);

            content = new GameObject("Content",
                typeof(RectTransform)
            );
            SetParent(viewport, content);

            // Content setup
            RectTransform contentRect = content.GetComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.sizeDelta = Vector2.zero;

            // Viewport setup
            RectTransform viewportRect = viewport.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.sizeDelta = Vector2.zero;
            viewportRect.anchoredPosition = Vector2.zero;

            viewport.GetComponent<UEImage>().color = Colors.black;

            // Scroll rect setup
            scrollRect.content = contentRect;
            scrollRect.viewport = viewportRect;

            scrollRect.elasticity = 0f;
            scrollRect.scrollSensitivity = 150f;
            scrollRect.movementType = UEScrollRect.MovementType.Clamped;

            // Scroll bar setup
            scrollRect.vertical = true;
            scrollBarV = new ScrollBar(ScrollType.Vertical);
            Add(gameObject, scrollBarV);
            scrollRect.verticalScrollbar = scrollBarV.scrollBar;

            scrollRect.horizontal = true;
            scrollBarH = new ScrollBar(ScrollType.Horizontal);
            Add(gameObject, scrollBarH);
            scrollRect.horizontalScrollbar = scrollBarH.scrollBar;
            scrollRect.horizontalScrollbarVisibility = UEScrollRect.ScrollbarVisibility.AutoHide;
        }

        /**
         * <summary>
         * Adds the provided component as a child to this one.
         * </summary>
         * <param name="child">The object which should be a child of this object</param>
         */
        public override void Add(UIObject child) {
            Add(content, child);
        }

        /**
         * <summary>
         * Sets the layout to be used on this ScrollView.
         * </summary>
         * <param name="layoutType">The type of layout to use</param>
         */
        public override void SetLayout(LayoutType layoutType) {
            SetLayout(content, layoutType);
        }
    }
}
