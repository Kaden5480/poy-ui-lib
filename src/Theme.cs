using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UILib.Behaviours;

namespace UILib {
    /**
     * <summary>
     * Provides a way of modifying how UIs are themed.
     * </summary>
     */
    public class Theme : ICloneable {
        // Underlying current theme (not a copy)
        private static Theme current = fallbackTheme;

        // The fallback theme
        private static Theme fallbackTheme = new Theme();

        // Registered themes
        private static Dictionary<string, Theme> themes
            = new Dictionary<string, Theme>();

        /**
         * <summary>
         * The name of this theme.
         * </summary>
         */
        public string name { get; private set; } = "Fallback";

        /**
         * <summary>
         * Registers a new theme.
         *
         * If you wish to register a new theme, its name must be unique.
         *
         * You should register themes in `Awake`.
         *
         * You shouldn't use event listeners for
         * <see cref="UIRoot.onPreInit"/> or <see cref="UIRoot.onInit"/>
         * just register directly in `Awake`.
         * </summary>
         * <param name="theme">The theme to add</param>
         */
        public static void Register(Theme theme) {
            if (string.IsNullOrEmpty(theme.name) == true) {
                Plugin.LogError("Tried registering a theme without a name");
                return;
            }

            if (theme.name == fallbackTheme.name
                || themes.ContainsKey(theme.name) == true
            ) {
                Plugin.LogError($"Theme: {theme.name} has already been registered");
                return;
            }

            themes[theme.name] = theme;
        }

        /**
         * <summary>
         * Gets all registered themes.
         *
         * You can only rely on this being usable from
         * <see cref="UIRoot.onPreInit"/> onwards.
         *
         * Note:
         * These themes are not copies.
         * </summary>
         * <returns>The registered themes</returns>
         */
        public static Dictionary<string, Theme> GetThemes() {
            return themes;
        }

        /**
         * <summary>
         * Gets a copy of the current theme.
         *
         * You can only rely on this being usable from
         * <see cref="UIRoot.onPreInit"/> onwards.
         * </summary>
         * <returns>A copy of the current theme</returns>
         */
        public static Theme GetTheme() {
            return current.Copy();
        }

        /**
         * <summary>
         * Gets a <see cref="Register">registered</see> theme.
         *
         * You can only rely on this being usable from
         * <see cref="UIRoot.onPreInit"/> onwards.
         * </summary>
         * <param name="theme">The theme to get</param>
         * <returns>The theme if found, null otherwise</returns>
         */
        public static Theme GetTheme(string name) {
            if (themes.TryGetValue(name, out Theme theme) == false) {
                return null;
            }

            return theme;
        }

        /**
         * <summary>
         * Can only construct privately.
         * </summary>
         */
        private Theme() {}

        /**
         * <summary>
         * Constructs an instance of the fallback theme
         * with a custom name.
         *
         * Use this if you want to create a new theme
         * to register with UILib.
         * </summary>
         * <param name="name">The name to use</param>
         */
        public Theme(string name) {
            if (name != null) {
                name = name.Trim();
            }

            this.name = name;
        }

#region Internal

        /**
         * <summary>
         * Builds a theme with a provided hue.
         * </summary>
         * <param name="name">The name of the theme</param>
         * <param name="hue">The hue to build the theme with</param>
         * <returns>The theme which was built</returns>
         */
        private static Theme BuildWithHue(string name, float hue) {
            return new Theme(name) {
                background         = Colors.HSL(hue, 10f,  10f),
                foreground         = Colors.HSL(hue, 100f, 90f),
                accent             = Colors.HSL(hue, 10f,  15f),
                accentAlt          = Colors.HSL(hue, 10f,  20f),
                selectNormal       = Colors.HSL(hue, 20f,  40f),
                selectHighlight    = Colors.HSL(hue, 20f,  50f),
                selectAltNormal    = Colors.HSL(hue, 30f,  60f),
                selectAltHighlight = Colors.HSL(hue, 30f,  70f),
            };
        }

        /**
         * <summary>
         * Registers built-in themes.
         * </summary>
         */
        internal static void RegisterBuiltIn() {
            Register(new Theme("Peaks Dark"));

            Register(new Theme("Peaks Light") {
                foreground         = Colors.RGB(0f,   0f,   0f),
                background         = Colors.RGB(255f, 255f, 255f),
                accent             = Colors.RGB(230f, 230f, 230f),
                accentAlt          = Colors.RGB(210f, 210f, 210f),
                selectNormal       = Colors.RGB(180f, 180f, 180f),
                selectHighlight    = Colors.RGB(150f, 150f, 150f),
                selectAltNormal    = Colors.RGB(130f, 130f, 130f),
                selectAltHighlight = Colors.RGB(120f, 120f, 120f),
                importantNormal    = Colors.RGB(240f, 100f, 100f),
                importantHighlight = Colors.RGB(230f, 80f,  80f),
            });

            Register(BuildWithHue("UILib Red",       0f));
            Register(BuildWithHue("UILib Orange",    25f));
            Register(BuildWithHue("UILib Yellow",    62f));
            Register(BuildWithHue("UILib Green",     100f));
            Register(BuildWithHue("UILib Mint",      165f));
            Register(BuildWithHue("UILib Aqua",      190f));
            Register(BuildWithHue("UILib Blue",      215f));
            Register(BuildWithHue("UILib Purple",    250f));
            Register(BuildWithHue("UILib Pink",      300f));

            // Also set the default
            SetTheme(Config.selectedTheme.Value);
        }

        /**
         * <summary>
         * Sets the current default theme.
         * </summary>
         */
        internal static void SetTheme(string name) {
            current = GetTheme(name);
            if (current == null) {
                Plugin.LogError($"Unable to find theme: {name}, using fallback instead");
                current = fallbackTheme;
            }
        }

        /**
         * <summary>
         * Gets the current theme directly.
         * This doesn't make a copy.
         * </summary>
         */
        internal static Theme GetThemeUnsafe() {
            return current;
        }

        /**
         * <summary>
         * Provides an internal way of quickly
         * generating color blocks for this theme.
         * </summary>
         */
        internal ColorBlock MakeBlock(Color normal, Color highlight) {
            return new ColorBlock() {
                fadeDuration     = selectFadeTime,
                colorMultiplier  = 1f,
                normalColor      = normal,
                disabledColor    = normal,
                selectedColor    = normal,
                highlightedColor = highlight,
                pressedColor     = highlight,
            };
        }

        /**
         * <summary>
         * The color block used by things like
         * <see cref="Button">Buttons</see>.
         * </summary>
         */
        internal ColorBlock blockSelect {
            get => MakeBlock(selectNormal, selectHighlight);
        }

        /**
         * <summary>
         * The color block used by things like
         * the handles of
         * scroll bars of <see cref="ScrollView">
         * ScrollViews</see>.
         * </summary>
         */
        internal ColorBlock blockSelectAlt {
            get => MakeBlock(selectHighlight, selectAltNormal);
        }

        /**
         * <summary>
         * The color block used by things like
         * the handles of <see cref="Slider">
         * Sliders</see>.
         * </summary>
         */
        internal ColorBlock blockSelectLight {
            get => MakeBlock(selectAltNormal, selectAltHighlight);
        }

        /**
         * <summary>
         * The color block used by things like
         * the resize buttons in <see cref="Window>windows</see>.
         * </summary>
         */
        internal ColorBlock blockSelectDark {
            get => MakeBlock(accent, selectNormal);
        }

        /**
         * <summary>
         * The color block used by things of importance.
         * Such as the close button on <see cref="Window>Windows</see>.
         * </summary>
         */
        internal ColorBlock blockImportant {
            get => MakeBlock(importantNormal, importantHighlight);
        }

#endregion

        /**
         * <summary>
         * Supports making a clone of this theme.
         * You probably want to use <see cref="Copy"/>
         * instead though, since it also handles
         * converting to a <see cref="Theme"/>.
         * </summary>
         * <returns>The clone</returns>
         */
        public object Clone() {
            return MemberwiseClone();
        }

        /**
         * <summary>
         * Takes a copy of this theme.
         * Like <see cref="Clone"/> but also handles
         * converting to a <see cref="Theme"/> type.
         * </summary>
         * <returns>The copy</returns>
         */
        public Theme Copy() {
            return (Theme) Clone();
        }

        /**
         * <summary>
         * Takes a copy of this theme and applies a custom name.
         * </summary>
         * <returns>The copy with a custom name</returns>
         */
        public Theme Copy(string name) {
            Theme copy = Copy();
            copy.name = name;
            return copy;
        }

#region Base Colors

        /**
         * <summary>
         * The foreground color (like on text).
         * </summary>
         */
        public Color foreground = Colors.RGB(255f, 255f, 255f);

        /**
         * <summary>
         * The background color (such as the
         * background for <see cref="Window">Windows</see>).
         * </summary>
         */
        public Color background = Colors.RGB(0f, 0f, 0f);

        /**
         * <summary>
         * The accent color (such as the backgrounds
         * of the scroll bars for <see cref="Components.ScrollView">
         * ScrollViews</see> and the title bar for
         * <see cref="Window">Windows</see>.
         * </summary>
         */
        public Color accent = Colors.RGB(22f, 22f, 22f);

        /**
         * <summary>
         * The alternate accent color (such as the background
         * of the color picker window.
         * </summary>
         */
        public Color accentAlt = Colors.RGB(41f, 41f, 41f);

#endregion

#region Animations

        /**
         * <summary>
         * How long it should take for <see cref="UIObject">
         * UIObjects</see> to ease in.
         *
         * This only affects `UIObjects` with animations on.
         *
         * This applies whenever a `UIObject` is shown.
         * </summary>
         */
        public float easeInTime = 0f;

        /**
         * <summary>
         * How long it should take for <see cref="UIObject">
         * UIObjects</see> to ease out.
         *
         * This only affects `UIObjects` with animations on.
         *
         * This applies whenever a `UIObject` is hidden.
         * </summary>
         */
        public float easeOutTime = 0f;

        /**
         * <summary>
         * The curve to apply when <see cref="easeInTime">easing in</see>.
         * </summary>
         */
        public Func<float, float> easeInFunction = Curves.EaseInOutExp;

        /**
         * <summary>
         * The curve to apply when <see cref="easeOutTime">easing out</see>.
         * </summary>
         */
        public Func<float, float> easeOutFunction = Curves.EaseInOutExp;

#endregion

#region Overlays and Windows

        /**
         * <summary>
         * The default/maximum opacity to apply to <see cref="Overlay">
         * Overlays</see>.
         * Must be between 0-1 inclusive.
         * Note this affects all content on the overlay as well,
         * since this just uses a `CanvasGroup` applied to the overlay.
         * </summary>
         */
        public float overlayOpacity = 1f;

        /**
         * <summary>
         * The default/maximum opacity to apply to
         * the <see cref="InputOverlay"/>.
         * This will only affect the background of the
         * overlay, not all content on it.
         * Must be between 0-1 inclusive.
         * </summary>
         */
        public float inputOverlayOpacity = 0.96f;

        /**
         * <summary>
         * How long it should take for the
         * <see cref="InputOverlay"/> to fade in/out.
         * This applies whenever the overlay is shown/hidden.
         * </summary>
         */
        public float inputOverlayFadeTime = 0f;

        /**
         * <summary>
         * The default opacity to set on a <see cref="Window">
         * Window's</see> background.
         * Must be between 0-1 inclusive.
         * </summary>
         */
        public float windowOpacity = 1f;

        /**
         * <summary>
         * The default opacity to set on a <see cref="Window">
         * Window's</see> decoratons (title bar, scroll bar, etc.)
         * Must be between 0-1 inclusive.
         * </summary>
         */
        public float windowDecorationOpacity = 1f;

        /**
         * <summary>
         * How long it should take for the
         * <see cref="Window">Window's</see> decorations to fade in/out.
         * This applies whenever <see cref="Window.Enable"/> or <see cref="Window.Disable"/>
         * is called.
         * </summary>
         */
        public float windowDecorationFadeTime = 0.5f;

#endregion

#region Selectables

        /**
         * <summary>
         * The normal color for selectables (such as
         * the background of <see cref="Components.Button">Buttons</see>).
         * </summary>
         */
        public Color selectNormal = Colors.RGB(50f, 50f, 50f);

        /**
         * <summary>
         * The highlighted color for selectables (such
         * as the background of <see cref="Components.Button">Buttons</see>
         * when hovered over).
         * </summary>
         */
        public Color selectHighlight = Colors.RGB(80f, 80f, 80f);

        /**
         * <summary>
         * A secondary normal color for selectables.
         * This will be the normal color
         * of the handles for <see cref="Components.Slider">Sliders</see>
         * and the highlight color of scroll bars of
         * <see cref="Components.ScrollView">ScrollViews</see>.
         * </summary>
         */
        public Color selectAltNormal = Colors.RGB(134f, 134f, 134f);

        /**
         * <summary>
         * A secondary highlight color for selectables.
         * This will be the highlight color
         * of the handles for <see cref="Components.Slider">Sliders</see>..
         * </summary>
         */
        public Color selectAltHighlight = Colors.RGB(164f, 164f, 164f);

        /**
         * <summary>
         * How long it should take a selectable
         * to fade between its normal and highlight colors.
         * </summary>
         */
        public float selectFadeTime = 0.1f;

#endregion

#region Important

        /**
         * <summary>
         * The primary important color.
         * This is used as the initial background of
         * the close button on <see cref="Window">Windows</see>.
         * </summary>
         */
        public Color importantNormal = Colors.RGB(166f, 89f, 89f);
        /**
         * <summary>
         * The secondary important color.
         * This is used as the highlighted background of
         * the close button on <see cref="Window">Windows</see>.
         * </summary>
         */
        public Color importantHighlight = Colors.RGB(204f, 51f, 51f);

#endregion

#region Fonts

        /**
         * <summary>
         * The default font to be used.
         * </summary>
         */
        public Font font = Resources.gameFont;

        /**
         * <summary>
         * The secondary font to be used.
         * This exists specifically to deal with
         * <see cref="Components.TextField">TextFields</see>
         * as the underlying Unity `InputFields`
         * can process some fonts incorrectly.
         * </summary>
         */
        public Font fontAlt = Resources.gameFontAlt;

        /**
         * <summary>
         * The line spacing to use on this font.
         * </summary>
         */
        public float fontLineSpacing = 3f;

        /**
         * <summary>
         * The value to scale fonts by.
         *
         * This could be useful if you have already made
         * a large UI and want to swap out the font,
         * and the font sizes you originally chose are too small/large
         * with the new font.
         *
         * Here you can just scale the font by a value instead
         * of going through every place you specified a font size.
         * </summary>
         */
        public float fontScaler = 1f;

        /**
         * <summary>
         * Like <see cref="fontScaler"/> but specific to
         * <see cref="Components.TextField">TextFields</see>
         * due to the reasons explained in <see cref="fontAlt"/>.
         * </summary>
         */
        public float fontScalerAlt = 2f;

#endregion

#region Notifications

        /**
         * <summary>
         * The maximum length of a notification's message before
         * it starts getting replaced with ellipses.
         * </summary>
         */
        public int notificationMaxMessage = 256;

        /**
         * <summary>
         * The maximum length of a notification's title before
         * it starts getting replaced with ellipses.
         * </summary>
         */
        public int notificationMaxTitle = 48;

        /**
         * <summary>
         * The default notification sound to use.
         * </summary>
         */
        public AudioClip notification = Resources.notification;

        /**
         * <summary>
         * The volume to play the default notification with.
         * </summary>
         */
        public float notificationVolume = 0.6f;

        /**
         * <summary>
         * The error notification sound to use.
         * </summary>
         */
        public AudioClip notificationError = Resources.notificationError;

        /**
         * <summary>
         * The volume to play the error notification with.
         * </summary>
         */
        public float notificationErrorVolume = 0.4f;

        /**
         * <summary>
         * The opacity to apply to notifications.
         * </summary>
         */
        public float notificationOpacity = 1f;

        /**
         * <summary>
         * How long the notification should stay on screen before fading.
         * Note that error notifications ignore this and will
         * always stay on the screen until dismissed by the user.
         * </summary>
         */
        public float notificationWaitTime = 3f;

        /**
         * <summary>
         * How long it should take for a notification
         * to fade out.
         * </summary>
         */
        public float notificationFadeTime = 1f;


        /**
         * <summary>
         * The sound to play in place of the game's built-in click sound.
         * </summary>
         */
        public AudioClip navigationSound = Resources.gameMenuClick;

        /**
         * <summary>
         * The volume to play the navigation sound with.
         * </summary>
         */
        public float navigationSoundVolume = 0.20f;

#endregion
    }
}
