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

        // Click listener
        internal ClickHandler clickHandler { get; private set; }

        // The parent and children of this object
        protected UIObject parent         { get; private set; }
        protected List<UIObject> children { get; private set; }

        private LayoutElement layoutElement;

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

            clickHandler = gameObject.AddComponent<ClickHandler>();
            clickHandler.AddListener(BringToFront);

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
         * Bring the canvas this UIObject is attached to, if any,
         * to the front.
         *
         * NOTE:
         * This is implemented differently in Window to
         * actually handle bringing the UI to the front.
         * Otherwise, it will just do nothing.
         * </summary>
         */
        public virtual void BringToFront() {
            if (parent != null) {
                parent.BringToFront();
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
        protected static void SetParent(GameObject parent, GameObject child) {
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
        internal static void SetLayout(GameObject obj, LayoutType layoutType) {
            switch (layoutType) {
                case LayoutType.None:
                    return;
                case LayoutType.Vertical:
                    VerticalLayoutGroup vertical = obj.AddComponent<VerticalLayoutGroup>();
                    vertical.childForceExpandWidth = true;
                    vertical.childForceExpandHeight = false;
                    vertical.childControlWidth = true;
                    vertical.childControlHeight = true;
                    vertical.padding = new RectOffset(20, 20, 20, 20);
                    break;
                case LayoutType.Horizontal:
                    HorizontalLayoutGroup horizontal = obj.AddComponent<HorizontalLayoutGroup>();
                    horizontal.childForceExpandWidth = false;
                    horizontal.childForceExpandHeight = true;
                    horizontal.childControlWidth = true;
                    horizontal.childControlHeight = true;
                    horizontal.padding = new RectOffset(20, 20, 20, 20);
                    break;
                default:
                    return;
            }

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
    }
}
