using System.Collections.Generic;

using UnityEngine;

using UECanvas = UnityEngine.Canvas;
using UECanvasScaler = UnityEngine.UI.CanvasScaler;
using UEGraphicRaycaster = UnityEngine.UI.GraphicRaycaster;

namespace UILib {
    internal class Canvas : BaseComponent {
        private const int minSortingOrder = 1000;
        private static List<UECanvas> canvases = new List<UECanvas>();

        private UECanvas canvas;
        private UECanvasScaler scaler;

        internal Canvas() {
            canvas = root.AddComponent<UECanvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            scaler = root.AddComponent<UECanvasScaler>();
            scaler.uiScaleMode = UECanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = UECanvasScaler.ScreenMatchMode.Expand;

            root.AddComponent<UEGraphicRaycaster>();

            Register(canvas);
        }

        /**
         * <summary>
         * Set this canvas to be on top.
         * </summary>
         */
        internal void BringToFront() {
            SetOnTop(canvas);
        }

        /**
         * <summary>
         * Sets a canvas to be above all others.
         * </summary>
         */
        private static void SetOnTop(UECanvas canvas) {
            // Try finding the canvas
            int index = canvases.IndexOf(canvas);

            if (index < 0) {
                return;
            }

            // Iterate the list in reverse, decrementing all sorting orders
            // until reaching the canvas to set on top
            for (int i = canvases.Count - 1; i > index; i--) {
                canvases[i].sortingOrder--;
            }

            // Now remove the canvas from the list, and add it back
            // to the end, while also updating the sorting order
            canvases.Remove(canvas);
            canvas.sortingOrder = minSortingOrder + canvases.Count;
            canvases.Add(canvas);
        }

        /**
         * <summary>
         * Registers a canvas so it can be sorted.
         * </summary>
         */
        private static void Register(UECanvas canvas) {
            canvas.sortingOrder = minSortingOrder + canvases.Count;
            canvases.Add(canvas);
        }
    }
}

