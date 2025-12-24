using UnityEngine;

using UILib;
using UILib.Components;
using UILib.Layouts;
using UILib.Notifications;
using UILib.Patches;

namespace UILibExamples {
    /**
     * <summary>
     * A simple example explaining locks.
     * </summary>
     */
    public class Locks : Example {
        private Lock @lock;
        private Overlay overlay;

        public Locks() {
            // By default, Overlays/Windows will be set
            // to auto pause the game when they're visible,
            // but sometimes you don't want this behaviour
            overlay = new Overlay(200f, 200f);

            // So you can disable it
            overlay.SetLockMode(LockMode.None);

            // Anchor this overlay so it appears on the left
            // of the screen, but in the middle vertically
            overlay.SetAnchor(AnchorType.MiddleLeft);

            // Add a background that fills the entire overlay
            Image background = new Image(Color.black);
            background.SetFill(FillType.All);
            overlay.Add(background);

            Label label = new Label("This overlay won't pause the game", 20);
            label.SetSize(200f, 40f);
            overlay.Add(label);

            // Assign a global shortcut
            Shortcut shortcut = new Shortcut(new[] { KeyCode.PageUp });
            shortcut.onTrigger.AddListener(() => {
                ManageLock();
            });
            UIRoot.AddShortcut(shortcut);
        }

        public void ManageLock() {
            // If you really want to deal with locks
            // manually for some reason, they pause the game
            // and free the cursor when you construct them
            if (@lock == null) {
                @lock = new Lock();
                Notifier.Notify("Locks", "Paused the game");

                // You can also specify custom lock modes to
                // lock certain things.
                // These also allow you to keep one lock, but update
                // its behaviour.
                // Any time you set a different lock mode, it
                // will immediately update with the LockHandler

                // This only pauses the game, but the cursor remains
                // fixed to the middle of the screen (unless something else
                // is controlling it)
                //@lock = new Lock(LockMode.Pause);

                // This prevents interacting with certain extra navigation menus
                // such as the InGameMenu
                //@lock = new Lock(LockMode.Navigation);

                // You can also combine multiple lock modes
                //@lock = new Lock(LockMode.Pause | LockMode.Navigation);


                // You can also add/remove specific modes from the current one
                //@lock.AddMode(LockMode.Pause);
                //@lock.RemoveMode(LockMode.Navigation);
            }
            // And you *must* call `Close` on them to let the LockHandler
            // revert any changes
            else {
                @lock.Close();
                @lock = null;

                Notifier.Notify("Locks", "Letting the game unpause");

                // The lock handler has a variety of states you can check
                Notifier.Notify(
                    "LockHandler State",
                    $"Currently pausing?: {LockHandler.isPaused}"
                    + $" Currently freeing cursor?: {LockHandler.isCursorFree}"
                    + $" Currently locking navigation?: {LockHandler.isNavigationLocked}"
                );

                // It also provides events for when certain locks are applied/released
                // such as LockHandler.onPause, LockHandler.onUnpause, LockHandler.onCursorFree, and so on
            }
        }

        public override void Toggle() {
            overlay.ToggleVisibility();
        }
    }
}
