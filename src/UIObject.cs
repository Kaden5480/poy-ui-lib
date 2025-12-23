using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using UILib.Behaviours;
using UILib.Components;
using UILib.Layouts;

namespace UILib {
    /**
     * <summary>
     * The base class which most objects
     * inherit from.
     *
     * You'd rarely want to use this class directly though.
     * Most of the time you'd want to use <see cref="UIComponent"/>.
     *
     * Provides most of the necessary functionality.
     * </summary>
     */
    public abstract class UIObject {
        // The logger for this UIObject
        internal Logger logger { get; private set; }

        /**
         * <summary>
         * The underlying `GameObject` this UIObject attaches
         * components to.
         * </summary>
         */
        public GameObject gameObject { get; private set; }

        /**
         * <summary>
         * The `RectTransform` attached to <see cref="gameObject"/>.
         * </summary>
         */
        public RectTransform rectTransform { get; private set; }

        /**
         * <summary>
         * This UIObject's tooltip.
         * </summary>
         */
        public string tooltip { get; private set; }

        /**
         * <summary>
         * The theme currently applied to this UIObject.
         * </summary>
         */
        public Theme theme { get; private set; }

        /**
         * <summary>
         * The currently configured width of this UIObject.
         * </summary>
         */
        protected float width { get; private set; }

        /**
         * <summary>
         * The currently configured height of this UIObject.
         * </summary>
         */
        protected float height { get; private set; }

        /**
         * <summary>
         * Whether this UIObject is currently set to be visible.
         * </summary>
         */
        public bool isVisible { get => gameObject.activeSelf; }

        /**
         * <summary>
         * The parent of this UIObject.
         * </summary>
         */
        public UIObject parent { get; private set; }

        /**
         * <summary>
         * This UIObject's children.
         * </summary>
         */
        public List<UIObject> children { get; private set; }

        // Where layouts should be handled and children should be added
        // by default
        private UIObject content;

#region Enable/Hover/Click/Drag Events

        internal MouseHandler mouseHandler { get; private set; }

        /**
         * <summary>
         * Invokes listeners when this UIObject is enabled (shown).
         * </summary>
         */
        public virtual UnityEvent onEnable { get => mouseHandler.onEnable; }

        /**
         * <summary>
         * Invokes listeners when this UIObject is disabled (hidden).
         * </summary>
         */
        public virtual UnityEvent onDisable { get => mouseHandler.onDisable; }

        /**
         * <summary>
         * Invokes listeners whenever this UIObject is hovered over.
         * </summary>
         */
        public virtual UnityEvent onPointerEnter { get => mouseHandler.onPointerEnter; }

        /**
         * <summary>
         * Invokes listeners whenever this UIObject is no longer hovered over.
         * </summary>
         */
        public virtual UnityEvent onPointerExit { get => mouseHandler.onPointerExit; }

        /**
         * <summary>
         * Invokes listeners when the pointer is held down on this UIObject.
         * </summary>
         */
        public virtual UnityEvent onPointerDown { get => mouseHandler.onPointerDown; }

        /**
         * <summary>
         * Invokes listeners when the pointer is no longer being
         * held down on this UIObject.
         * </summary>
         */
        public virtual UnityEvent onPointerUp { get => mouseHandler.onPointerUp; }

        /**
         * <summary>
         * Invokes listeners whenever this UIObject is clicked.
         * </summary>
         */
        public virtual UnityEvent onClick { get => mouseHandler.onClick; }

        /**
         * <summary>
         * Invokes listeners whenever this UIObject is double clicked.
         * </summary>
         */
        public virtual UnityEvent onDoubleClick { get => mouseHandler.onDoubleClick; }

        /**
         * <summary>
         * Invokes listeners whenever the cursor has started
         * dragging on this UIObject.
         * </summary>
         */
        public virtual MouseEvent onBeginDrag { get => mouseHandler.onBeginDrag; }

        /**
         * <summary>
         * Invokes listeners whenever the cursor is
         * dragging on this UIObject.
         * </summary>
         */
        public virtual MouseEvent onDrag { get => mouseHandler.onDrag; }

        /**
         * <summary>
         * Invokes listeners whenever the cursor has
         * stopped dragging on this UIObject.
         * </summary>
         */
        public virtual MouseEvent onEndDrag { get => mouseHandler.onEndDrag; }

#endregion

#region Layout

        /**
         * <summary>
         * The current anchor set on this UIObject.
         * </summary>
         */
        public AnchorType anchorType { get; private set; }

        /**
         * <summary>
         * The current fill set on this UIObject.
         * </summary>
         */
        public FillType fillType { get; private set; }

        /**
         * <summary>
         * The `LayoutElement` for this UIObject, if it has one.
         * </summary>
         */
        public LayoutElement layoutElement { get; private set; }

        /**
         * <summary>
         * The current layout set on this UIObject's content.
         * It's important to note that this is set on the
         * configured <see cref="SetContent">content</see>.
         * </summary>
         */
        public LayoutType layoutType { get; private set; }

        /**
         * <summary>
         * The `ContentSizeFitter` for this UIObject's content, if it has one.
         * It's important to note that this is added to the
         * configured <see cref="SetContent">content</see>.
         * </summary>
         */
        public ContentSizeFitter fitter { get; private set; }

        /**
         * <summary>
         * The `LayoutGroup` for this UIObject's content, if it has one.
         * It's important to note that this is added to the
         * configured <see cref="SetContent">content</see>,
         * not necessarily on the <see cref="gameObject"/>.
         * </summary>
         */
        public HorizontalOrVerticalLayoutGroup layoutGroup { get; private set; }

#endregion

        /**
         * <summary>
         * Initializes a UIObject.
         * </summary>
         */
        public UIObject() {
            Type type = GetType();

            logger = new Logger(type);
            gameObject = new GameObject($"{type}");
            gameObject.layer = LayerMask.NameToLayer("UI");
            rectTransform = gameObject.AddComponent<RectTransform>();

            children = new List<UIObject>();

            mouseHandler = gameObject.AddComponent<MouseHandler>();
            onPointerEnter.AddListener(OnPointerEnter);
            onPointerExit.AddListener(OnPointerExit);
            onPointerDown.AddListener(OnPointerDown);
            onPointerUp.AddListener(OnPointerUp);
            onClick.AddListener(OnClick);
            onDoubleClick.AddListener(OnDoubleClick);
            onBeginDrag.AddListener(OnBeginDrag);
            onDrag.AddListener(OnDrag);
            onEndDrag.AddListener(OnEndDrag);

            // Tooltip events
            onPointerEnter.AddListener(ShowTooltip);
            onPointerExit.AddListener(HideTooltip);
            onDisable.AddListener(HideTooltip);

            SetAnchor(AnchorType.Middle);

            // By default, just make this the content
            content = this;

            // Set the default theme
            theme = Theme.GetThemeUnsafe();
        }

        /**
         * <summary>
         * Converts a position to be local to this UIObject's
         * <see cref="rectTransform">RectTransform</see>.
         * </summary>
         * <param name="position">The position to convert</param>
         * <returns>The local position relative to the RectTransform</returns>
         */
        internal Vector2 ToLocalPosition(Vector2 position) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, position, null,
                out Vector2 local
            );

            return local;
        }

        /**
         * <summary>
         * Toggles the visibility of this UIObject.
         * </summary>
         */
        public virtual void ToggleVisibility() {
            // Write in terms of Hide and Show
            // to make overriding a little easier
            if (isVisible == true) {
                Hide();
            }
            else {
                Show();
            }
        }

        /**
         * <summary>
         * Shows this UIObject.
         * </summary>
         */
        public virtual void Show() {
            gameObject.SetActive(true);
        }

        /**
         * <summary>
         * Hides this UIObject.
         * </summary>
         */
        public virtual void Hide() {
            gameObject.SetActive(false);
        }

        /**
         * <summary>
         * Sets a different component to be the `content`.
         *
         * By default, the `content` will just be the <see cref="gameObject"/>
         * of this UIObject. However, some components need to set custom
         * content objects as in some cases the children should be
         * added to a different UIObject.
         *
         * A good example of this would be
         * <see cref="ScrollView">ScrollViews</see>,
         * which are composed of a variety of `GameObjects`
         * like so:
         * <code>
         * - ScrollView
         *   - Viewport
         *     - Content
         *   - ScrollBar
         *   - ScrollBar
         * </code>
         *
         * In general adding to the <see cref="Components.ScrollView"/> directly doesn't make much
         * sense, you usually want to add to the "Content" `GameObject` instead.
         * So, <see cref="Components.ScrollView">ScrollViews</see> configure their content
         * to be the "Content", rather than "ScrollView".
         * </summary>
         * <param name="content">The component which should be the `content` instead</param>
         */
        public virtual void SetContent(UIComponent content) {
            this.content = content;
        }

        /**
         * <summary>
         * Allows setting the theme of just this UIObject.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected abstract void SetThisTheme(Theme theme);

        /**
         * <summary>
         * Allows setting the theme of this UIObject
         * and all children.
         *
         * Passing a `theme` of `null` will result in
         * the default theme being applied instead.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        public virtual void SetTheme(Theme theme) {
            if (theme == null) {
                theme = Theme.GetThemeUnsafe();
            }

            this.theme = theme;
            foreach (UIObject child in children) {
                child.SetTheme(theme);
            }

            SetThisTheme(theme);
        }

#region Handling Parents/Children

        /**
         * <summary>
         * Sets a GameObject to be the parent of another.
         * </summary>
         * <param name="parent">The GameObject which should be the parent</param>
         * <param name="child">The GameObject which should be the child</param>
         */
        internal static void SetParent(GameObject parent, GameObject child) {
            child.transform.SetParent(
                parent.transform, false
            );
        }

        /**
         * <summary>
         * Adds a child to this UIObject but with a custom
         * defined parent GameObject.
         *
         * > [!NOTE]
         * > This UIObject will still be marked as the parent and have
         * > the child added as its child, this simply changes the GameObject
         * > that's used as the parent in the GameObject hierarchy.
         * </summary>
         * <param name="parent">The GameObject to use as a parent instead</param>
         * <param name="child">The child to add</param>
         * <param name="setTheme">Whether the child should inherit this object's theme</param>
         */
        internal void Add(GameObject parent, UIObject child, bool setTheme = false) {
            if (layoutGroup != null) {
                child.AddLayoutElement();
            }

            if (child.parent != null) {
                child.parent.RemoveChild(child);
            }

            if (setTheme == true) {
                child.SetTheme(theme);
            }

            SetParent(parent, child.gameObject);
            child.parent = this;
            children.Add(child);
        }

        /**
         * <summary>
         * Adds a component directly to this UIObject,
         * ignoring the configured <see cref="SetContent">content</see>.
         * </summary>
         * <param name="child">The child to add</param>
         * <param name="setTheme">Whether the child should inherit this object's theme immediately</param>
         */
        public virtual void AddDirect(UIComponent child, bool setTheme = false) {
            Add(gameObject, child, setTheme);
        }

        /**
         * <summary>
         * Adds a component to the
         * <see cref="SetContent">content</see>.
         * </summary>
         * <param name="child">The child to add</param>
         * <param name="setTheme">Whether the child should inherit the content's theme immediately</param>
         */
        public virtual void AddContent(UIComponent child, bool setTheme = false) {
            if (content == this) {
                AddDirect(child, setTheme);
                return;
            }

            content.Add(child, setTheme);
        }

        /**
         * <summary>
         * Shorthand for <see cref="AddContent"/>.
         * </summary>
         * <param name="child">The child to add</param>
         * <param name="setTheme">Whether the child should inherit the content's theme immediately</param>
         */
        public void Add(UIComponent child, bool setTheme = false) {
            AddContent(child, setTheme);
        }

        /**
         * <summary>
         * Removes a child from this UIObject.
         * </summary>
         * <param name="child">The child to remove</param>
         */
        private void RemoveChild(UIObject child) {
            children.Remove(child);
        }

        /**
         * <summary>
         * Destroy this UIObject and all children.
         * </summary>
         */
        public virtual void Destroy() {
            if (parent != null) {
                parent.RemoveChild(this);
            }

            GameObject.DestroyImmediate(gameObject);

            for (int i = 0; i < children.Count; i++) {
                children[i].Destroy();
            }

            HideTooltip();
        }

#endregion

#region Tooltips

        /**
         * <summary>
         * Sets the tooltip for this UIObject.
         * </summary>
         * <param name="tooltip">The tooltip to display</param>
         */
        public void SetTooltip(string tooltip) {
            this.tooltip = tooltip;
        }

        /**
         * <summary>
         * Displays this UIObject's tooltip.
         * </summary>
         */
        internal void ShowTooltip() {
            if (UIRoot.tooltipOverlay == null) {
                return;
            }

            if (string.IsNullOrEmpty(tooltip) == true) {
                if (parent != null) {
                    parent.ShowTooltip();
                }

                return;
            }

            UIRoot.tooltipOverlay.ShowTooltip(this);
        }

        /**
         * <summary>
         * Hides this UIObject's tooltip.
         * </summary>
         */
        internal void HideTooltip() {
            if (UIRoot.tooltipOverlay == null) {
                return;
            }

            UIRoot.tooltipOverlay.HideTooltip(this);
        }

#endregion

#region Events

        /**
         * <summary>
         * Destroys the <see cref="MouseHandler"/> on this object.
         * </summary>
         */
        internal void DestroyMouseHandler() {
            GameObject.DestroyImmediate(mouseHandler);
            mouseHandler = null;
        }

        /**
         * <summary>
         * Handles this UIObject being hovered over.
         * </summary>
         */
        protected virtual void OnPointerEnter() {
            if (parent != null) {
                parent.OnPointerEnter();
            }
        }

        /**
         * <summary>
         * Handles this UIObject no longer being hovered over.
         * </summary>
         */
        protected virtual void OnPointerExit() {
            if (parent != null) {
                parent.OnPointerExit();
            }
        }

        /**
         * <summary>
         * Handles the pointer being held down on this UIObject.
         * </summary>
         */
        protected virtual void OnPointerDown() {
            if (parent != null) {
                parent.OnPointerDown();
            }
        }

        /**
         * <summary>
         * Handles the pointer no longer being held down on this UIObject.
         * </summary>
         */
        protected virtual void OnPointerUp() {
            if (parent != null) {
                parent.OnPointerUp();
            }
        }

        /**
         * <summary>
         * Handles this UIObject being clicked.
         * </summary>
         */
        protected virtual void OnClick() {
            if (parent != null) {
                parent.OnClick();
            }
        }

        /**
         * <summary>
         * Handles this UIObject being double clicked.
         * </summary>
         */
        protected virtual void OnDoubleClick() {
            if (parent != null) {
                parent.OnDoubleClick();
            }
        }

        /**
         * <summary>
         * Handles the start of this UIObject being dragged.
         * </summary>
         * <param name="position">The position the drag started at</param>
         */
        protected virtual void OnBeginDrag(Vector2 position) {
            if (parent != null) {
                parent.OnBeginDrag(position);
            }
        }

        /**
         * <summary>
         * Handles this UIObject being dragged.
         * </summary>
         * <param name="position">The position dragged to</param>
         */
        protected virtual void OnDrag(Vector2 position) {
            if (parent != null) {
                parent.OnDrag(position);
            }
        }

        /**
         * <summary>
         * Handles the end of this UIObject being dragged.
         * </summary>
         * <param name="position">The position the drag ended at</param>
         */
        protected virtual void OnEndDrag(Vector2 position) {
            if (parent != null) {
                parent.OnEndDrag(position);
            }
        }

#endregion

#region Layout for Self

        /**
         * <summary>
         * Adds a `LayoutElement` to this UIObject
         * to allow automatically managing how
         * it's layed out in its parent container.
         *
         * In general, you won't need to call this
         * since whenever you add a UIObject to another, it
         * will automatically get a `LayoutElement` if the
         * parent being added to has a layout configured already.
         *
         * `LayoutElements` only do anything if the parent container has
         * a layout set on it (vertical or horizontal).
         * </summary>
         */
        public void AddLayoutElement() {
            if (layoutElement != null) {
                return;
            }

            layoutElement = gameObject.AddComponent<LayoutElement>();
            SetSize(width, height);
            SetFill(fillType);
        }

        /**
         * <summary>
         * Sets the size of this UIObject.
         *
         * If you use a certain fill, this size behaves differently.
         * Instead of being the actual width and height of the UIObject, they
         * will add/subtract from the width/height, depending on which fills are used.
         *
         * If you don't use a fill, both width and height will remain as the width and height.
         * Their behaviour will be unchanged.
         *
         * If you use a <see cref="FillType.Horizontal"/> fill, you could specify a width such as
         * `-20f` to subtract 20 pixels from the full width.
         *
         * If you use a <see cref="FillType.Vertical"/> fill, you could specify a height such as
         * `100f` and it would add 100 pixels to the full height.
         *
         * Similarly, if you use choose to fill <see cref="FillType.All"/> the space, any
         * positive values will increase this UIObject beyond its normal fill. Any negative
         * values will decrease
         * it from its normal fill.
         *
         * </summary>
         * <param name="width">The width to set</param>
         * <param name="height">The height to set</param>
         */
        public virtual void SetSize(float width, float height) {
            this.width = width;
            this.height = height;

            if (layoutElement != null) {
                layoutElement.preferredWidth = width;
                layoutElement.preferredHeight = height;
            }
            else {
                rectTransform.sizeDelta = new Vector2(width, height);
            }
        }

        /**
         * <summary>
         * Sets an offset for this UIObject from its current anchor.
         * </summary>
         * <param name="x">The x offset to set</param>
         * <param name="y">The y offset to set</param>
         */
        public void SetOffset(float x, float y) {
            rectTransform.anchoredPosition = new Vector2(x, y);
        }

        /**
         * <summary>
         * Sets the anchor type of this UIObject.
         * This determines how it will be positioned within its
         * parent container.
         * </summary>
         * <param name="anchorType">The type of anchor to use</param>
         * <param name="fillType">The type of fill to use</param>
         */
        public void SetAnchor(AnchorType anchorType) {
            this.anchorType = anchorType;

            Vector2 vec = Vector2.zero;

            switch (anchorType) {
                // Top
                case AnchorType.TopLeft:
                    vec = new Vector2(0f, 1f);
                    break;
                case AnchorType.TopMiddle:
                    vec = new Vector2(0.5f, 1f);
                    break;
                case AnchorType.TopRight:
                    vec = Vector2.one;
                    break;

                // Middle
                case AnchorType.MiddleLeft:
                    vec = new Vector2(0f, 0.5f);
                    break;
                case AnchorType.Middle:
                    vec = new Vector2(0.5f, 0.5f);
                    break;
                case AnchorType.MiddleRight:
                    vec = new Vector2(1f, 0.5f);
                    break;

                // Bottom
                case AnchorType.BottomLeft:
                    vec = Vector2.zero;
                    break;
                case AnchorType.BottomMiddle:
                    vec = new Vector2(0.5f, 0f);
                    break;
                case AnchorType.BottomRight:
                    vec = new Vector2(1f, 0f);
                    break;

                default:
                    logger.LogDebug($"Unexpected anchor type: {anchorType}");
                    return;
            }

            rectTransform.anchorMin = vec;
            rectTransform.anchorMax = vec;
            rectTransform.pivot     = vec;

            // Reapply fill
            SetFill(fillType);
        }

        /**
         * <summary>
         * Sets the fill using a LayoutElement if this UIObject has one.
         * This is handled specially as layout elements are a bit weird.
         * </summary>
         * <param name="fillType">The type of fill to use</param>
         */
        private void SetElementFill(FillType fillType) {
            switch (fillType) {
                case FillType.None:
                    SetSize(width, height);
                    break;
                case FillType.Vertical:
                    SetSize(width, -1f);
                    break;
                case FillType.Horizontal:
                    SetSize(-1f, height);
                    break;
                case FillType.All:
                    SetSize(-1f, -1f);
                    break;
                default:
                    logger.LogDebug($"Unexpected fill type: {fillType}");
                    break;
            }
        }

        /**
         * <summary>
         * Sets the fill type of this UIObject.
         * This determines how this UIObject will fill its
         * parent container.
         * </summary>
         * <param name="fillType">The type of fill to use</param>
         */
        public void SetFill(FillType fillType) {
            this.fillType = fillType;

            if (layoutElement != null) {
                SetElementFill(fillType);
                return;
            }

            Vector2 currAnchorMin = rectTransform.anchorMin;
            Vector2 currAnchorMax = rectTransform.anchorMax;
            Vector2 currPivot     = rectTransform.pivot;
            Vector2 currSizeDelta = rectTransform.sizeDelta;

            switch (fillType) {
                case FillType.None:
                    break;

                case FillType.Vertical:
                    rectTransform.anchorMin = new Vector2(currAnchorMin.x, 0f);
                    rectTransform.anchorMax = new Vector2(currAnchorMax.x, 1f);
                    rectTransform.pivot     = new Vector2(currPivot.x,     0.5f);
                    rectTransform.sizeDelta = new Vector2(currSizeDelta.x, 0f);
                    break;

                case FillType.Horizontal:
                    rectTransform.anchorMin = new Vector2(0f,   currAnchorMin.y);
                    rectTransform.anchorMax = new Vector2(1f,   currAnchorMax.y);
                    rectTransform.pivot     = new Vector2(0.5f, currPivot.y);
                    rectTransform.sizeDelta = new Vector2(0f,   currSizeDelta.y);
                    break;

                case FillType.All:
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.pivot     = new Vector2(0.5f, 0.5f);
                    rectTransform.sizeDelta = Vector2.zero;
                    break;

                default:
                    logger.LogDebug($"Unexpected fill type: {fillType}");
                    return;
            }
        }

#endregion

#region Layout for Content

        /**
         * <summary>
         * Sets the layout to be used on a given `GameObject`.
         * </summary>
         * <param name="obj">The object to set the layout for</param>
         * <param name="layoutType">The type of layout to use</param>
         */
        private void SetLayout(GameObject obj, LayoutType layoutType) {
            if (layoutGroup != null) {
                GameObject.DestroyImmediate(layoutGroup);
            }

            this.layoutType = layoutType;

            switch (layoutType) {
                case LayoutType.None:
                    return;
                case LayoutType.Vertical:
                    layoutGroup = obj.AddComponent<VerticalLayoutGroup>();
                    break;
                case LayoutType.Horizontal:
                    layoutGroup = obj.AddComponent<HorizontalLayoutGroup>();
                    break;
                default:
                    return;
            }

            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;

            SetElementAlignment(AnchorType.Middle);

            if (fitter == null) {
                fitter = obj.AddComponent<ContentSizeFitter>();
            }

            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        /**
         * <summary>
         * Sets the layout to be used on the configured
         * <see cref="SetContent">content</see> for this UIObject.
         * </summary>
         * <param name="layoutType">The type of layout to use</param>
         */
        public void SetContentLayout(LayoutType layoutType) {
            if (content != this) {
                content.SetContentLayout(layoutType);
                return;
            }

            SetLayout(gameObject, layoutType);
        }

        /**
         * <summary>
         * Sets the alignment of the child elements
         * of the <see cref="SetContent">content</see>.
         *
         * This requires you to have already called <see cref="SetContentLayout"/>
         * to configure a layout for the <see cref="SetContent">content</see>.
         * </summary>
         * <param name="alignment">The alignment to use</param>
         */
        public void SetElementAlignment(AnchorType alignment) {
            if (content != this) {
                content.SetElementAlignment(alignment);
                return;
            }

            if (layoutGroup == null) {
                logger.LogDebug("No layout group, can't apply element alignment");
                return;
            }

            layoutGroup.childAlignment = AnchorToText(alignment);
        }

        /**
         * <summary>
         * Sets the spacing between child elements of the
         * <see cref="SetContent">content</see>.
         *
         * This requires you to have already called <see cref="SetContentLayout"/>
         * to configure a layout for the <see cref="SetContent">content</see>.
         * </summary>
         * <param name="spacing">The spacing to use</param>
         */
        public void SetElementSpacing(float spacing) {
            if (content != this) {
                content.SetElementSpacing(spacing);
                return;
            }

            if (layoutGroup == null) {
                logger.LogDebug("No layout group, can't apply element spacing");
                return;
            }

            layoutGroup.spacing = spacing;
        }

        /**
         * <summary>
         * Sets the padding to apply to the
         * <see cref="SetContent">content</see>.
         *
         * This requires you to have already called <see cref="SetContentLayout"/>
         * to configure a layout for the <see cref="SetContent">content</see>.
         * </summary>
         * <param name="left">The left padding to use</param>
         * <param name="right">The right padding to use</param>
         * <param name="top">The top padding to use</param>
         * <param name="bottom">The bottom padding to use</param>
         */
        public void SetContentPadding(
            int left = 0,
            int right = 0,
            int top = 0,
            int bottom = 0
        ) {
            if (content != this) {
                content.SetContentPadding(left, right, top, bottom);
                return;
            }

            if (layoutGroup == null) {
                logger.LogDebug("No layout group, can't apply padding");
                return;
            }

            layoutGroup.padding = new RectOffset(left, right, top, bottom);
        }

        /**
         * <summary>
         * Sets the padding to apply to the
         * <see cref="SetContent">content</see>.
         *
         * This requires you to have already called <see cref="SetContentLayout"/>
         * to configure a layout for the <see cref="SetContent">content</see>.
         * </summary>
         * <param name="horizontal">The horizontal padding to use</param>
         * <param name="vertical">The vertical padding to use</param>
         */
        public void SetContentPadding(int horizontal, int vertical) {
            SetContentPadding(horizontal, horizontal, vertical, vertical);
        }

        /**
         * <summary>
         * Sets the padding to apply to the
         * <see cref="SetContent">content</see>.
         *
         * This requires you to have already called <see cref="SetContentLayout"/>
         * to configure a layout for the <see cref="SetContent">content</see>.
         * </summary>
         * <param name="padding">The padding to use all around</param>
         */
        public void SetContentPadding(int padding) {
            SetContentPadding(padding, padding);
        }

        /**
         * <summary>
         * Converts an <see cref="Layouts.AnchorType"/> to a Unity `TextAnchor`.
         * </summary>
         * <param name="anchor">The anchor to convert</param>
         * <returns>The converted anchor</returns>
         */
        public TextAnchor AnchorToText(AnchorType anchor) {
            switch (anchor) {
                case AnchorType.TopLeft:
                    return TextAnchor.UpperLeft;
                case AnchorType.TopMiddle:
                    return TextAnchor.UpperCenter;
                case AnchorType.TopRight:
                    return TextAnchor.UpperRight;

                case AnchorType.MiddleLeft:
                    return TextAnchor.MiddleLeft;
                case AnchorType.Middle:
                    return TextAnchor.MiddleCenter;
                case AnchorType.MiddleRight:
                    return TextAnchor.MiddleRight;

                case AnchorType.BottomLeft:
                    return TextAnchor.LowerLeft;
                case AnchorType.BottomMiddle:
                    return TextAnchor.LowerCenter;
                case AnchorType.BottomRight:
                    return TextAnchor.LowerRight;

                // Shouldn't happen
                default:
                    logger.LogError($"What is this anchor? `{anchor}`");
                    return TextAnchor.MiddleCenter;
            }
        }

#endregion

    }
}
