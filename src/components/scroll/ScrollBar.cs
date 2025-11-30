using UnityEngine;

using UEColorBlock = UnityEngine.UI.ColorBlock;
using UEImage = UnityEngine.UI.Image;
using UEScrollbar = UnityEngine.UI.Scrollbar;

using UILib.Behaviours;
using UILib.Layouts;

namespace UILib.Components {
    /**
     * <summary>
     * A scroll bar.
     * </summary>
     */
    public class ScrollBar : UIObject {
        private ScrollSetter scrollSetter;

        /**
         * <summary>
         * The width of this scroll bar.
         * </summary>
         */
        public float barWidth { get; private set; }

        private GameObject slidingArea;
        private GameObject handle;

        private UEImage background;
        internal UEScrollbar scrollBar { get; private set; }

        /**
         * <summary>
         * Initializes a ScrollBar.
         * </summary>
         * <param name="scrollType">The type of scrollbar to use</param>
         * <param name="width">The width of the scrollbar</param>
         */
        internal ScrollBar(ScrollType scrollType, float width) {
            scrollSetter = gameObject.AddComponent<ScrollSetter>();

            // Making the object hierarchy
            slidingArea = new GameObject("Sliding Area",
                typeof(RectTransform)
            );
            SetParent(gameObject, slidingArea);

            handle = new GameObject("Handle",
                typeof(RectTransform), typeof(UEImage)
            );
            SetParent(slidingArea, handle);

            // Scrollbar setup
            background = gameObject.AddComponent<UEImage>();
            scrollBar = gameObject.AddComponent<UEScrollbar>();
            scrollBar.handleRect = handle.GetComponent<RectTransform>();
            scrollBar.targetGraphic = handle.GetComponent<UEImage>();

            switch (scrollType) {
                case ScrollType.Vertical:
                    scrollBar.direction = UEScrollbar.Direction.BottomToTop;
                    SetAnchor(AnchorType.TopRight);
                    SetFill(FillType.Vertical);
                    SetWidth(width);
                    break;
                case ScrollType.Horizontal:
                    scrollBar.direction = UEScrollbar.Direction.LeftToRight;
                    SetAnchor(AnchorType.BottomLeft);
                    SetFill(FillType.Horizontal);
                    SetWidth(width);
                    break;
                default:
                    logger.LogDebug($"Unexpected ScrollType: {scrollType}");
                    break;
            }

            InitLayout();
            SetTheme(theme);
        }

        /**
         * <summary>
         * Sets the width of this scrollbar.
         * </summary>
         * <param name="width">The width to set</param>
         */
        public void SetWidth(float width) {
            this.barWidth = width;

            switch (scrollBar.direction) {
                case UEScrollbar.Direction.TopToBottom:
                case UEScrollbar.Direction.BottomToTop:
                    rectTransform.sizeDelta = new Vector2(barWidth, 0f);
                    rectTransform.anchoredPosition = Vector2.zero;
                    break;
                case UEScrollbar.Direction.RightToLeft:
                case UEScrollbar.Direction.LeftToRight:
                    rectTransform.sizeDelta = new Vector2(-barWidth, 20f);
                    rectTransform.anchoredPosition = new Vector2(-barWidth/2, 0f);
                    break;
            }
        }

        /**
         * <summary>
         * Allows setting the theme of this scroll bar
         * and all children.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        public override void SetTheme(Theme theme) {
            base.SetTheme(theme);

            background.color = theme.accent;
            scrollBar.colors = theme.blockSelectAlt;
        }

        /**
         * <summary>
         * Initializes the ScrollBar's layout.
         * </summary>
         */
        internal void InitLayout() {
            // Sliding area
            RectTransform slideRect = slidingArea.GetComponent<RectTransform>();
            slideRect.anchorMin = Vector2.zero;
            slideRect.anchorMax = Vector2.one;
            slideRect.sizeDelta = Vector2.zero;

            // Handle
            scrollBar.handleRect.anchorMin = Vector2.zero;
            scrollBar.handleRect.anchorMax = Vector2.one;
            scrollBar.handleRect.sizeDelta = Vector2.zero;
        }

        /**
         * <summary>
         * Sets the value of the scrollbar.
         * Should be between 0-1 (inclusive).
         * </summary>
         * <param name="value">The value to set the scrollbar to</param>
         */
        internal void SetScroll(float value) {
            scrollSetter.SetScroll(scrollBar, value);
        }

        /**
         * <summary>
         * This override ignores drags.
         * </summary>
         * <param name="position">The position the drag started at</param>
         */
        protected override void OnBeginDrag(Vector2 position) {}

        /**
         * <summary>
         * This override ignores drags.
         * </summary>
         * <param name="position">The position dragged to</param>
         */
        protected override void OnDrag(Vector2 position) {}

        /**
         * <summary>
         * This override ignores drags.
         * </summary>
         * <param name="position">The position dragged to</param>
         */
        protected override void OnEndDrag(Vector2 position) {}
    }
}
