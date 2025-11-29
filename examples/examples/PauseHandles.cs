using UnityEngine;

using UILib;
using UILib.Components;
using UILib.Layouts;
using UILib.Notifications;
using UILib.Patches;

namespace UILibExamples {
    /**
     * <summary>
     * A simple example explaining pause handles.
     * </summary>
     */
    public class PauseHandles : Example {
        private PauseHandle pauseHandle;
        private Overlay overlay;

        public PauseHandles() {
            // By default, Overlays/Windows will be set
            // to auto pause the game when they're visible,
            // but sometimes you don't want this behaviour
            overlay = new Overlay(200f, 200f);

            // Anchor this overlay so it appears on the left
            // of the screen, but in the middle vertically
            overlay.SetAnchor(AnchorType.MiddleLeft);

            // So you can disable it
            overlay.SetAutoPause(false);

            // Add a background that fills the entire overlay
            Image background = new Image(Color.black);
            background.SetFill(FillType.All);
            overlay.Add(background);

            Label label = new Label("This overlay won't pause the game", 20);
            label.SetSize(200f, 40f);
            overlay.Add(label);
        }

        public void Update() {
            if (Input.GetKeyDown(KeyCode.PageUp) == false) {
                return;
            }

            // If you really want to deal with pause handles
            // manually for some reason, they pause the game
            // when you construct them
            if (pauseHandle == null) {
                pauseHandle = new PauseHandle();
                Notifier.Notify("PauseHandles", "Paused the game");
            }
            // And you *must* call `Close` on them to let the game
            // unpause again
            else {
                pauseHandle.Close();
                pauseHandle = null;

                // Keep in mind that an active pause handle anywhere
                // will keep the game paused, even if it's from other mods
                Notifier.Notify("PauseHandles", "Letting the game unpause");

                // Also, you can check the pause handler's current state
                Notifier.Notify(
                    "PauseHandler State",
                    $"Currently pausing?: {PauseHandler.shouldPause}"
                );
            }
        }

        public override void Toggle() {
            overlay.ToggleVisibility();
        }
    }
}
