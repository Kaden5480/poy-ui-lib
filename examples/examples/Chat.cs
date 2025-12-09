using System;

using UnityEngine;

using UILib;
using UILib.Components;
using UILib.Layouts;
using UILib.Notifications;
using UIButton = UILib.Components.Button;
using RetainMode = UILib.Components.TextField.RetainMode;

namespace UILibExamples {
    /**
     * <summary>
     * A pretty bad example of a primitive chat.
     * I suggest looking at the source code of:
     * https://github.com/Kaden5480/poy-in-game-logs/
     *
     * It handles things more cleanly.
     * </summary>
     */
    public class Chat : Example {
        private Window window;

        // Message history
        private QueueArea history;

        // Input box
        private TextField inputBox;

        private void AddMessage(string message) {
            Label label = new Label($"{DateTime.Now} | {message}", 20);
            label.SetAnchor(AnchorType.BottomMiddle);
            label.SetFill(FillType.Horizontal);
            label.SetSize(0f, 30f);
            history.Add(label);

            // Also clear the input
            inputBox.SetValue("");
        }

        public Chat() {
            // Get the default theme
            Theme theme = new Theme();

            window = new Window("Chat", 800f, 600f);

#region Message History

            // The message history
            history = new QueueArea(30);
            history.SetContentLayout(LayoutType.Vertical);
            history.SetAnchor(AnchorType.BottomLeft);

            // Align messages to the left
            history.SetElementAlignment(TextAnchor.LowerLeft);

            // Add some padding around the messages so they don't
            // appear really close to the window's border
            // or the input box
            // A bigger padding is needed on the bottom to
            // leave space for the input box
            history.SetContentPadding(10, 10, 10, 110);

            window.Add(history);

            Image image = new Image(Colors.RGB(255, 0, 0));
            image.SetFill(FillType.All);
            history.Add(image);

            // Tell the window's scrollview that
            // it should scroll over the message
            // history instead
            window.scrollView.SetContent(history);

#endregion

#region Input Area

            // Add another area for the input box
            Area inputArea = new Area();
            inputArea.SetAnchor(AnchorType.BottomMiddle);
            inputArea.SetFill(FillType.Horizontal);

            // Assign a height of 100 and leave
            // room for vertical scrollbar
            inputArea.SetSize(-20f, 100f);
            inputArea.SetOffset(-10f, 0f);

            // Add a background to it
            Image inputAreaBg = new Image(theme.accent);
            inputAreaBg.SetFill(FillType.All);
            inputArea.Add(inputAreaBg);

            // Add directly to the window, instead of to the content
            // This keeps it fixed in place so it doesn't scroll
            window.AddDirect(inputArea);

#endregion

#region Controls

            Area controlArea = new Area();
            controlArea.SetContentLayout(LayoutType.Vertical);
            controlArea.SetElementSpacing(10);
            inputArea.Add(controlArea);

            // Add a text field
            inputBox = new TextField("Enter message", 20);
            inputBox.onValidSubmit.AddListener(AddMessage);

            // Retain focus and the user input
            inputBox.SetRetainFocus(true);
            inputBox.SetRetainMode(
                RetainMode.CancelEscape
                | RetainMode.CancelClick
            );

            inputBox.SetSize(200f, 30f);
            controlArea.Add(inputBox);

            UIButton sendButton = new UIButton("Send", 20);
            sendButton.SetSize(100f, 30f);
            sendButton.onClick.AddListener(() => {
                AddMessage(inputBox.userInput);
            });
            controlArea.Add(sendButton);

#endregion

        }

        public override void Toggle() {
            window.ToggleVisibility();
        }
    }
}
