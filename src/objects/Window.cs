using UnityEngine;

using UECanvas = UnityEngine.Canvas;
using UECanvasScaler = UnityEngine.UI.CanvasScaler;
using UEGraphicRaycaster = UnityEngine.UI.GraphicRaycaster;
using UEImage = UnityEngine.UI.Image;

namespace UILib {
    public class Window : UIObject {
        internal GameObject canvasObj;
        internal UECanvas canvas;

        private TopBar topBar;
        private GameObject content;

        /**
         * <summary>
         * Initializes a Window.
         * </summary>
         * <param name="width">The width of this Window</param>
         * <param name="height">The height of this Window</param>
         */
        public Window(float width, float height) {
            // Get a canvas to draw this window on
            canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<UECanvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            UECanvasScaler scaler = canvasObj.AddComponent<UECanvasScaler>();
            scaler.uiScaleMode = UECanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = UECanvasScaler.ScreenMatchMode.Expand;

            canvasObj.AddComponent<UEGraphicRaycaster>();

            SetParent(canvasObj, gameObject);

            UIRoot.Register(this);

            // The top bar
            topBar = new TopBar(this, 20f);
            Add(gameObject, topBar);

            // The content
            content = new GameObject("Content");
            SetParent(gameObject, content);

            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.anchoredPosition = new Vector2(0f, -10f);
            contentRect.sizeDelta        = new Vector2(0f, -20f);

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
         * Bring this Window's canvas to the front
         * </summary>
         */
        public override void BringToFront() {
            UIRoot.BringToFront(this);
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
         * Move this window by a given delta.
         * </summary>
         */
        public void MoveBy(Vector3 delta) {
            rectTransform.localPosition += delta;
        }
    }
}
