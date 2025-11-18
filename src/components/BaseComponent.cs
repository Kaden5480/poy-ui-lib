using System;
using UnityEngine;
using UnityEngine.UI;

namespace UILib {
    public abstract class BaseComponent {
        internal Logger logger;

        protected GameObject root;
        protected RectTransform rectTransform;

        /**
         * <summary>
         * Initializes a BaseComponent.
         * </summary>
         */
        public BaseComponent() {
            Type type = GetType();
            logger = new Logger(type);

            // Root object of this UI component
            root = new GameObject($"{type}");
            rectTransform = root.AddComponent<RectTransform>();

            SetAnchor(AnchorType.Middle);
        }

        /**
         * <summary>
         * Makes a GameObject/BaseComponent the child of a GameObject.
         * </summary>
         * <param name="parent">The parent object</param>
         * <param name="child">The object which should be a child of the parent</param>
         */
        protected static void MakeChild(GameObject parent, GameObject child) {
            child.transform.SetParent(parent.transform, false);
        }

        protected static void MakeChild(GameObject parent, BaseComponent child) {
            MakeChild(parent, child.root);
        }

        /**
         * <summary>
         * Adds the provided component as a child to this one.
         * </summary>
         * <param name="child">The object which should be a child of this object</param>
         */
        public virtual void AddChild(BaseComponent child) {
            MakeChild(root, child.root);
        }

        /**
         * <summary>
         * Shows this component.
         * </summary>
         */
        public virtual void Show() {
            root.SetActive(true);
        }

        /**
         * <summary>
         * Hides this component.
         * </summary>
         */
        public virtual void Hide() {
            root.SetActive(false);
        }

        /**
         * <summary>
         * Sets the root object to not destroy on load.
         * </summary>
         */
        public virtual void DontDestroyOnLoad() {
            GameObject.DontDestroyOnLoad(root);
        }

        /**
         * <summary>
         * Forcefully rebuilds the layout of this component.
         * </summary>
         */
        public virtual void RebuildLayout() {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        /**
         * <summary>
         * Sets the size of this component.
         * </summary>
         * <param name="width">The width to set</param>
         * <param name="height">The height to set</param>
         */
        public virtual void SetSize(float width, float height) {
            rectTransform.sizeDelta = new Vector2(width, height);
        }

        /**
         * <summary>
         * Sets the anchor type of this component.
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
         * Sets the fill type of this component.
         * </summary>
         * <param name="fillType">The type of fill to use</param>
         */
        protected virtual void SetFill(FillType fillType) {
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
    }
}
