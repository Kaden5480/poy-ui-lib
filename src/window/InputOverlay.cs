using System;
using System.Collections;

using UnityEngine;

using UILib.Behaviours;
using UILib.Components;
using UILib.Layouts;

namespace UILib {
    /**
     * <summary>
     * An overlay which pops up to allow the user
     * to enter a key.
     *
     * When working with UILib, you should really check
     * if <see cref="waitingForInput"/> is set before
     * doing any other input handling in your mod.
     * This prevents some weirdness where your mod
     * may trigger something while the input overlay
     * is waiting for an input.
     * </summary>
     */
    public class InputOverlay : UIObject {
        internal Canvas canvas;

        // The request coroutine
        private IEnumerator coroutine;

        // The duration for the cancel timer
        private float cancelDuration = 1f;

        // Show cancel progress
        private ProgressBar cancelProgress;

        /**
         * <summary>
         * Whether the input overlay
         * is currently waiting for a key.
         * </summary>
         */
        public static bool waitingForInput {
            get => UIRoot.inputOverlay.coroutine != null;
        }

        /**
         * <summary>
         * Initializes the input overlay.
         * </summary>
         */
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
            cancelProgress = new ProgressBar();
            cancelProgress.SetSize(200f, 40f);

            Label cancelLabel = new Label("Cancelling...", 22);
            cancelLabel.SetAnchor(AnchorType.Middle);
            cancelLabel.SetSize(200f, 40f);
            cancelProgress.Add(cancelLabel);

            cancelProgress.Hide();
            inputArea.Add(cancelProgress);

            // Hide by default
            Hide();
        }

        /**
         * <summary>
         * Gets the current key being pressed.
         * This is nasty, but at least it only runs once.
         * </summary>
         * <returns>The key that was pressed</returns>
         */
        private KeyCode GetCurrentKey() {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode))) {
                if (Input.GetKeyDown(key) == true) {
                    return key;
                }
            }

            return KeyCode.None;
        }

        /**
         * <summary>
         * The coroutine which handles an input request.
         * </summary>
         * <param name="ev">The event to invoke when a keycode is read</param>
         */
        private IEnumerator HandleRequest(KeyCodeEvent ev) {
            float cancelTimer = 0f;

            // Run until the cancel timer fills up
            while (cancelTimer < cancelDuration) {
                // If the user is holding "esc", keep incrementing
                // the cancel timer
                if (Input.GetKey(KeyCode.Escape) == true) {
                    cancelTimer += Time.deltaTime;
                }
                // Otherwise, if the user has released esc
                // but only pressed it for a short amount of time
                // take "esc" as the input
                else if (cancelTimer > 0 && cancelTimer < 0.06f){
                    ev.Invoke(KeyCode.Escape);
                    yield break;
                }
                // Otherwise, reset the timer
                // and check the current key
                else {
                    cancelTimer = 0f;

                    if (Event.current.type == EventType.KeyDown) {
                        ev.Invoke(GetCurrentKey());
                        yield break;
                    }
                }

                // If the timer is > 0, show its progress
                if (cancelTimer > 0) {
                    cancelProgress.Show();
                    cancelProgress.SetProgress(cancelTimer / cancelDuration);
                }
                // Or keep it hidden
                else {
                    cancelProgress.Hide();
                }

                yield return null;
            }

            // The input was cancelled
            ev.Invoke(KeyCode.None);

            yield break;
        }

        /**
         * <summary>
         * Request a key from the user.
         *
         * The event will invoke the listener with the KeyCode read.
         * If the request was cancelled by the user, KeyCode.None will
         * be used instead.
         * </summary>
         * <returns>An event to listen for when the request has finished</returns>
         */
        internal KeyCodeEvent Request() {
            // Can't request when the coroutine is already running
            if (coroutine != null) {
                return null;
            }

            KeyCodeEvent ev = new KeyCodeEvent();
            ev.AddListener((KeyCode key) => {
                coroutine = null;
                Hide();
            });

            Show();

            coroutine = HandleRequest(ev);
            Plugin.instance.StartCoroutine(coroutine);

            return ev;
        }
    }
}
