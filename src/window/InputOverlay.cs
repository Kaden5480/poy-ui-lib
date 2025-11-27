using UnityEngine;

using UILib.Components;
using UILib.Layout;

namespace UILib {
    /**
     * <summary>
     * An overlay which pops up to allow the user
     * to enter a key.
     * </summary>
     */
    internal class InputOverlay : UIObject {
        internal Canvas canvas;

        internal InputOverlay() {
            Theme theme = UIRoot.defaultTheme;

            canvas = new Canvas();
            canvas.Add(this);
            canvas.canvas.sortingOrder
                = UIRoot.inputOverlaySortingOrder;

            SetFill(FillType.All);

            Color bgColor = theme.background;
            bgColor.a = 0.97f;

            // The huge translucent background
            Image background = new Image(bgColor);
            background.SetFill(FillType.All);
            Add(background, false);

            // Smaller background
            Image inputBackground = new Image(theme.background);
            inputBackground.SetAnchor(AnchorType.Middle);
            inputBackground.SetSize(400f, 260f);
            Add(inputBackground);

            // Area for laying out content
            Area inputArea = new Area();
            inputArea.SetAnchor(AnchorType.Middle);
            inputArea.SetFill(FillType.All);
            inputArea.SetContentLayout(LayoutType.Vertical);
            inputArea.SetElementSpacing(40);
            inputBackground.Add(inputArea);

            // Input info
            Area inputInfo = new Area();
            inputInfo.SetFill(FillType.All);
            inputInfo.SetContentLayout(LayoutType.Vertical);
            inputInfo.SetElementSpacing(5);

            Label inputTitle = new Label("Press any key", 50);
            inputTitle.SetSize(300f, 50f);
            inputInfo.Add(inputTitle);

            Label smallText = new Label("(or hold \"esc\" to cancel)", 20);
            smallText.text.color = theme.selectHighlight;
            smallText.SetSize(300f, 20f);
            inputInfo.Add(smallText);

            inputArea.Add(inputInfo);

            // Cancel progress
            ProgressBar cancelProgress = new ProgressBar();
            cancelProgress.SetSize(200f, 40f);

            Label cancelLabel = new Label("Cancelling...", 22);
            cancelLabel.SetAnchor(AnchorType.Middle);
            cancelLabel.SetSize(200f, 40f);
            cancelProgress.Add(cancelLabel);

            cancelProgress.Hide();
            inputArea.Add(cancelProgress);
        }
    }
}
