using System.IO;
using System.Reflection;

using UnityEngine;

namespace UILib {
    /**
     * <summary>
     * A class holding resources used by UILib.
     * These resources are loaded from an `AssetBundle`.
     * </summary>
     */
    public static class Resources {
        private const string bundlePath = "res.uilib.bundle";
        private static AssetBundle bundle = null;

        /**
         * <summary>
         * The default Peaks of Yore font (Roman Antique).
         * </summary>
         */
        public static Font gameFont { get; private set; } = LoadAsset<Font>(
            "RomanAntique"
        );

        /**
         * <summary>
         * The default Peaks of Yore font (Roman Antique).
         * A scuffed variant which is used internally because input
         * fields are a mess.
         * </summary>
         */
        internal static Font gameFontScuffed { get; private set; } = LoadAsset<Font>(
            "RomanAntiqueScuffed"
        );

        /**
         * <summary>
         * The default Peaks of Yore menu click sound.
         * </summary>
         */
        internal static AudioClip gameMenuClick { get; private set; } = LoadAsset<AudioClip>(
            "MenuClick"
        );

        /**
         * <summary>
         * The default checkmark used by Peaks of Yore.
         * </summary>
         */
        public static Texture2D checkMark { get; private set; } = LoadAsset<Texture2D>(
            "CheckMark"
        );

        /**
         * <summary>
         * A texture of a circle.
         * </summary>
         */
        public static Texture2D circle { get; private set; } = LoadAsset<Texture2D>(
            "Circle"
        );

        /**
         * <summary>
         * A texture of a triangle.
         * </summary>
         */
        public static Texture2D triangle { get; private set; } = LoadAsset<Texture2D>(
            "Triangle"
        );

        /**
         * <summary>
         * The default notification sound used by UILib.
         * </summary>
         */
        public static AudioClip notification { get; private set; } = LoadAsset<AudioClip>(
            "Notification"
        );

        /**
         * <summary>
         * The default error notification sound used by UILib.
         * </summary>
         */
        public static AudioClip notificationError { get; private set; } = LoadAsset<AudioClip>(
            "NotificationError"
        );

        /**
         * <summary>
         * A material which renders an HSV rectangle.
         * </summary>
         */
        public static Material hsvRect { get; private set; } = LoadAsset<Material>(
            "HSVRect"
        );

        /**
         * <summary>
         * A material which renders a spectrum of all hues at maximum
         * saturation and value.
         * </summary>
         */
        public static Material hsvSpectrum { get; private set; } = LoadAsset<Material>(
            "HSVSpectrum"
        );

        /**
         * <summary>
         * A material which renders a specific HSV color
         * in a gradient of opacities.
         * </summary>
         */
        public static Material hsvOpacity { get; private set; } = LoadAsset<Material>(
            "HSVOpacity"
        );

        /**
         * <summary>
         * Loads an `AssetBundle`.
         *
         * If your asset bundle is embedded under `res/`, for example,
         * and has the name `uilib.bundle`, then the name should be `res.uilib.bundle`.
         * </summary>
         * <param name="assembly">The assembly to load the bundle from</param>
         * <param name="name">The name of the asset bundle to load</param>
         * <returns>The loaded asset bundle</returns>
         */
        public static AssetBundle LoadBundle(Assembly assembly, string name) {
            using (Stream stream = assembly.GetManifestResourceStream(
                $"{assembly.GetName().Name}.{name}"
            )) {
                return AssetBundle.LoadFromStream(stream);
            }
        }

        /**
         * <summary>
         * UILib specific bundle loading.
         * </summary>
         * <param name="name">The name of the asset to load</param>
         * <returns>The loaded asset</returns>
         */
        private static T LoadAsset<T>(string name) where T : UnityEngine.Object {
            if (bundle == null) {
                bundle = LoadBundle(Assembly.GetExecutingAssembly(), bundlePath);
            }

            return bundle.LoadAsset<T>(name);
        }
    }
}
