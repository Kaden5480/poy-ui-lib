using UnityEngine;

using UECanvas = UnityEngine.Canvas;
using UECanvasScaler = UnityEngine.UI.CanvasScaler;
using UEGraphicRaycaster = UnityEngine.UI.GraphicRaycaster;

namespace UILib {
    internal class Canvas : UIObject {
        internal UECanvas canvas { get; private set; }

        public Canvas() {
            canvas = gameObject.AddComponent<UECanvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            UECanvasScaler scaler = gameObject.AddComponent<UECanvasScaler>();
            scaler.uiScaleMode = UECanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = UECanvasScaler.ScreenMatchMode.Expand;

            gameObject.AddComponent<UEGraphicRaycaster>();
        }
    }
}
