using UnityEngine;

namespace UILib {
    public class Window : UIObject {
        private const float minWidth = 200f;
        private const float minHeight = 200f;

        public bool fullscreen { get; private set; }
        private WindowState state;
        private Vector2 latestDragPosition;

        public string name { get; private set; }

        internal Canvas canvas;
        internal TopBar topBar { get; private set; }
        private GameObject content;
        public ScrollView scrollView { get; private set; }

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
            contentRect.anchoredPosition = new Vector2(
                0f, -((topBarHeight + 2*topBarPadding) / 2)
            );
            contentRect.sizeDelta = new Vector2(
                0f, -(topBarHeight + 2*topBarPadding)
            );

            // Add scroll view
            scrollView = new ScrollView();
            scrollView.Fill();
            Add(content, scrollView);

            SetAnchor(AnchorType.Middle);
            SetSize(width, height);

            AddResizeButton();
        }

        /**
         * <summary>
         * Adjusts the scrollbar and adds in a button
         * to resize.
         * </summary>
         */
        internal virtual void AddResizeButton() {
            RectTransform rect = scrollView.scrollBarV.rectTransform;
            rect.anchoredPosition = new Vector2(0f, 10f);
            rect.sizeDelta = new Vector2(20f, -20f);

            scrollView.Add(
                scrollView.gameObject,
                new ResizeButton(this)
            );
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
            scrollView.SetLayout(layoutType);
        }

        /**
         * <summary>
         * Sets the alignment of the child elements for the layout.
         * </summary>
         * <param name="alignment">The alignment to use</param>
         */
        public override void SetLayoutAlignment(TextAnchor alignment) {
            scrollView.SetLayoutAlignment(alignment);
        }

        /**
         * <summary>
         * Sets the spacing for the layout.
         * </summary>
         * <param name="spacing">The spacing to use</param>
         */
        public override void SetLayoutSpacing(float spacing) {
            scrollView.SetLayoutSpacing(spacing);
        }

        /**
         * <summary>
         * Sets the padding for the layout.
         * </summary>
         * <param name="left">The left padding to use</param>
         * <param name="right">The right padding to use</param>
         * <param name="top">The top padding to use</param>
         * <param name="bottom">The bottom padding to use</param>
         */
        public override void SetLayoutPadding(
            int left = 0,
            int right = 0,
            int top = 0,
            int bottom = 0
        ) {
            scrollView.SetLayoutPadding(left, right, top, bottom);
        }

        /**
         * <summary>
         * Adds the provided component as a child to this one.
         * </summary>
         * <param name="child">The object which should be a child of this object</param>
         */
        public override void Add(UIObject child) {
            scrollView.Add(child);
        }

        /**
         * <summary>
         * Converts a position to be local to
         * the Canvas of this Window.
         * </summary>
         * <returns>The local position</returns>
         */
        internal Vector2 ToCanvasLocal(Vector2 position) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.rectTransform, position, canvas.canvas.worldCamera,
                out Vector2 canvasLocal
            );

            return canvasLocal;
        }

        /**
         * <summary>
         * Makes this window fullscreen.
         * </summary>
         */
        public void BeginFullscreen() {
            if (fullscreen == true) {
                return;
            }

            state = new WindowState(this);
            Fill();
            rectTransform.anchoredPosition = Vector2.zero;
            topBar.fullscreenButton.label.text.text = "-";
            fullscreen = true;
        }

        /**
         * <summary>
         * Returns this window to a windowed state.
         * </summary>
         * <returns>Whether the fullscreen mode was even changed</returns>
         */
        public bool EndFullscreen() {
            if (fullscreen == false) {
                return false;
            }

            if (state != null) {
                state.Restore(this);
                state = null;
            }

            topBar.fullscreenButton.label.text.text = "+";
            fullscreen = false;
            return true;
        }

        /**
         * <summary>
         * Returns this window to a windowed state
         * and also to a given position.
         * </summary>
         * <param name="position">The world position to restore to</param>
         */
        public void EndFullscreen(Vector2 position) {
            if (EndFullscreen() == false) {
                return;
            }

            // Fix local position
            Vector2 sizeDelta = rectTransform.sizeDelta;
            Vector2 pivot = rectTransform.pivot;

            rectTransform.localPosition = new Vector2(
                position.x + sizeDelta.x*(pivot.x-0.5f),
                position.y - sizeDelta.y*(1f-pivot.y)
            );
        }

        /**
         * <summary>
         * Bring this Window's canvas to the front
         * </summary>
         */
        protected override void OnClick() {
            UIRoot.BringToFront(this);
        }

        /**
         * <summary>
         * Handles this Window being dragged from anywhere (not just the top bar).
         * </summary>
         * <param name="position">The position the drag started at</param>
         */
        protected override void OnBeginDrag(Vector2 position) {
            UIRoot.BringToFront(this);

            // Only listen to drag events when Alt is held
            if (Input.GetKey(KeyCode.LeftAlt) == false) {
                return;
            }

            // In fullscreen mode, only moving is allowed
            if (fullscreen == true && Input.GetMouseButton(0) == false) {
                return;
            }

            // Otherwise, dragging is allowed
            HandleBeginDrag(position);
        }

        /**
         * <summary>
         * Handles this Window being dragged from anywhere (not just the top bar).
         * </summary>
         * <param name="position">The position dragged to</param>
         */
        protected override void OnDrag(Vector2 position) {
            // Only listen to drag events when Alt is held
            if (Input.GetKey(KeyCode.LeftAlt) == false) {
                return;
            }

            // If dragging with lmb, move
            if (Input.GetMouseButton(0) == true) {
                HandleMove(position);
            }
            // If dragging with rmb, resize (only in windowed mode)
            else if (fullscreen == false
                && Input.GetMouseButton(1) == true
            ) {
                HandleResize(position);
            }
        }

        /**
         * <summary>
         * Handles starting to drag this window.
         * </summary>
         * <param name="position">The position dragging started at</param>
         */
        internal virtual void HandleBeginDrag(Vector2 position) {
            position = ToCanvasLocal(position);

            EndFullscreen(position);
            latestDragPosition = position;

        }

        /**
         * <summary>
         * Handles moving this window.
         * </summary>
         * <param name="position">The position dragged to</param>
         */
        internal virtual void HandleMove(Vector2 position) {
            position = ToCanvasLocal(position);

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
            position = ToCanvasLocal(position);

            ResizeBy(position - latestDragPosition);
            latestDragPosition = position;
        }

        /**
         * <summary>
         * Move this window by a given delta.
         * </summary>
         */
        private void MoveBy(Vector3 delta) {
            rectTransform.localPosition += delta;
        }

        /**
         * <summary>
         * Resizes this window by a given delta.
         * </summary>
         */
        private void ResizeBy(Vector3 delta) {
            Vector2 pivot = rectTransform.pivot;
            Vector2 oldSize = rectTransform.sizeDelta;

            float newX = Mathf.Clamp(oldSize.x + delta.x, minWidth, 1920f);
            float newY = Mathf.Clamp(oldSize.y - delta.y, minHeight, 1080f);

            rectTransform.sizeDelta = new Vector2(newX, newY);

            float realDeltaX = newX - oldSize.x;
            float realDeltaY = newY - oldSize.y;

            MoveBy(new Vector3(realDeltaX*pivot.x, -(realDeltaY*(1-pivot.y)), 0f));
        }
    }
}
