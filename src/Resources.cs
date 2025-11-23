using System.IO;
using System.Reflection;

using UnityEngine;

namespace UILib {
    public static class Resources {
        private const string bundlePath = "uilib.bundle";
        private static AssetBundle bundle;

        public static Font gameFont { get; private set; } = LoadFromBundle<Font>(
            "RomanAntique"
        );

        public static Texture2D checkMark { get; private set; } = LoadFromBundle<Texture2D>(
            "CheckMark"
        );

        public static Texture2D triangle { get; private set; } = LoadFromBundle<Texture2D>(
            "Triangle"
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
