using UnityEngine;

using UEImage = UnityEngine.UI.Image;

namespace UILib {
    public class Window : UIObject {
        public string name { get; private set; }

        internal Canvas canvas;

        private TopBar topBar;
        private GameObject content;

        /**
         * <summary>
         * Initializes a Window.
         * </summary>
         * <param name="name">The name of this window</param>
         * <param name="width">The width of this Window</param>
         * <param name="height">The height of this Window</param>
         */
        public Window(string name, float width, float height) {
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

            SetSize(width, height);
        }

        /**
         * <summary>
         * Destroy this Window and all children.
         * </summary>
         */
        public override void Destroy() {
            UIRoot.Unregister(this);
            base.Destroy();
        }

        /**
         * <summary>
         * Bring this Window's canvas to the front
         * </summary>
         */
        public override void OnClick() {
            UIRoot.BringToFront(this);
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
         * Sets the layout to be used on this Window.
         * </summary>
         * <param name="layoutType">The type of layout to use</param>
         */
        public override void SetLayout(LayoutType layoutType) {
            SetLayout(content, layoutType);
        }

        /**
         * <summary>
         * Move this window by a given delta.
         * </summary>
         */
        public void MoveBy(Vector3 delta) {
            rectTransform.localPosition += delta;
        }
    }
}
