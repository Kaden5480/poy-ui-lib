using HarmonyLib;
using UnityEngine.SceneManagement;

namespace UILib.Patches {
    internal class Patcher {
        /**
         * <summary>
         * Applies patches.
         * </summary>
         */
        internal static void Awake() {
            Harmony.CreateAndPatchAll(typeof(MenuFix));
        }

        /**
         * <summary>
         * Forwards a scene load to patches that need it.
         * </summary>
         * <param name="scene">The scene which loaded</param>
         * <param name="mode">The mode the scene loaded with</param>
         */
        internal static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            Cache.FindObjects();
        }

        /**
         * <summary>
         * Forwards a scene unload to patches that need it.
         * </summary>
         * <param name="scene">The scene which unloaded</param>
         */
        internal static void OnSceneUnloaded(Scene scene) {
            Cache.Clear();
        }
    }
}
