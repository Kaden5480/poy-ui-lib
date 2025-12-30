using System;
using System.Collections;

using UnityEngine;

using UILib.Events;
using UILib.Components;
using UILib.Layouts;
using UILib.Patches;

namespace UILib {
    /**
     * <summary>
     * An overlay which pops up to allow the user
     * to enter a key.
     * </summary>
     */
    internal class InputOverlay : Overlay {
        // The large background
        private Image background;

        // Smaller background
        private Image inputBackground;

        // The request coroutine
        private IEnumerator coroutine;

        // The duration for the cancel timer
        private const float quitDuration = 1f;

        // How long a press can be
        // until it's detected as a cancel/unbind
        private const float readTime = 0.2f;

        // Show cancel progress
        private ProgressBar quitProgress;
        private Label quitLabel;

        // The small text
        private Label smallText;

        /**
         * <summary>
         * Whether the input overlay
         * is currently waiting for a key.
         * </summary>
         */
        internal static bool waitingForInput {
            get => UIRoot.inputOverlay.coroutine != null;
        }

        /**
         * <summary>
         * Initializes the input overlay.
         * </summary>
         */
        internal InputOverlay() : base(0f, 0f) {
            // Disable sorting
            SetSortingMode(Overlay.SortingMode.Static);

            SetFill(FillType.All);

            // The huge translucent background
            background = new Image(theme.background);
            background.SetFill(FillType.All);
            Add(background);

            // Smaller background
            inputBackground = new Image(theme.background);
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

            smallText = new Label("(hold \"esc\" to cancel, or \"backspace\" to unbind)", 20);
            smallText.SetSize(400f, 20f);
            inputInfo.Add(smallText);

            inputArea.Add(inputInfo);

            // Cancel progress
            quitProgress = new ProgressBar();
            quitProgress.SetSize(200f, 40f);

            quitLabel = new Label("Cancelling...", 22);
            quitLabel.SetAnchor(AnchorType.Middle);
            quitLabel.SetSize(200f, 40f);
            quitProgress.Add(quitLabel);

            quitProgress.Hide();
            inputArea.Add(quitProgress);

            SetThisTheme(theme);
        }

        /**
         * <summary>
         * Allows setting the theme of the input overlay.
         *
         * This handles setting the theme specifically for this object,
         * not its children. It's protected to allow overriding if you
         * were to create a subclass.
         *
         * In most cases, you'd probably want to use
         * <see cref="UIObject.SetTheme"/> instead.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {
            base.SetThisTheme(theme);

            if (inputBackground == null) {
                return;
            }

            inputBackground.SetColor(theme.background);

            // Only apply opacity to background
            fade.SetOpacities(max: 1f);
            canvasGroup.alpha = 1f;

            // Use a different fade time
            fade.SetFadeTime(theme.inputOverlayFadeTime);

            Color bg = theme.background;
            bg.a = theme.inputOverlayOpacity;
            background.SetColor(bg);

            smallText.text.color = theme.selectHighlight;
        }

        /**
         * <summary>
         * Checks whether the provided KeyCode should be ignored.
         * </summary>
         * <param name="key">The key to check</param>
         * <returns>Whether it should be ignored</returns>
         */
        private bool ShouldIgnore(KeyCode key) {
            switch (key) {
                case KeyCode.LeftCommand:
                case KeyCode.RightCommand:
                    return true;
            }

            return false;
        }

        /**
         * <summary>
         * Gets the current key or mouse button being pressed.
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

            for (int i = 0; i <= 6; i++) {
                if (Input.GetMouseButton(i) == true) {
                    return (KeyCode) Enum.Parse(typeof(KeyCode), $"Mouse{i}");
                }
            }

            return KeyCode.None;
        }

        /**
         * <summary>
         * Unity's event system is stupid and doesn't handle
         * other mouse buttons properly, so check all
         * of them here.
         * </summary>
         * <returns>Whether a mouse button was pressed</returns>
         */
        private bool IsMouseDown() {
            for (int i = 0; i <= 6; i++) {
                if (Input.GetMouseButton(i) == true) {
                    return true;
                }
            }

            return false;
        }

        /**
         * <summary>
         * The coroutine which handles an input request.
         * </summary>
         * <param name="ev">The event to invoke when a keycode is read</param>
         */
        private IEnumerator HandleRequest(ValueEvent<bool, KeyCode> ev) {
            // Wait a little bit before reading inputs
            yield return new WaitForSeconds(0.02f);

            float quitTimer = 0f;
            bool cancelling = false;

            // Run until the cancel timer fills up
            while (quitTimer < quitDuration) {
                // If the user is holding "esc", keep incrementing
                // the cancel timer
                if (Input.GetKey(KeyCode.Escape) == true) {
                    cancelling = true;
                    quitTimer += Time.deltaTime;
                }
                // If the user is holding "backspace", keep incrementing
                // the unbind timer
                else if (Input.GetKey(KeyCode.Backspace) == true) {
                    cancelling = false;
                    quitTimer += Time.deltaTime;
                }
                // Otherwise, if the user has released esc
                // but only pressed it for a short amount of time
                // take "esc" as the input
                else if (quitTimer > 0 && quitTimer < readTime){
                    if (cancelling == true) {
                        ev.Invoke(false, KeyCode.Escape);
                    }
                    else {
                        ev.Invoke(false, KeyCode.Backspace);
                    }
                    yield break;
                }

                // Otherwise, reset the timer
                // and check the current key
                else {
                    quitTimer = 0f;
                    cancelling = false;

                    if (Event.current.type == EventType.KeyDown
                        || IsMouseDown() == true
                    ) {
                        KeyCode current = GetCurrentKey();
                        if (ShouldIgnore(current) == false) {
                            ev.Invoke(false, current);
                            yield break;
                        }
                    }
                }

                // If the timer is >= readTime, show its progress
                if (quitTimer >= readTime) {
                    quitProgress.Show();

                    if (cancelling == true) {
                        quitLabel.SetText("Cancelling...");
                    }
                    else {
                        quitLabel.SetText("Unbinding...");
                    }

                    quitProgress.SetProgress(quitTimer / quitDuration);
                }
                // Or keep it hidden
                else {
                    quitProgress.Hide();
                }

                yield return null;
            }

            // The input was cancelled
            ev.Invoke(cancelling, KeyCode.None);

            yield break;
        }

        /**
         * <summary>
         * Request a key or mouse button from the user.
         *
         * The event will invoke the listener with the KeyCode read.
         *
         * If the input was cancelled, `true, KeyCode.None` will be returned,
         * otherwise `false, KeyCode.None` will return to indicate an unbind.
         *
         * If the input was read successfully, `false, <keycode>` will be returned.
         * </summary>
         * <param name="theme">The theme to use</param>
         * <returns>An event to listen for when the request has finished</returns>
         */
        internal ValueEvent<bool, KeyCode> Request(Theme theme) {
            // Can't request when the coroutine is already running
            if (coroutine != null) {
                return null;
            }

            ValueEvent<bool, KeyCode> ev = new ValueEvent<bool, KeyCode>();
            ev.AddListener(delegate {
                coroutine = null;
                Audio.PlayNavigation(theme);
                Hide();
            });

            SetTheme(theme);

            Show();

            coroutine = HandleRequest(ev);
            Plugin.instance.StartCoroutine(coroutine);

            return ev;
        }
    }
}
