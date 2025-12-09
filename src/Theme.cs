using System;

using UnityEngine;
using UnityEngine.UI;

namespace UILib {
    /**
     * <summary>
     * Provides a way of modifying how UIs are themed.
     * </summary>
     */
    public class Theme : ICloneable {

#region Internal

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
         * of <see cref="ColorPicker.ColorPickerWindow"/>).
         * </summary>
         */
        public Color accentAlt = Colors.RGB(41f, 41f, 41f);

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
         * How long it should take for <see cref="Overlay">
         * Overlays</see> to fade in/out.
         * This applies whenever an overlay is shown/hidden.
         * </summary>
         */
        public float overlayFadeTime = 0f;

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
        public Font fontAlt = Resources.gameFontScuffed;

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

#endregion

#region Notifications

        /**
         * <summary>
         * The maximum length of a notification's message.
         * </summary>
         */
        public int notificationMaxMessage = 256;

        /**
         * <summary>
         * The maximum length of a notification's title.
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

#endregion

    }
}
