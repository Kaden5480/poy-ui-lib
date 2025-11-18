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
    }
}
