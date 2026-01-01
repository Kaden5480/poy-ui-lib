using UILib;
using UILib.Animations;
using UILib.Components;
using UILib.Layouts;
using UnityEngine;

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

        private const float minOffset = 0f;
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
            Ease offsetEase = overlay.gameObject.AddComponent<Ease>();

            // Make the change take 1 second
            offsetEase.SetDuration(1f);

            // Ease between these two offsets
            offsetEase.SetValues(minOffset, maxOffset);

            // On each iteration, apply an offset
            offsetEase.onEase.AddListener((float offset) => {
                overlay.SetOffset(20f + offset, offset / 2);
            });

            // Add to the ease group
            easeGroup.Add(offsetEase);

            // Add an ease for controlling rotation
            Ease rotationEase = overlay.gameObject.AddComponent<Ease>();

            // The duration could be shorter/longer, it would still work
            // Ease groups only finish once all eases have finished
            rotationEase.SetDuration(1f);
            rotationEase.SetValues(0f, 360f);
            rotationEase.onEase.AddListener((float rotation) => {
                overlay.gameObject.transform.rotation = Quaternion.Euler(
                    0f, 0f, rotation
                );
            });
            easeGroup.Add(rotationEase);

            // Force the ease group to the start
            easeGroup.EaseOut(true);
        }

        public override void Toggle() {
            overlay.ToggleVisibility();
        }
    }
}
