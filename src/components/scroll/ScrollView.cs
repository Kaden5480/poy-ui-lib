using UnityEngine;

using ContentSizeFitter = UnityEngine.UI.ContentSizeFitter;
using VerticalLayoutGroup = UnityEngine.UI.VerticalLayoutGroup;

using UEImage = UnityEngine.UI.Image;
using UEMask = UnityEngine.UI.Mask;
using UEScrollRect = UnityEngine.UI.ScrollRect;

using UILib.Layouts;

namespace UILib.Components {
    /**
     * <summary>
     * A ScrollView.
     *
     * If content is larger than the ScrollView, ScrollBars
     * will be enabled to allow scrolling through it.
     *
     * Vertical ScrollBars are always on by default.
     * </summary>
     */
    public class ScrollView : UIComponent {
        /**
         * <summary>
         * This scroll view's background.
         * </summary>
         */
        public UEImage background { get; private set; }

        /**
         * <summary>
         * The width of this scroll view's scroll bars.
         * </summary>
         */
        public float scrollBarWidth { get; private set; } = 20f;

        private GameObject viewport;
        private Area scrollContent;

        internal CustomScrollRect scrollRect { get; private set; }
        internal ScrollBar scrollBarH  { get; private set; }
        internal ScrollBar scrollBarV  { get; private set; }

        /**
         * <summary>
         * Initializes a ScrollView.
         * </summary>
         * <param name="scrollBarWidth">The width of the scrollbars</param>
         */
        public ScrollView(float scrollBarWidth = 20) {
            this.scrollBarWidth = scrollBarWidth;
            scrollRect = gameObject.AddComponent<CustomScrollRect>();

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
            viewportRect.sizeDelta = new Vector2(-scrollBarWidth, 0f);
            viewportRect.anchoredPosition = new Vector2(-scrollBarWidth/2, 0f);

            background = viewport.GetComponent<UEImage>();

            // Scroll rect setup
            scrollRect.viewport = viewportRect;
            scrollRect.elasticity = 0f;
            scrollRect.scrollSensitivity = 150f;
            scrollRect.movementType = UEScrollRect.MovementType.Clamped;

            // Scroll bar setup
            scrollRect.horizontal = true;
            scrollBarH = new ScrollBar(ScrollType.Horizontal, scrollBarWidth);
            Add(gameObject, scrollBarH);
            scrollRect.horizontalScrollbar = scrollBarH.scrollBar;
            scrollRect.horizontalScrollbarVisibility = UEScrollRect.ScrollbarVisibility.AutoHide;

            scrollRect.vertical = true;
            scrollBarV = new ScrollBar(ScrollType.Vertical, scrollBarWidth);
            Add(gameObject, scrollBarV);
            scrollRect.verticalScrollbar = scrollBarV.scrollBar;

            // Use the scrollContent instead
            SetContent(scrollContent);

            // Set the theme
            SetTheme(theme);
        }

        /**
         * <summary>
         * Allows setting the theme of this scroll view
         * and all children.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        public override void SetTheme(Theme theme) {
            base.SetTheme(theme);

            background.color = theme.background;
        }

        /**
         * <summary>
         * Changes which types of scrolling are allowed.
         * </summary>
         * <param name="horizontal">Whether horizontal scrolling should be allowed</param>
         * <param name="vertical">Whether vertical scrolling should be allowed</param>
         */
        public void SetAllowedScroll(bool horizontal = true, bool vertical = true) {
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
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
