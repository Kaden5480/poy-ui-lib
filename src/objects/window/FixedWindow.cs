using UnityEngine;

namespace UILib {
    public class FixedWindow : UIObject {
        public string name { get; private set; }

        internal Canvas canvas;

        internal TopBar topBar { get; private set; }
        private GameObject content;

        /**
         * <summary>
         * Initializes a FixedWindow.
         * </summary>
         * <param name="name">The name of the window</param>
         * <param name="width">The width of the window</param>
         * <param name="height">The height of the window</param>
         */
        public FixedWindow(string name, float width, float height) {
            this.name = name;

            // Get a canvas to draw this window on
            canvas = new Canvas();
            canvas.Add(this);

            UIRoot.Register(this);

            // The top bar
            float topBarHeight = 25f;

            topBar = new TopBar(this, topBarHeight);
            Add(gameObject, topBar);

            // The content
            content = new GameObject("Content");
            SetParent(gameObject, content);

            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.anchoredPosition = new Vector2(0f, -(topBarHeight / 2));
            contentRect.sizeDelta        = new Vector2(0f, -(topBarHeight));

            SetAnchor(AnchorType.Middle);
            SetSize(width, height);
        }

        /**
         * <summary>
         * Destroy this FixedWindow and all children.
         * </summary>
         */
        public override void Destroy() {
            UIRoot.Unregister(this);
            base.Destroy();
        }

        /**
         * <summary>
         * Sets the layout to be used on this FixedWindow.
         * </summary>
         * <param name="layoutType">The type of layout to use</param>
         */
        public override void SetLayout(LayoutType layoutType) {
            SetLayout(content, layoutType);
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
