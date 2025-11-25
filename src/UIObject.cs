using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UILib.Behaviours;
using UILib.Components;
using UILib.Layout;

namespace UILib {
    /**
     * <summary>
     * The base class which most objects
     * inherit from.
     *
     * You'd rarely want to use this class directly though.
     * Most of the time you'd want to use UIComponent.
     *
     * Provides most of the necessary functionality.
     * </summary>
     */
    public abstract class UIObject {
        protected float width  { get; private set; }
        protected float height { get; private set; }

        // The logger for this UIObject
        internal Logger logger { get; private set; }

        // The GameObject and RectTransform this UIObject handles
        public GameObject gameObject       { get; private set; }
        public RectTransform rectTransform { get; private set; }

        // Click and drag listeners
        internal MouseHandler mouseHandler { get; private set; }

        // The parent and children of this object
        public UIObject parent         { get; private set; }
        public List<UIObject> children { get; private set; }

        // Where layouts should be handled and children should be added
        private UIObject content;

        // Layout information
        public AnchorType anchorType                       { get; private set; }
        public FillType fillType                           { get; private set; }
        public LayoutType layoutType                       { get; private set; }
        public LayoutElement layoutElement                 { get; private set; }
        public ContentSizeFitter fitter                    { get; private set; }
        public HorizontalOrVerticalLayoutGroup layoutGroup { get; private set; }

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
            mouseHandler.AddClickListener(OnClick);
            mouseHandler.AddDoubleClickListener(OnDoubleClick);
            mouseHandler.AddBeginDragListener(OnBeginDrag);
            mouseHandler.AddDragListener(OnDrag);
            mouseHandler.AddEndDragListener(OnEndDrag);

            SetAnchor(AnchorType.Middle);

            // By default, just make this the content
            content = this;
        }

        /**
         * <summary>
         * Sets a different component to be the content.
         * </summary>
         * <param name="content">The component which should be the content instead</param>
         */
        public virtual void SetContent(UIComponent content) {
            this.content = content;
        }

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
         * Adds a child to an object with a custom defined parent.
         *
         * NOTE:
         * This UIObject will still be marked as the parent and have
         * the child added as its child, this simply changes the GameObject
         * that's used as the parent in the GameObject hierarchy.
         * </summary>
         * <param name="parent">The GameObject to use as a parent instead</param>
         * <param name="child">The child to add</param>
         */
        internal void Add(GameObject parent, UIObject child) {
            if (layoutGroup != null) {
                child.AddLayoutElement();
            }

            if (child.parent != null) {
                child.parent.RemoveChild(child);
            }

            SetParent(parent, child.gameObject);
            child.parent = this;
            children.Add(child);
        }

        /**
         * <summary>
         * Adds a component as a child.
         * </summary>
         * <param name="child">The child to add</param>
         */
        public virtual void Add(UIComponent child) {
            if (content == this) {
                Add(gameObject, child);
                return;
            }

            content.Add(child);
        }

        /**
         * <summary>
         * Removes a child from this object.
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
        }

        /**
         * <summary>
         * Destroys the mouse handler on this object.
         * </summary>
         */
        internal void DestroyMouseHandler() {
            GameObject.DestroyImmediate(mouseHandler);
            mouseHandler = null;
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

        /**
         * <summary>
         * Shows this UIObject.
         * </summary>
         */
        public void Show() {
            gameObject.SetActive(true);
        }

        /**
         * <summary>
         * Hides this UIObject.
         * </summary>
         */
        public void Hide() {
            gameObject.SetActive(false);
        }

        /**
         * <summary>
         * Adds a layout element to this UIObject
         * to allow automatically managing layouts.
         * </summary>
         */
        public void AddLayoutElement() {
            if (layoutElement != null) {
                return;
            }

            layoutElement = content.gameObject.AddComponent<LayoutElement>();
            SetSize(width, height);
            SetFill(fillType);
        }

        /**
         * <summary>
         * Sets the size of this UIObject.
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
         * Sets the anchor type of this object.
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
         * Sets the fill for layout elements.
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
         * Sets the fill type of this object.
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

        /**
         * <summary>
         * Sets the layout to be used on a given GameObject.
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

            SetElementAlignment(TextAnchor.MiddleCenter);

            fitter = obj.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        /**
         * <summary>
         * Sets the layout to be used on this UIObject.
         * </summary>
         * <param name="layoutType">The type of layout to use</param>
         */
        public void SetLayout(LayoutType layoutType) {
            if (content != this) {
                content.SetLayout(layoutType);
                return;
            }

            SetLayout(gameObject, layoutType);
        }

        /**
         * <summary>
         * Sets the alignment of the child elements for the layout.
         * </summary>
         * <param name="alignment">The alignment to use</param>
         */
        public void SetElementAlignment(TextAnchor alignment) {
            if (content != this) {
                content.SetElementAlignment(alignment);
                return;
            }

            if (layoutGroup == null) {
                logger.LogDebug("No layout group, can't apply spacing");
                return;
            }

            layoutGroup.childAlignment = alignment;
        }

        /**
         * <summary>
         * Sets the spacing between elements for the layout.
         * </summary>
         * <param name="spacing">The spacing to use</param>
         */
        public void SetElementSpacing(float spacing) {
            if (content != this) {
                content.SetElementSpacing(spacing);
                return;
            }

            if (layoutGroup == null) {
                logger.LogDebug("No layout group, can't apply spacing");
                return;
            }

            layoutGroup.spacing = spacing;
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
        public void SetLayoutPadding(
            int left = 0,
            int right = 0,
            int top = 0,
            int bottom = 0
        ) {
            if (content != this) {
                content.SetLayoutPadding(left, right, top, bottom);
                return;
            }

            if (layoutGroup == null) {
                logger.LogDebug("No layout group, can't apply padding");
                return;
            }

            layoutGroup.padding = new RectOffset(left, right, top, bottom);
        }
    }
}
