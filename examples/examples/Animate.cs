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
            // for this example, use a custom fade time instead
            theme.overlayFadeTime = 0.6f;
            theme.overlayFadeFunction = Curves.EaseInExp;

            overlay = new Overlay(240f, 240f);
            overlay.SetTheme(theme);
            overlay.SetAnchor(AnchorType.MiddleLeft);

            Image background = new Image(theme.background);
            background.SetFill(FillType.All);
            background.SetContentLayout(LayoutType.Vertical);
            background.SetContentPadding(10);
            background.SetElementSpacing(10f);
            overlay.Add(background);

            // If you only care about apply animations when a UIObject is shown/hidden,
            // you can add any `BaseEase` like so
            Ease showHideEase = overlay.AddEase<Ease>();
            // 1 second
            showHideEase.SetDuration(1f);
            // Ease in/out exponentially
            showHideEase.SetEaseFunction(Curves.EaseInOutExp);
            // Ease in from -200 up to 20
            showHideEase.SetValues(-200f, 20f);

            // Apply a horizontal offset based upon the above parameters
            showHideEase.onEase.AddListener((float value) => {
                overlay.SetOffset(value, 0f);
            });

            // There's no need to run `showHideEase` manually, as it's controlled
            // when the UIObject is shown/hidden

            //
            // If you want to make custom animations that run whenever, see
            // the example below
            //

            // Add an ease group to control the animations
            // Something to note is you don't *have* to use an `EaseGroup`,
            // you can use `BaseEase` behaviours directly for very simple use
            // cases
            easeGroup = overlay.gameObject.AddComponent<EaseGroup>();

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

            // Add an ease behaviour to change the offset
            Ease offsetEase = overlay.gameObject.AddComponent<Ease>();

            // Make the change take 1 second
            offsetEase.SetDuration(1f);

            // Use a custom ease function
            offsetEase.SetEaseFunction(Curves.EaseInSine);

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
            rotationEase.SetEaseFunction(Curves.EaseInOutQuad);
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
