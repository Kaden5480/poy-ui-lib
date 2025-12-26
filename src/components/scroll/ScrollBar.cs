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

        private GameObject slidingArea;
        private GameObject handle;

        private UEImage background;
        internal UEScrollbar scrollBar { get; private set; }

        /**
         * <summary>
         * Initializes a ScrollBar.
         * </summary>
         * <param name="scrollType">The type of scrollbar to use</param>
         */
        internal ScrollBar(ScrollType scrollType) {
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
                    SetSize(20f, 0f);
                    break;
                case ScrollType.Horizontal:
                    scrollBar.direction = UEScrollbar.Direction.LeftToRight;
                    SetAnchor(AnchorType.BottomLeft);
                    SetFill(FillType.Horizontal);
                    SetSize(-20f, 20f);
                    SetOffset(-10f, 0f);
                    break;
                default:
                    logger.LogDebug($"Unexpected ScrollType: {scrollType}");
                    break;
            }

            InitLayout();
            SetThisTheme(theme);
        }

        /**
         * <summary>
         * Allows setting the theme of this scroll bar.
         *
         * This handles setting the theme specifically for this component,
         * not its children. It's protected to allow overriding if you
         * were to create a subclass.
         *
         * In most cases, you'd probably want to use
         * <see cref="UIObject.SetTheme"/> instead.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {
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
