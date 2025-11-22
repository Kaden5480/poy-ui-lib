using UnityEngine;

using UEColorBlock = UnityEngine.UI.ColorBlock;
using UEImage = UnityEngine.UI.Image;
using UEScrollbar = UnityEngine.UI.Scrollbar;

namespace UILib {
    internal class ScrollBar : UIObject {
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
            SetParent(gameObject, slidingArea);

            handle = new GameObject("Handle",
                typeof(RectTransform), typeof(UEImage)
            );
            SetParent(slidingArea, handle);

            // Scrollbar setup
            background = gameObject.AddComponent<UEImage>();
            background.color = Colors.grey;

            scrollBar = gameObject.AddComponent<UEScrollbar>();
            scrollBar.handleRect = handle.GetComponent<RectTransform>();
            scrollBar.targetGraphic = handle.GetComponent<UEImage>();
            scrollBar.colors = Colors.colorBlock;

            switch (scrollType) {
                case ScrollType.Vertical:
                    scrollBar.direction = UEScrollbar.Direction.BottomToTop;
                    SetAnchor(AnchorType.TopRight, FillType.FillVertical);
                    rectTransform.sizeDelta        = new Vector2(20f, 0f);
                    rectTransform.anchoredPosition = Vector2.zero;
                    break;
                case ScrollType.Horizontal:
                    scrollBar.direction = UEScrollbar.Direction.LeftToRight;
                    SetAnchor(AnchorType.BottomLeft, FillType.FillHorizontal);
                    rectTransform.sizeDelta        = new Vector2(-20f, 20f);
                    rectTransform.anchoredPosition = new Vector2(-10f, 0f);
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
