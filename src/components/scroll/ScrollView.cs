using UnityEngine;

using ContentSizeFitter = UnityEngine.UI.ContentSizeFitter;
using VerticalLayoutGroup = UnityEngine.UI.VerticalLayoutGroup;

using UEImage = UnityEngine.UI.Image;
using UEMask = UnityEngine.UI.Mask;
using UEScrollRect = UnityEngine.UI.ScrollRect;

using UILib.Layout;

namespace UILib.Components {
    /**
     * <summary>
     * A ScrollView.
     *
     * If content is larger than the ScrollView, ScrollBars
     * will be enabled to allow scrolling through it.
     *
     * NOTE:
     * Vertical ScrollBars are always on.
     * </summary>
     */
    public class ScrollView : UIComponent {
        private GameObject viewport;
        private Area scrollContent;

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

            scrollContent = new Area();
            Add(viewport, scrollContent);

            // Content setup
            RectTransform contentRect = scrollContent.rectTransform;
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

            // Use the scrollContent instead
            SetContent(scrollContent);
        }

        /**
         * <summary>
         * Sets a different component to be the content.
         * </summary>
         * <param name="content">The component which should be the content instead</param>
         */
        public override void SetContent(UIComponent content) {
            base.SetContent(content);
            scrollRect.content = content.rectTransform;
        }
    }
}
