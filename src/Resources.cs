using System.IO;
using System.Reflection;

using UnityEngine;

namespace UILib {
    /**
     * <summary>
     * A class holding resources used by UILib.
     * These resources are loaded from an asset bundle.
     * </summary>
     */
    public static class Resources {
        private const string bundlePath = "uilib.bundle";
        private static AssetBundle bundle;

        // The font the game uses (Roman Antique)
        public static Font gameFont { get; private set; } = LoadFromBundle<Font>(
            "RomanAntique"
        );

        internal static Font gameFontScuffed { get; private set; } = LoadFromBundle<Font>(
            "RomanAntiqueScuffed"
        );

        // A checkmark texture for toggles
        public static Texture2D checkMark { get; private set; } = LoadFromBundle<Texture2D>(
            "CheckMark"
        );

        // A circle texture (used by Slider)
        public static Texture2D circle { get; private set; } = LoadFromBundle<Texture2D>(
            "Circle"
        );

        // A triangle texture (used by ResizeButton)
        public static Texture2D triangle { get; private set; } = LoadFromBundle<Texture2D>(
            "Triangle"
        );

        // Notification sounds
        // Best played with 0.8 volume
        public static AudioClip notification { get; private set; } = LoadFromBundle<AudioClip>(
            "Notification"
        );

        // Best played with 0.6 volume
        public static AudioClip notificationError { get; private set; } = LoadFromBundle<AudioClip>(
            "NotificationError"
        );

        /**
         * <summary>
         * Loads a file with the specified filename
         * into a byte array.
         *
         * The files are loaded from res/, so passing
         * a name of "image.png" will load res/image.png.
         * </summary>
         * <param name="name">The name of the file to load</param>
         * <returns>The file's bytes</returns>
         */
        internal static byte[] LoadBytes(string name) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string assemblyName = assembly.GetName().Name;

            using (Stream stream = assembly.GetManifestResourceStream(
                $"{assemblyName}.res.{name}"
            )) {
            using (MemoryStream mem = new MemoryStream()) {
                stream.CopyTo(mem);
                return mem.ToArray();
            }}
        }

        /**
         * <summary>
         * Loads an asset by name.
         * </summary>
         * <param name="name">The name of the asset to load</param>
         * <returns>The loaded asset</returns>
         */
        internal static T LoadFromBundle<T>(string name) where T : UnityEngine.Object {
            if (bundle == null) {
                byte[] bundleData = LoadBytes(bundlePath);
                bundle = AssetBundle.LoadFromMemory(bundleData);
            }

            return bundle.LoadAsset<T>(name);
        }
    }
}
