using UnityEngine;

using ContentSizeFitter = UnityEngine.UI.ContentSizeFitter;
using VerticalLayoutGroup = UnityEngine.UI.VerticalLayoutGroup;

using UEImage = UnityEngine.UI.Image;
using UEMask = UnityEngine.UI.Mask;

namespace UILib {
    public class ScrollView : UIObject {
        private GameObject viewport;
        private GameObject content;

        private ScrollRect scrollRect;
        private ScrollBar scrollBarH;
        private ScrollBar scrollBarV;

        /**
         * <summary>
         * Initializes a ScrollView.
         * </summary>
         * <param name="scrollType">The type(s) of scrolling this scroll view supports</param>
         */
        public ScrollView(ScrollType scrollType) {
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

            VerticalLayoutGroup group = content.AddComponent<VerticalLayoutGroup>();
            group.childControlWidth = true;
            group.childControlHeight = true;
            group.childForceExpandWidth = true;
            group.childForceExpandHeight = false;
            group.padding = new RectOffset(20, 20, 20, 20);

            ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Viewport setup
            RectTransform viewportRect = viewport.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.sizeDelta = new Vector2(-20f, 0f);
            viewportRect.anchoredPosition = new Vector2(-10f, 0f);

            viewport.GetComponent<UEImage>().color = Colors.black;

            // Scroll rect setup
            scrollRect.content = contentRect;
            scrollRect.viewport = viewportRect;
            scrollRect.vertical = scrollType.HasFlag(ScrollType.Vertical);
            scrollRect.horizontal = scrollType.HasFlag(ScrollType.Horizontal);

            scrollRect.elasticity = 0f;
            scrollRect.scrollSensitivity = 150f;
            scrollRect.movementType = UnityEngine.UI.ScrollRect.MovementType.Clamped;

            // Scroll bar setup
            if (scrollRect.vertical == true) {
                scrollBarV = new ScrollBar(ScrollType.Vertical);
                Add(gameObject, scrollBarV);
                scrollRect.verticalScrollbar = scrollBarV.scrollBar;
            }

            // TODO: Fix horizontal scrollbars
            if (scrollRect.horizontal == true) {
                scrollBarH = new ScrollBar(ScrollType.Horizontal);
                Add(gameObject, scrollBarH);
                scrollRect.horizontalScrollbar = scrollBarH.scrollBar;
            }

            SetAnchor(AnchorType.Middle, FillType.Fill);
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
    }
}
