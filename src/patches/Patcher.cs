using System;

using HarmonyLib;
using UnityEngine.SceneManagement;

namespace UILib.Patches {
    internal static class Patcher {
        private static Logger logger = new Logger(typeof(Patcher));

        /**
         * <summary>
         * Applies a patch and logs a completion message.
         * </summary>
         * <param name="patch">The patch to apply</param>
         */
        private static void Patch(Type patch) {
            Harmony.CreateAndPatchAll(patch);
            logger.LogDebug($"Applied patch: {patch}");
        }

        /**
         * <summary>
         * Applies patches.
         * </summary>
         */
        internal static void Awake() {
            Patch(typeof(Audio));
            Patch(typeof(InteractFixes));
            Patch(typeof(FadeFix));
            Patch(typeof(MenuFix));
            Patch(typeof(PauseFixes));
            Patch(typeof(SceneLoads));

            SceneLoads.onUnload.AddListener((Scene scene) => {
                PauseFixes.CloseHandles();
            });
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
            SceneLoads.OnSceneLoaded(scene);
        }

        /**
         * <summary>
         * Forwards a scene unload to patches that need it.
         * </summary>
         * <param name="scene">The scene which unloaded</param>
         */
        internal static void OnSceneUnloaded(Scene scene) {
            SceneLoads.OnSceneUnloaded(scene);
            Cache.Clear();
        }

        /**
         * <summary>
         * Runs patches that need to run each frame.
         * </summary>
         */
        internal static void Update() {
            UI.InputFieldFix.Update();
        }
    }
}
