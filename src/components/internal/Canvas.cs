using UnityEngine;

using UECanvas = UnityEngine.Canvas;
using UECanvasScaler = UnityEngine.UI.CanvasScaler;
using UEGraphicRaycaster = UnityEngine.UI.GraphicRaycaster;

namespace UILib {
    internal class Canvas : BaseComponent {
        private UECanvas canvas;
        private UECanvasScaler scaler;

        internal Canvas() {
            canvas = root.AddComponent<UECanvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            scaler = root.AddComponent<UECanvasScaler>();
            scaler.uiScaleMode = UECanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = UECanvasScaler.ScreenMatchMode.Expand;

            root.AddComponent<UEGraphicRaycaster>();
        }
    }
}

