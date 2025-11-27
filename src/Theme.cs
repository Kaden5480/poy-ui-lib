using UnityEngine;
using UnityEngine.UI;

namespace UILib {
    /**
     * <summary>
     * Provides a way of modifying how UIs are themed.
     * </summary>
     */
    public class Theme {

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

#region Base Colors

        /**
         * <summary>
         * The foreground color (like on text).
         * </summary>
         */
        public Color foreground = new Color(1f, 1f, 1f);

        /**
         * <summary>
         * The background color (such as the
         * background for <see cref="Windows"/>).
         * </summary>
         */
        public Color background = new Color(0f, 0f, 0f);

        /**
         * <summary>
         * The accent color (such as the backgrounds
         * of the scroll bars for <see cref="ScrollView">
         * ScrollViews</see> and the title bar for
         * <see cref="Window">Windows</see>.
         * </summary>
         */
        public Color accent = new Color(22f/255f, 22f/255f, 22f/255f);

#endregion

#region Selectables

        /**
         * <summary>
         * The normal color for selectables (such as
         * the background of <see cref="Button">Buttons</see>).
         * </summary>
         */
        public Color selectNormal = new Color(50f/255f, 50f/255f, 50f/255f);

        /**
         * <summary>
         * The highlighted color for selectables (such
         * as the background of <see cref="Button">Buttons</see>
         * when hovered over).
         * </summary>
         */
        public Color selectHighlight = new Color(80f/255f, 80f/255f, 80f/255f);

        /**
         * <summary>
         * A secondary normal color for selectables.
         * This will be the normal color
         * of the handles for <see cref="Slider">Sliders</see>
         * and the highlight color of scroll bars of
         * <see cref="ScrollView">ScrollViews</see>.
         * </summary>
         */
        public Color selectAltNormal = new Color(134f/255f, 134f/255f, 134f/255f);

        /**
         * <summary>
         * A secondary highlight color for selectables.
         * This will be the highlight color
         * of the handles for <see cref="Slider">Sliders</see>
         * </summary>
         */
        public Color selectAltHighlight = new Color(164f/255f, 164f/255f, 164f/255f);

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
         * the close button on <see cref="Window">Windows</see>
         * </summary>
         */
        public Color importantNormal = new Color(166/255f, 89f/255f, 89f/255f);
        /**
         * <summary>
         * The secondary important color.
         * This is used as the highlighted background of
         * the close button on <see cref="Window">Windows</see>
         * </summary>
         */
        public Color importantHighlight = new Color(204f/255f, 51/255f, 51f/255f);

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
         * <see cref="TextField">TextFields</see>
         * as the underlying Unity InputFields
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
         * Note this only affects the normal notifications.
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
