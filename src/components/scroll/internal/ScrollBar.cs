using UnityEngine;

using UEColorBlock = UnityEngine.UI.ColorBlock;
using UEImage = UnityEngine.UI.Image;
using UEScrollbar = UnityEngine.UI.Scrollbar;

namespace UILib {
    internal class ScrollBar : BaseComponent {
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
            // Making the object hierarchy
            slidingArea = new GameObject("Sliding Area",
                typeof(RectTransform)
            );
            MakeChild(root, slidingArea);

            handle = new GameObject("Handle",
                typeof(RectTransform), typeof(UEImage)
            );
            MakeChild(slidingArea, handle);

            // Scrollbar setup
            background = root.AddComponent<UEImage>();
            background.color = Colors.grey;

            scrollBar = root.AddComponent<UEScrollbar>();
            scrollBar.handleRect = handle.GetComponent<RectTransform>();
            scrollBar.targetGraphic = handle.GetComponent<UEImage>();

            scrollBar.colors = new UEColorBlock() {
                fadeDuration     = 0.1f,
                colorMultiplier  = 1f,
                normalColor      = Colors.lightGrey,
                selectedColor    = Colors.lightGrey,
                disabledColor    = Colors.lightGrey,
                highlightedColor = Colors.lighterGrey,
                pressedColor     = Colors.lighterGrey,
            };

            switch (scrollType) {
                case ScrollType.Vertical:
                    scrollBar.direction = UEScrollbar.Direction.BottomToTop;
                    break;
                case ScrollType.Horizontal:
                    scrollBar.direction = UEScrollbar.Direction.LeftToRight;
                    break;
                default:
                    logger.LogDebug($"Unexpected ScrollType: {scrollType}");
                    break;
            }

            InitLayout();
        }

        /**
         * <summary>
         * Initializes the ScrollBar's layout.
         * </summary>
         */
        internal void InitLayout() {
            // Scrollbar
            rectTransform.anchorMin = new Vector2(1f, 0f);
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot     = Vector2.one;
            rectTransform.sizeDelta = new Vector2(20f, 0f);

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
         * Resets the scroll.
         * </summary>
         */
        internal void ResetScroll() {
            scrollBar.value = 1f;
        }
    }
}
