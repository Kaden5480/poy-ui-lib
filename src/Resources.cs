using System.IO;
using System.Reflection;

using UnityEngine;

namespace UILib {
    public static class Resources {
        public static Font gameFont { get; private set; } = LoadFontFromBundle(
            "romanantique.bundle", "RomanAntique"
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
         * Loads a font by file name.
         * </summary>
         * <param name="name">The name of the font file to load</param>
         * <returns>The loaded font</returns>
         */
        internal static Font LoadFontFromBundle(string bundleName, string fontName) {
            byte[] bundleData = LoadBytes(bundleName);
            AssetBundle assetBundle = AssetBundle.LoadFromMemory(bundleData);
            return assetBundle.LoadAsset<Font>(fontName);
        }
    }
}
