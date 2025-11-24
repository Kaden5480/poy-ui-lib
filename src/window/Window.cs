using UnityEngine;

using UILib.Components;
using UILib.Layout;

namespace UILib {
    /**
     * <summary>
     * A Window object.
     *
     * This object is one of the fundamental building blocks of UILib.
     *
     * It supports being moved, resized, maximised, closed, moved
     * on top of other windows.
     *
     * It also has a ScrollView built in.
     * </summary>
     */
    public class Window : UIObject {
        // The minimum width and height of windows
        private const float minWidth = 200f;
        private const float minHeight = 200f;

        // Whether this window is fullscreen
        public bool fullscreen { get; private set; }

        // States stored for helping with moving/resizing/maximising/etc.
        private WindowState state;
        private Vector2 latestDragPosition;

        // The name of this window
        public string name { get; private set; }

        // The canvas this window is attached to
        internal Canvas canvas;

        // This window's title bar
        internal TitleBar titleBar { get; private set; }

        // This window's container
        private GameObject container;

        // The ScrollView for the window
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

            // The title bar
            float titleBarHeight = 20f;
            int titleBarPadding = 5;

            titleBar = new TitleBar(this, titleBarHeight, titleBarPadding);
            Add(gameObject, titleBar);

            // The container
            container = new GameObject("Content");
            SetParent(gameObject, container);

            RectTransform containerRect = container.AddComponent<RectTransform>();
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.anchoredPosition = new Vector2(
                0f, -((titleBarHeight + 2*titleBarPadding) / 2)
            );
            containerRect.sizeDelta = new Vector2(
                0f, -(titleBarHeight + 2*titleBarPadding)
            );

            // Add scroll view
            scrollView = new ScrollView();
            scrollView.SetFill(FillType.All);
            Add(container, scrollView);

            SetAnchor(AnchorType.Middle);
            SetSize(width, height);

            AddResizeButton();

            // The scroll view is the content
            base.SetContent(scrollView);
        }

        /**
         * <summary>
         * Sets a different component to be the content.
         * </summary>
         * <param name="content">The component which should be the content instead</param>
         */
        public override void SetContent(UIComponent content) {
            scrollView.SetContent(content);
        }

        /**
         * <summary>
         * Adjusts the scrollbar and adds in a button
         * to resize.
         * </summary>
         */
        internal virtual void AddResizeButton() {
            float scrollBarWidth = scrollView.scrollBarWidth;

            RectTransform rect = scrollView.scrollBarV.rectTransform;
            rect.anchoredPosition = new Vector2(0f, scrollBarWidth/2);
            rect.sizeDelta = new Vector2(scrollBarWidth, -scrollBarWidth);

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
         * Scroll to the top of this window.
         * </summary>
         */
        public void ScrollToTop() {
            scrollView.scrollBarV.SetScroll(1f);
        }

        /**
         * <summary>
         * Scroll to the bottom of this window.
         * </summary>
         */
        public void ScrollToBottom() {
            scrollView.scrollBarV.SetScroll(0f);
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
            SetFill(FillType.All);
            rectTransform.anchoredPosition = Vector2.zero;
            titleBar.fullscreenButton.label.text.text = "-";
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
                state.Restore();
                state = null;
            }

            titleBar.fullscreenButton.label.text.text = "+";
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
            // Only listen to drag events when Alt is held
            if (Input.GetKey(KeyCode.LeftAlt) == false) {
                UIRoot.BringToFront(this);
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
            UIRoot.BringToFront(this);
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
