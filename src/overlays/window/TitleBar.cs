using UnityEngine;
using UEImage = UnityEngine.UI.Image;

using UILib.Components;
using UILib.Layouts;
using UIButton = UILib.Components.Button;

namespace UILib {
    /**
     * <summary>
     * The title bar which is placed at
     * the top of all <see cref="Window">Windows</see>.
     * </summary>
     */
    public class TitleBar : UIComponent {
        // The background
        private UEImage background;

        // An area for holding the buttons
        private Area buttonArea;

        /**
         * <summary>
         * This title bar's fullscreen button.
         * </summary>
         */
        public UIButton fullscreenButton { get; private set; }

        /**
         * <summary>
         * This title bar's close button.
         * </summary>
         */
        public UIButton closeButton { get; private set; }

        // The height of this TitleBar
        private float barHeight;

        // The Window this TitleBar is attached to
        private Window window;

        // The title bar name
        private Label windowName;

        /**
         * <summary>
         * Initializes a TitleBar.
         * </summary>
         * <param name="window">The Window this TitleBar  is attached to</param>
         * <param name="barHeight">The height to use for this TitleBar</param>
         * <param name="padding">The padding to use</param>
         */
        internal TitleBar(Window window, float barHeight, int padding) {
            this.window = window;
            this.barHeight = barHeight;

            // Total height of the container, including padding
            float totalHeight = barHeight + 2*padding;

            // Fill up the entire top of the Window
            SetSize(0f, totalHeight);
            SetAnchor(AnchorType.TopLeft);
            SetFill(FillType.Horizontal);

            // Add a background
            background = gameObject.AddComponent<UEImage>();

            // Add the name of the window
            windowName = new Label(window.name, (int) totalHeight - 5);
            Add(windowName);
            windowName.SetSize(0f, totalHeight);
            windowName.SetAnchor(AnchorType.TopLeft);
            windowName.SetFill(FillType.Horizontal);

            // Create an area for holding the buttons
            buttonArea = new Area();
            Add(buttonArea);
            buttonArea.SetContentLayout(LayoutType.Horizontal);
            buttonArea.SetAnchor(AnchorType.TopRight);
            buttonArea.SetContentPadding(padding, padding, padding, padding);
            buttonArea.SetElementSpacing(padding);

            // The button for toggling fullscreen mode
            fullscreenButton = new UIButton("+", (int) barHeight);
            fullscreenButton.SetSize(barHeight, barHeight);
            fullscreenButton.SetAnchor(AnchorType.TopRight);
            fullscreenButton.onClick.AddListener(() => {
                ToggleFullscreen();
            });

            // The button to close the Window
            closeButton = new UIButton("x", (int) barHeight);
            closeButton.SetSize(barHeight, barHeight);
            closeButton.SetAnchor(AnchorType.TopRight);
            closeButton.onClick.AddListener(() => {
                window.Hide();
            });

            buttonArea.Add(fullscreenButton);
            buttonArea.Add(closeButton);

            SetThisTheme(theme);
        }

        /**
         * <summary>
         * Sets the name displayed in the title bar.
         * </summary>
         * <param name="name">The name to display</param>
         */
        internal void SetName(string name) {
            windowName.SetText(name);
        }

        /**
         * <summary>
         * Allows setting the theme of this title bar.
         *
         * This handles setting the theme specifically for this component,
         * not its children. It's protected to allow overriding if you
         * were to create a subclass.
         *
         * In most cases, you'd probably want to use
         * <see cref="UIObject.SetTheme"/> instead.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {
            background.color = theme.accent;
            closeButton.button.colors = theme.blockImportant;
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
