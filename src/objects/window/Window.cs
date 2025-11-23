using UnityEngine;

namespace UILib {
    public class Window : UIObject {
        private const float minWidth = 200f;
        private const float minHeight = 200f;

        public string name { get; private set; }

        public bool fullscreen { get; private set; }
        private WindowState state;

        private Vector2 latestDragPosition;

        internal Canvas canvas;

        internal TopBar topBar { get; private set; }
        private GameObject content;

        /**
         * <summary>
         * Initializes a Window.
         * </summary>
         * <param name="name">The name of the window</param>
         * <param name="width">The width of the window</param>
         * <param name="height">The height of the window</param>
         */
        public Window(string name, float width, float height) {
            this.name = name;

            width = Mathf.Max(width, minWidth);
            height = Mathf.Max(height, minHeight);

            // Get a canvas to draw this window on
            canvas = new Canvas();
            canvas.Add(this);

            UIRoot.Register(this);

            // The top bar
            float topBarHeight = 20f;
            int topBarPadding = 5;

            topBar = new TopBar(this, topBarHeight, topBarPadding);
            Add(gameObject, topBar);

            // The content
            content = new GameObject("Content");
            SetParent(gameObject, content);

            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.anchoredPosition = new Vector2(0f, -((topBarHeight + 2*topBarPadding) / 2));
            contentRect.sizeDelta        = new Vector2(0f, -(topBarHeight + 2*topBarPadding));

            SetAnchor(AnchorType.Middle);
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
         * Sets the layout to be used on this Window.
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

        /**
         * <summary>
         * Handles changing windowing mode.
         * </summary>
         */
        public virtual void HandleWindowingChange() {
            if (fullscreen == false) {
                state = new WindowState(this);
                SetAnchor(AnchorType.Middle, FillType.Fill);
                rectTransform.anchoredPosition = Vector2.zero;
            }
            else if (state != null) {
                state.Restore(this);
                state = null;
            }

            fullscreen = !fullscreen;
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
         * Handles this Window being dragged from anywhere (not just the top bar).
         * </summary>
         * <param name="position">The position the drag started at</param>
         */
        public override void OnBeginDrag(Vector2 position) {
            if (Input.GetKey(KeyCode.LeftAlt) == false) {
                UIRoot.BringToFront(this);
                return;
            }

            HandleBeginDrag(position);
        }

        /**
         * <summary>
         * Handles this Window being dragged from anywhere (not just the top bar).
         * </summary>
         * <param name="position">The position dragged to</param>
         */
        public override void OnDrag(Vector2 position) {
            if (Input.GetKey(KeyCode.LeftAlt) == false) {
                return;
            }

            HandleMove(position);
            HandleResize(position);
        }


        /**
         * <summary>
         * Handles starting to drag this window.
         * </summary>
         * <param name="position">The position dragging started at</param>
         */
        internal virtual void HandleBeginDrag(Vector2 position) {
            UIRoot.BringToFront(this);
            latestDragPosition = position;
        }

        /**
         * <summary>
         * Handles moving this window.
         * </summary>
         * <param name="position">The position dragged to</param>
         */
        internal virtual void HandleMove(Vector2 position) {
            if (fullscreen == true) {
                return;
            }

            if (Input.GetMouseButton(0) == false) {
                return;
            }

            MoveBy(position - latestDragPosition);
            latestDragPosition = position;
        }

        /**
         * <summary>
         * Handles resizing this window.
         * </summary>
         * <param name="position">The position dragged to</param>
         */
        internal virtual void HandleResize(Vector2 position) {
            if (fullscreen == true) {
                return;
            }

            if (Input.GetMouseButton(1) == false) {
                return;
            }

            ResizeBy(position - latestDragPosition);
            latestDragPosition = position;
        }

        /**
         * <summary>
         * Move this window by a given delta.
         * </summary>
         */
        public void MoveBy(Vector3 delta) {
            rectTransform.localPosition += delta;
        }

        /**
         * <summary>
         * Resizes this window by a given delta.
         * </summary>
         */
        public void ResizeBy(Vector3 delta) {
            Vector2 oldDelta = rectTransform.sizeDelta;

            float newX = Mathf.Clamp(oldDelta.x + delta.x, minWidth, 1920f);
            float newY = Mathf.Clamp(oldDelta.y - delta.y, minHeight, 1080f);

            rectTransform.sizeDelta = new Vector2(newX, newY);

            float realDeltaX = newX - oldDelta.x;
            float realDeltaY = newY - oldDelta.y;

            MoveBy(new Vector3(realDeltaX/2, -realDeltaY/2, 0f));
        }
    }
}
