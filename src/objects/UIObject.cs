using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace UILib {
    public class UIObject {
        // The logger for this UIObject
        internal Logger logger { get; private set; }

        // The GameObject and RectTransform this UIObject handles
        internal GameObject gameObject       { get; private set; }
        internal RectTransform rectTransform { get; private set; }

        // Click and drag listeners
        internal MouseHandler mouseHandler { get; private set; }

        // The parent and children of this object
        public UIObject parent         { get; private set; }
        public List<UIObject> children { get; private set; }

        private LayoutElement layoutElement;
        private HorizontalOrVerticalLayoutGroup layoutGroup;

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
            mouseHandler.AddBeginDragListener(OnBeginDrag);
            mouseHandler.AddDragListener(OnDrag);
            mouseHandler.AddEndDragListener(OnEndDrag);

            SetAnchor(AnchorType.Middle);
        }

        /**
         * <summary>
         * Destroy this UIObject and all children.
         * </summary>
         */
        public virtual void Destroy() {
            GameObject.DestroyImmediate(gameObject);
            foreach (UIObject child in children) {
                child.Destroy();
            }
        }

        /**
         * <summary>
         * Handles this UIObject being clicked.
         *
         * NOTE:
         * This is implemented differently in Window and Notification
         * Otherwise, it will just keep forwarding to the parent.
         * </summary>
         */
        public virtual void OnClick() {
            if (parent != null) {
                parent.OnClick();
            }
        }

        /**
         * <summary>
         * Handles this UIObject being dragged.
         * </summary>
         * <param name="position">The position the drag started at</param>
         */
        public virtual void OnBeginDrag(Vector2 position) {
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
        public virtual void OnDrag(Vector2 position) {
            if (parent != null) {
                parent.OnDrag(position);
            }
        }

        /**
         * <summary>
         * Handles this UIObject being dragged.
         * </summary>
         * <param name="position">The position the drag started at</param>
         */
        public virtual void OnEndDrag(Vector2 position) {
            if (parent != null) {
                parent.OnEndDrag(position);
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
        protected virtual void Add(GameObject parent, UIObject child) {
            SetParent(parent, child.gameObject);
            child.parent = this;
            children.Add(child);
        }

        /**
         * <summary>
         * Adds a child to this object.
         * </summary>
         * <param name="child">The child to add</param>
         */
        public virtual void Add(UIObject child) {
            Add(gameObject, child);
        }

        /**
         * <summary>
         * Adds a layout element to this UIObject
         * to allow automatically managing layouts.
         * </summary>
         */
        public virtual void AddLayoutElement() {
            layoutElement = gameObject.AddComponent<LayoutElement>();
        }

        /**
         * <summary>
         * Sets the size of this UIObject.
         * </summary>
         * <param name="width">The width to set</param>
         * <param name="height">The height to set</param>
         */
        public virtual void SetSize(float width, float height) {
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
        public virtual void SetAnchor(AnchorType anchorType, FillType fillType = FillType.None) {
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

            SetFill(fillType);
        }

        /**
         * <summary>
         * Sets the fill type of this object.
         * </summary>
         * <param name="fillType">The type of fill to use</param>
         */
        internal virtual void SetFill(FillType fillType) {
            Vector2 currAnchorMin = rectTransform.anchorMin;
            Vector2 currAnchorMax = rectTransform.anchorMax;
            Vector2 currPivot     = rectTransform.pivot;
            Vector2 currSizeDelta = rectTransform.sizeDelta;

            switch (fillType) {
                case FillType.None:
                    break;

                case FillType.FillVertical:
                    rectTransform.anchorMin = new Vector2(currAnchorMin.x, 0f);
                    rectTransform.anchorMax = new Vector2(currAnchorMax.x, 1f);
                    rectTransform.pivot     = new Vector2(currPivot.x,     0.5f);
                    rectTransform.sizeDelta = new Vector2(currSizeDelta.x, 0f);
                    break;

                case FillType.FillHorizontal:
                    rectTransform.anchorMin = new Vector2(0f,   currAnchorMin.y);
                    rectTransform.anchorMax = new Vector2(1f,   currAnchorMax.y);
                    rectTransform.pivot     = new Vector2(0.5f, currPivot.y);
                    rectTransform.sizeDelta = new Vector2(0f,   currSizeDelta.y);
                    break;

                case FillType.Fill:
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
         * Sets this component to fill all space.
         * </summary>
         */
        public virtual void Fill() {
            SetAnchor(AnchorType.TopLeft, FillType.Fill);
        }

        /**
         * <summary>
         * Sets the layout to be used on a given GameObject.
         * </summary>
         * <param name="obj">The object to set the layout for</param>
         * <param name="layoutType">The type of layout to use</param>
         */
        internal void SetLayout(GameObject obj, LayoutType layoutType) {
            if (layoutGroup != null) {
                GameObject.DestroyImmediate(layoutGroup);
            }

            switch (layoutType) {
                case LayoutType.None:
                    return;
                case LayoutType.Vertical:
                    layoutGroup = obj.AddComponent<VerticalLayoutGroup>();
                    layoutGroup.childForceExpandWidth = true;
                    layoutGroup.childForceExpandHeight = false;
                    break;
                case LayoutType.Horizontal:
                    layoutGroup = obj.AddComponent<HorizontalLayoutGroup>();
                    layoutGroup.childForceExpandWidth = false;
                    layoutGroup.childForceExpandHeight = true;
                    break;
                default:
                    return;
            }

            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;

            SetLayoutAlignment(TextAnchor.MiddleCenter);

            ContentSizeFitter fitter = obj.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        /**
         * <summary>
         * Sets the layout to be used on this UIObject.
         * </summary>
         * <param name="layoutType">The type of layout to use</param>
         */
        public virtual void SetLayout(LayoutType layoutType) {
            SetLayout(gameObject, layoutType);
        }

        /**
         * <summary>
         * Sets the alignment of the child elements for the layout.
         * </summary>
         * <param name="alignment">The alignment to use</param>
         */
        public virtual void SetLayoutAlignment(TextAnchor alignment) {
            if (layoutGroup == null) {
                logger.LogDebug("No layout group, can't apply spacing");
                return;
            }

            layoutGroup.childAlignment = alignment;
        }

        /**
         * <summary>
         * Sets the spacing for the layout.
         * </summary>
         * <param name="spacing">The spacing to use</param>
         */
        public virtual void SetLayoutSpacing(float spacing) {
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
        public virtual void SetLayoutPadding(
            int left = 0,
            int right = 0,
            int top = 0,
            int bottom = 0
        ) {
            if (layoutGroup == null) {
                logger.LogDebug("No layout group, can't apply padding");
                return;
            }

            layoutGroup.padding = new RectOffset(left, right, top, bottom);
        }
    }
}
