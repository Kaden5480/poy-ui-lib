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
         * This scroll view's horizontal <see cref="ScrollBar"/>.
         * </summary>
         */
        public ScrollBar scrollBarH  { get; private set; }

        /**
         * <summary>
         * This scroll view's vertical <see cref="ScrollBar"/>.
         * </summary>
         */
        public ScrollBar scrollBarV  { get; private set; }

        /**
         * <summary>
         * This scroll view's viewport.
         * </summary>
         */
        public Area viewport { get; private set; }

        /**
         * <summary>
         * This scroll view's content.
         * </summary>
         */
        public Area scrollContent { get; private set; }

        internal CustomScrollRect scrollRect { get; private set; }

        /**
         * <summary>
         * Initializes a ScrollView.
         * </summary>
         * <param name="scrollType">The types of scrolling to support</param>
         */
        public ScrollView(
            ScrollType scrollType = ScrollType.Horizontal | ScrollType.Vertical
        ) {
            scrollRect = gameObject.AddComponent<CustomScrollRect>();

            viewport = new Area();
            viewport.gameObject.AddComponent<UEMask>();
            viewport.gameObject.AddComponent<UEImage>();
            AddDirect(viewport);

            scrollContent = new Area();
            viewport.AddDirect(scrollContent);

            // Content setup
            scrollContent.SetFill(FillType.All);
            scrollContent.SetSize(0f,0f);

            // Viewport setup
            viewport.SetFill(FillType.All);
            viewport.SetSize(-20f, 0f);
            viewport.SetOffset(-10f, 0f);

            background = viewport.gameObject.GetComponent<UEImage>();

            // Scroll rect setup
            scrollRect.viewport = viewport.rectTransform;
            scrollRect.elasticity = 0f;
            scrollRect.scrollSensitivity = 150f;
            scrollRect.movementType = UEScrollRect.MovementType.Clamped;

            // Scroll bar setup
            if (scrollType.HasFlag(ScrollType.Horizontal) == true) {
                scrollRect.horizontal = true;
                scrollBarH = new ScrollBar(ScrollType.Horizontal);
                Add(gameObject, scrollBarH);
                scrollRect.horizontalScrollbar = scrollBarH.scrollBar;
                scrollRect.horizontalScrollbarVisibility = UEScrollRect.ScrollbarVisibility.AutoHide;
            }

            if (scrollType.HasFlag(ScrollType.Vertical) == true) {
                scrollRect.vertical = true;
                scrollBarV = new ScrollBar(ScrollType.Vertical);
                Add(gameObject, scrollBarV);
                scrollRect.verticalScrollbar = scrollBarV.scrollBar;
            }

            // Use the scrollContent instead
            SetContent(scrollContent);

            // Set the theme
            SetThisTheme(theme);
        }

        /**
         * <summary>
         * Allows setting the theme of this scroll view.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {
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
            scrollRect.horizontal = horizontal;
            scrollRect.vertical = vertical;
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

        /**
         * <summary>
         * Scroll to the top of this scroll view.
         * </summary>
         */
        public void ScrollToTop() {
            scrollBarV.SetScroll(1f);
        }

        /**
         * <summary>
         * Scroll to the bottom of this scroll view.
         * </summary>
         */
        public void ScrollToBottom() {
            scrollBarV.SetScroll(0f);
        }

    }
}
