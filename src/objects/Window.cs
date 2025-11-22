using UnityEngine;

using UECanvas = UnityEngine.Canvas;
using UEGraphicRaycaster = UnityEngine.UI.GraphicRaycaster;
using UECanvasScaler = UnityEngine.UI.CanvasScaler;

namespace UILib {
    public class Window : UIObject {
        internal GameObject canvasObj;
        internal UECanvas canvas;

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
    }
}
