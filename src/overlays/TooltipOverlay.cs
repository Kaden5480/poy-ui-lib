using System.Collections;

using UnityEngine;

using UILib.Behaviours;
using UILib.Components;
using UILib.Layouts;

namespace UILib {
    /**
     * <summary>
     * An overlay which is used for displaying tooltips.
     * </summary>
     */
    internal class TooltipOverlay : UIObject {
        /**
         * <summary>
         * The canvas assigned to the tooltip overlay.
         * </summary>
         */
        internal Canvas canvas { get; private set; }

        // Fading in/out
        private float fadeTime = 0.25f;
        private CanvasGroup canvasGroup;
        private IEnumerator showRoutine;
        private IEnumerator hideRoutine;

        // Background
        private Image background;

        // Label with tooltip
        private Label tooltipLabel;

        // The UIObject which the tooltip is being shown for
        private UIObject current;

        /**
         * <summary>
         * Initializes the tooltip overlay.
         * </summary>
         */
        internal TooltipOverlay() {
            canvas = new Canvas();
            canvas.Add(this);

            canvasGroup = gameObject.AddComponent<CanvasGroup>();

            Area area = new Area(fillType: FillType.All);
            area.SetSize(200f, 0f);
            Add(area);

            background = new Image();
            background.SetFill(FillType.All);
            background.SetContentLayout(LayoutType.Vertical);
            background.SetContentPadding(10, 5);
            area.Add(background);

            tooltipLabel = new Label("", 20);
            tooltipLabel.SetSize(300f, 0f);
            tooltipLabel.SetFill(FillType.Vertical);
            background.Add(tooltipLabel);

            SetThisTheme(theme);

            canvasGroup.alpha = 0f;
        }

        /**
         * <summary>
         * Allows setting the theme of the tooltip overlay.
         * </summary>
         * <param name="theme">The theme to apply</param>
         */
        protected override void SetThisTheme(Theme theme) {
            background.SetColor(theme.background);
            tooltipLabel.SetColor(theme.foreground);
        }

        /**
         * <summary>
         * Routine for fading the tooltip in.
         * </summary>
         * <param name="obj">The object to fade in for</param>
         */
        internal IEnumerator ShowRoutine(UIObject obj) {
            // Wait for hiding to stop
            while (hideRoutine != null) {
                yield return null;
            }

            current = obj;

            // Use new text and position
            tooltipLabel.SetText(obj.tooltip);

            Vector2 position = obj.rectTransform.position;
            Vector2 pivot = obj.rectTransform.pivot;
            Vector2 size = obj.rectTransform.sizeDelta;

            // Move to the UIObject
            rectTransform.position = position
                + new Vector2(
                    0f, size.y*(pivot.y + rectTransform.pivot.y)
                );

            // Begin fading in
            float timer = 0f;
            while (timer < fadeTime) {
                canvasGroup.alpha = timer / fadeTime;
                timer += Time.deltaTime;

                yield return null;
            }

            // Finish
            showRoutine = null;
            yield break;
        }

        /**
         * <summary>
         * Routine for fading the tooltip out.
         * </summary>
         * <param name="obj">The object to fade out for</param>
         */
        internal IEnumerator HideRoutine(UIObject obj) {
            // Kill the show routine
            if (showRoutine != null) {
                Plugin.instance.StopCoroutine(showRoutine);
                showRoutine = null;
            }

            // Begin fading out from current opacity
            float timer = fadeTime * canvasGroup.alpha;
            while (timer > 0f) {
                canvasGroup.alpha = timer / fadeTime;
                timer -= Time.deltaTime;

                yield return null;
            }

            if (current == obj) {
                current = null;
            }

            canvasGroup.alpha = 0f;
            hideRoutine = null;

            yield break;
        }

        /**
         * <summary>
         * Shows the tooltip for a given UIObject.
         * </summary>
         * <param name="obj">The object to show the tooltip for</param>
         */
        internal void ShowTooltip(UIObject obj) {
            if (obj == null) {
                return;
            }

            // Start a new show routine
            if (showRoutine != null) {
                Plugin.instance.StopCoroutine(showRoutine);
                showRoutine = null;
            }

            showRoutine = ShowRoutine(obj);
            Plugin.instance.StartCoroutine(showRoutine);
        }

        /**
         * <summary>
         * Hides the tooltip for the given UIObject.
         * </summary>
         */
        internal void HideTooltip(UIObject obj) {
            if (current != null && obj != current) {
                return;
            }

            // Start a new hide routine
            if (hideRoutine != null) {
                Plugin.instance.StopCoroutine(hideRoutine);
                hideRoutine = null;
            }

            hideRoutine = HideRoutine(obj);
            Plugin.instance.StartCoroutine(hideRoutine);
        }
    }
}
