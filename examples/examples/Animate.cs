using UILib;
using UILib.Animations;
using UILib.Components;
using UILib.Layouts;

using UIButton = UILib.Components.Button;

namespace UILibExamples {
    /**
     * <summary>
     * An example demonstrating animations.
     * </summary>
     */
    public class Animate : Example {
        private Overlay overlay;
        private EaseGroup easeGroup;

        private const float minOffset = 20f;
        private const float maxOffset = 300f;

        public Animate() {
            Theme theme = Theme.GetTheme();
            // Ignore the theme's fade settings
            // for this example
            theme.overlayFadeTime = 0f;

            overlay = new Overlay(240f, 240f);
            overlay.SetTheme(theme);
            overlay.SetAnchor(AnchorType.MiddleLeft);

            Image background = new Image(theme.background);
            background.SetFill(FillType.All);
            background.SetContentLayout(LayoutType.Vertical);
            background.SetContentPadding(10);
            background.SetElementSpacing(10f);
            overlay.Add(background);

            // Add buttons to control easing
            UIButton easeInButton = new UIButton("Ease in", 20);
            easeInButton.SetSize(160f, 30f);
            easeInButton.onClick.AddListener(() => {
                easeGroup.EaseIn();
            });
            background.Add(easeInButton);

            UIButton easeOutButton = new UIButton("Ease out", 20);
            easeOutButton.SetSize(160f, 30f);
            easeOutButton.onClick.AddListener(() => {
                easeGroup.EaseOut();
            });
            background.Add(easeOutButton);

            // Add an ease group to control the animations
            easeGroup = overlay.gameObject.AddComponent<EaseGroup>();

            // Add an ease behaviour to change the offset
            Ease ease = overlay.gameObject.AddComponent<Ease>();

            // Make the change take 1 second
            ease.SetDuration(1f);

            // Ease between these two offsets
            ease.SetValues(minOffset, maxOffset);

            // On each iteration, apply an offset
            ease.onEase.AddListener((float offset) => {
                overlay.SetOffset(offset, 0f);
            });

            // Add to the ease group
            easeGroup.Add(ease);

            // Force the ease group to the start
            easeGroup.EaseOut(true);
        }

        public override void Toggle() {
            overlay.ToggleVisibility();
        }
    }
}
