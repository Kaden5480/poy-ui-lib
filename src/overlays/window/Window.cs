using UnityEngine;

using UILib.Behaviours;
using UILib.Components;
using UILib.Layouts;

namespace UILib {
    /**
     * <summary>
     * A Window object. Like an <see cref="Overlay"/> but more dynamic.
     *
     * This object is one of the fundamental building blocks of UILib.
     *
     * It supports being moved, resized, maximised, closed.
     *
     * It also has a specially configured <see cref="ScrollView"/> built in.
     * </summary>
     */
    public class Window : Overlay {
        /**
         * <summary>
         * This window's name.
         * </summary>
         */
        public string name { get; private set; }

        /**
         * <summary>
         * The minimum width of this window.
         * </summary>
         */
        public float minWidth { get; private set; } = 200f;

        /**
         * <summary>
         * The minimum height of this window.
         * </summary>
         */
        public float minHeight { get; private set; } = 200f;

        /**
         * <summary>
         * The maximum width of this window.
         * </summary>
         */
        public float maxWidth { get; private set; } = 1920f;

        /**
         * <summary>
         * The maximum height of this window.
         * </summary>
         */
        public float maxHeight { get; private set; } = 1080f;

        /**
         * <summary>
         * Whether this window is currently in fullscreen mode.
         * </summary>
         */
        public bool fullscreen { get; private set; }

        /**
         * <summary>
         * This window's underlying <see cref="Components.ScrollView"/>.
         * </summary>
         */
        public ScrollView scrollView { get; private set; }

        /**
         * <summary>
         * This window's <see cref="TitleBar"/>.
         * Contains the name and close and fullscreen buttons.
         * </summary>
         */
        public TitleBar titleBar { get; private set; }

        /**
         * <summary>
         * This window's <see cref="ResizeButton"/>.
         * </summary>
         */
        public ResizeButton resizeButton { get; private set; }

        // States stored for helping with moving/resizing/maximising/etc.
        private WindowState state;
        private Vector2 latestDragPosition;

        // Fade for canvas groups
        private Fade decorationFade;

        // Canvas groups for fading in/out components
        private CanvasGroup cgTitleBar;
        private CanvasGroup cgScrollBarH;
        private CanvasGroup cgScrollBarV;
        private CanvasGroup cgResizeButton;

        /**
         * <summary>
         * Initializes a Window.
         * </summary>
         * <param name="name">The name of the window</param>
         * <param name="width">The width of the window</param>
         * <param name="height">The height of the window</param>
         */
        public Window(string name, float width, float height) : base(width, height) {
            this.name = name;

            width = Mathf.Max(width, minWidth);
            height = Mathf.Max(height, minHeight);

            // The title bar
            float titleBarHeight = 20f;
            int titleBarPadding = 5;

            titleBar = new TitleBar(this, titleBarHeight, titleBarPadding);
            AddDirect(titleBar);

            // Modify container dimensions
            container.SetSize(0f, -(titleBarHeight + 2*titleBarPadding));
            container.SetOffset(0f, -(titleBarHeight + 2*titleBarPadding) / 2);

            // Add scroll view
            scrollView = new ScrollView();
            scrollView.SetFill(FillType.All);
            container.AddDirect(scrollView);

            SetAnchor(AnchorType.Middle);
            SetSize(width, height);

            AddResizeButton();

            // The scroll view is the content
            SetContent(scrollView);

            // The group for fading window decorations
            decorationFade = gameObject.AddComponent<Fade>();

            // The groups themselves
            AddCanvasGroup(ref cgTitleBar, titleBar);
            AddCanvasGroup(ref cgScrollBarH, scrollView.scrollBarH);
            AddCanvasGroup(ref cgScrollBarV, scrollView.scrollBarV);
            AddCanvasGroup(ref cgResizeButton, resizeButton);

            decorationFade.Add(cgTitleBar);
            decorationFade.Add(cgScrollBarH);
            decorationFade.Add(cgScrollBarV);
            decorationFade.Add(cgResizeButton);

            // Set the theme
            SetThisTheme(theme);
        }

        /**
         * <summary>
         * Sets the name of this window.
         * </summary>
         * <param name="name">The window's new name</param>
         */
        public void SetName(string name) {
            this.name = name;
            titleBar.SetName(name);
        }

        /**
         * <summary>
         * Updates opacities for canvas groups.
         * </summary>
         * <param name="group">The group to update the opacity of</param>
         * <param name="opacity">The opacity to set on it</param>
         */
        private void SetCanvasGroupOpacity(CanvasGroup group, float opacity) {
            if (group != null) {
                group.alpha = opacity;
            }
        }

        /**
         * <summary>
         * Assigns canvas groups if they don't exist
         * </summary>
         * <param name="group">Where to save the group to</param>
         * <param name="uiObject">The object to add a group to</param>
         */
        private void AddCanvasGroup(ref CanvasGroup group, UIObject uiObject) {
            if (group != null) {
                return;
            }

            group = uiObject.gameObject.GetComponent<CanvasGroup>();

            if (group == null) {
                group = uiObject.gameObject.AddComponent<CanvasGroup>();
            }

            group.alpha = theme.windowDecorationOpacity;
        }

        /**
         * <summary>
         * Allows setting the theme of this window.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {
            base.SetThisTheme(theme);

            if (decorationFade == null) {
                return;
            }

            // Set the background opacity
            Color bg = scrollView.background.color;
            bg.a = theme.windowOpacity;
            scrollView.background.color = bg;

            // Set fade times and opacities on decorations
            decorationFade.SetFadeTime(theme.windowDecorationFadeTime);
            decorationFade.SetOpacities(max: theme.windowDecorationOpacity);
            SetCanvasGroupOpacity(cgTitleBar, theme.windowDecorationOpacity);
            SetCanvasGroupOpacity(cgScrollBarH, theme.windowDecorationOpacity);
            SetCanvasGroupOpacity(cgScrollBarV, theme.windowDecorationOpacity);
            SetCanvasGroupOpacity(cgResizeButton, theme.windowDecorationOpacity);
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

            resizeButton = new ResizeButton(this);
            scrollView.AddDirect(resizeButton);
        }

#region Interactions

        /**
         * <summary>
         * Enables interacting with this window.
         *
         * Note that this also changes the visibility of the
         * window decorations (title bar, scroll bar, resize button).
         * </summary>
         */
        public override void Enable() {
            base.Enable();
            decorationFade.FadeIn();
        }

        /**
         * <summary>
         * Disables interacting with this window.
         *
         * Note that this also changes the visibility of the
         * window decorations (title bar, scroll bar, resize button).
         * </summary>
         */
        public override void Disable() {
            base.Disable();
            decorationFade.FadeOut();
        }

#endregion

#region Scrolling

        /**
         * <summary>
         * Scroll to the top of this window.
         * </summary>
         */
        public void ScrollToTop() {
            scrollView.ScrollToTop();
        }

        /**
         * <summary>
         * Scroll to the bottom of this window.
         * </summary>
         */
        public void ScrollToBottom() {
            scrollView.ScrollToBottom();
        }

#endregion

#region Fullscreen

        /**
         * <summary>
         * Makes this window fullscreen.
         *
         * You shouldn't normally need to call this method
         * as it's automatically used internally.
         * </summary>
         */
        public void BeginFullscreen() {
            if (fullscreen == true) {
                return;
            }

            state = new WindowState(this);
            SetFill(FillType.All);
            SetOffset(0f, 0f);
            titleBar.fullscreenButton.label.text.text = "-";
            fullscreen = true;
        }

        /**
         * <summary>
         * Returns this window to a windowed state.
         *
         * You shouldn't normally need to call this method
         * as it's automatically used internally.
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
         *
         * You shouldn't normally need to call this method
         * as it's automatically used internally.
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

#endregion

#region Events

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
         * Handles this Window being dragged from anywhere (not just the top bar).
         * </summary>
         * <param name="position">The position the drag started at</param>
         */
        protected override void OnBeginDrag(Vector2 position) {
            // Only listen to drag events when Alt is held
            if (Input.GetKey(KeyCode.LeftAlt) == false) {
                BringToFront();
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
            BringToFront();
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

#endregion

#region Moving/Resizing

        /**
         * <summary>
         * Sets the minimum dimensions for this window.
         * </summary>
         * <param name="width">The minimum width</param>
         * <param name="height">The minimum height</param>
         */
        public void SetMinSize(float width, float height) {
            minWidth = width;
            minHeight = height;
            ResizeBy(Vector3.zero);
        }

        /**
         * <summary>
         * Sets the maximum dimensions for this window.
         * </summary>
         * <param name="width">The maximum width</param>
         * <param name="height">The maximum height</param>
         */
        public void SetMaxSize(float width, float height) {
            maxWidth = width;
            maxHeight = height;
            ResizeBy(Vector3.zero);
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

            float newX = Mathf.Clamp(oldSize.x + delta.x, minWidth, maxWidth);
            float newY = Mathf.Clamp(oldSize.y - delta.y, minHeight, maxHeight);

            rectTransform.sizeDelta = new Vector2(newX, newY);

            float realDeltaX = newX - oldSize.x;
            float realDeltaY = newY - oldSize.y;

            MoveBy(new Vector3(realDeltaX*pivot.x, -(realDeltaY*(1-pivot.y)), 0f));
        }

#endregion

    }
}
