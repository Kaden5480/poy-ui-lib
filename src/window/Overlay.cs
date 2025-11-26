using UILib.Components;
using UILib.Layout;
using UILib.Patches;

namespace UILib {
    /**
     * <summary>
     * A primitive UIObject which stays in a
     * fixed position on the screen.
     * </summary>
     */
    public class Overlay : UIObject {
        // The canvas this overlay is attached to
        internal Canvas canvas;

        // Whether this overlay should pause the game
        // automatically when it's shown
        private bool autoPause;

        // This overlay's pause handle
        private PauseHandle pauseHandle;

        // This overlay's container
        protected Area container;

        /**
         * <summary>
         * Initializes an overlay.
         *
         * By default overlays will pause the game when shown
         * If you want to disable this behaviour see <see cref="SetPauseMode"/>
         * </summary>
         * <param name="width">The width of the overlay</param>
         * <param name="height">The height of the overlay</param>
         */
        public Overlay(float width, float height) {
            // Get a canvas to draw this overlay on
            canvas = new Canvas();
            canvas.Add(this);

            // Register this overlay
            UIRoot.Register(this);

            // Container for content
            container = new Area();
            container.SetFill(FillType.All);
            AddDirect(container);

            // Set to the middle of the screen by default
            SetAnchor(AnchorType.Middle);

            // Pause by default
            SetPauseMode(true);

            // Show by default
            Show();
        }

        /**
         * <summary>
         * Destroy this overlay and all children.
         * </summary>
         */
        public override void Destroy() {
            UIRoot.Unregister(this);
            base.Destroy();
        }

        /**
         * <summary>
         * Set whether this overlay should
         * automatically pause the game when it's shown.
         *
         * If this overlay has a <see cref="PauseHandle"/>
         * it will close it if you set `autoPause` to `false`.
         * </summary>
         * <param name="autoPause">Whether to pause the game automatically</param>
         */
        public void SetPauseMode(bool autoPause) {
            this.autoPause = autoPause;

            if (pauseHandle != null) {
                pauseHandle.Close();
                pauseHandle = null;
            }
        }

        /**
         * <summary>
         * When opening, get a PauseHandle
         * if this overlay should pause the game.
         * </summary>
         */
        public override void Show() {
            base.Show();

            if (pauseHandle != null) {
                pauseHandle.Close();
            }

            if (autoPause == true) {
                pauseHandle = new PauseHandle();
            }
        }

        /**
         * <summary>
         * When closing, close the PauseHandle
         * if one was requested.
         * </summary>
         */
        public override void Hide() {
            base.Hide();

            if (pauseHandle != null) {
                pauseHandle.Close();
                pauseHandle = null;
            }
        }
    }
}
