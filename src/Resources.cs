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
        private const string bundlePath = "uilib.bundle";
        private static AssetBundle bundle;

        /**
         * <summary>
         * The default Peaks of Yore font (Roman Antique).
         * </summary>
         */
        public static Font gameFont { get; private set; } = LoadFromBundle<Font>(
            "RomanAntique"
        );

        /**
         * <summary>
         * The default Peaks of Yore font (Roman Antique).
         * A scuffed variant which is used internally because input
         * fields are a mess.
         * </summary>
         */
        internal static Font gameFontScuffed { get; private set; } = LoadFromBundle<Font>(
            "RomanAntiqueScuffed"
        );

        /**
         * <summary>
         * The default checkmark used by Peaks of Yore.
         * </summary>
         */
        public static Texture2D checkMark { get; private set; } = LoadFromBundle<Texture2D>(
            "CheckMark"
        );

        /**
         * <summary>
         * A texture of a circle.
         * </summary>
         */
        public static Texture2D circle { get; private set; } = LoadFromBundle<Texture2D>(
            "Circle"
        );

        /**
         * <summary>
         * A texture of a triangle.
         * </summary>
         */
        public static Texture2D triangle { get; private set; } = LoadFromBundle<Texture2D>(
            "Triangle"
        );

        /**
         * <summary>
         * The default notification sound used by UILib.
         * </summary>
         */
        public static AudioClip notification { get; private set; } = LoadFromBundle<AudioClip>(
            "Notification"
        );

        /**
         * <summary>
         * The default error notification sound used by UILib.
         * </summary>
         */
        public static AudioClip notificationError { get; private set; } = LoadFromBundle<AudioClip>(
            "NotificationError"
        );

        /**
         * <summary>
         * A material which renders an HSV rectangle.
         * </summary>
         */
        public static Material hsvRect { get; private set; } = LoadFromBundle<Material>(
            "HSVRect"
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
