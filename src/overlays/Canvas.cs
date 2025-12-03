using UnityEngine;
using UnityEngine.UI;

using UECanvas = UnityEngine.Canvas;
using UECanvasScaler = UnityEngine.UI.CanvasScaler;
using UEGraphicRaycaster = UnityEngine.UI.GraphicRaycaster;

namespace UILib {
    /**
     * <summary>
     * A Canvas which each <see cref="Overlay"/> gets attached to.
     *
     * Every overlay has its own canvas to handle
     * sorting them on top of each other.
     * </summary>
     */
    internal class Canvas : UIObject {
        internal UECanvas canvas { get; private set; }
        internal UEGraphicRaycaster raycaster { get; private set; }

        /**
         * <summary>
         * Initializes a Canvas.
         * </summary>
         */
        internal Canvas() {
            canvas = gameObject.AddComponent<UECanvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = -1;

            UECanvasScaler scaler = gameObject.AddComponent<UECanvasScaler>();
            scaler.uiScaleMode = UECanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = UECanvasScaler.ScreenMatchMode.Expand;

            raycaster = gameObject.AddComponent<UEGraphicRaycaster>();
        }

        /**
         * <summary>
         * Adds a child to this canvas.
         * </summary>
         * <param name="child">The child to add</param>
         */
        internal void Add(UIObject child) {
            Add(gameObject, child);
        }
    }
}
