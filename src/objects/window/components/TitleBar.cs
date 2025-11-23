using UnityEngine;
using UEImage = UnityEngine.UI.Image;

namespace UILib {
    /**
     * <summary>
     * The title bar which is placed at
     * the top of all windows.
     * </summary>
     */
    internal class TitleBar : UIObject {
        // An area for holding the buttons
        private Area buttonArea;

        // The button for entering/exiting fullscreen
        internal Button fullscreenButton { get; private set; }

        // The button for closing this Window
        internal Button closeButton      { get; private set; }

        // The height of this TitleBar
        private float height;

        // The Window this TitleBar is attached to
        private Window window;

        /**
         * <summary>
         * Initializes a TitleBar.
         * </summary>
         * <param name="window">The Window this TitleBar  is attached to</param>
         * <param name="height">The height to use for this TitleBar</param>
         * <param name="padding">The padding to use</param>
         */
        internal TitleBar(Window window, float height, int padding) {
            this.window = window;
            this.height = height;

            // Total height of the container, including padding
            float totalHeight = height + 2*padding;

            // Fill up the entire top of the Window
            SetSize(0f, totalHeight);
            SetAnchor(AnchorType.TopLeft, FillType.FillHorizontal);

            // Add a background
            UEImage image = gameObject.AddComponent<UEImage>();
            image.color = Colors.darkGrey;

            // Add the name of the window
            Label label = new Label(window.name, (int) totalHeight - 5);
            Add(label);
            label.SetSize(0f, totalHeight);
            label.SetAnchor(AnchorType.TopLeft, FillType.FillHorizontal);

            // Create an area for holding the buttons
            buttonArea = new Area();
            Add(buttonArea);
            buttonArea.SetLayout(LayoutType.Horizontal);
            buttonArea.SetAnchor(AnchorType.TopRight);
            buttonArea.SetLayoutPadding(padding, padding, padding, padding);
            buttonArea.SetLayoutSpacing(padding);

            // The button for toggling fullscreen mode
            fullscreenButton = new Button("+", (int) height);
            fullscreenButton.AddLayoutElement();
            fullscreenButton.SetSize(height, height);
            fullscreenButton.SetAnchor(AnchorType.TopRight);
            fullscreenButton.AddListener(() => {
                ToggleFullscreen();
            });

            // The button to close the Window
            closeButton = new Button("x", (int) height);
            closeButton.AddLayoutElement();
            closeButton.SetSize(height, height);
            closeButton.SetAnchor(AnchorType.TopRight);
            closeButton.SetColorBlock(Colors.redColorBlock);
            closeButton.AddListener(() => {
                window.Hide();
            });

            buttonArea.Add(fullscreenButton);
            buttonArea.Add(closeButton);
        }

        /**
         * <summary>
         * Toggles fullscreen on the Window.
         * </summary>
         */
        private void ToggleFullscreen() {
            if (window.fullscreen == false) {
                window.BeginFullscreen();
            }
            else {
                window.EndFullscreen();
            }
        }

        /**
         * <summary>
         * Makes a Window fullscreen on double click.
         * </summary>
         */
        protected override void OnDoubleClick() {
            window.BeginFullscreen();
        }

        /**
         * <summary>
         * Restores a Window back from fullscreen when dragged with lmb.
         * </summary>
         */
        protected override void OnBeginDrag(Vector2 position) {
            // Only move when lmb is held
            if (Input.GetMouseButton(0) == false) {
                return;
            }

            window.HandleBeginDrag(position);
        }

        /**
         * <summary>
         * Restores a Window back from fullscreen when dragged with lmb.
         * </summary>
         */
        protected override void OnDrag(Vector2 position) {
            // Only move when lmb is held
            if (Input.GetMouseButton(0) == false) {
                return;
            }

            window.HandleMove(position);
        }
    }
}
