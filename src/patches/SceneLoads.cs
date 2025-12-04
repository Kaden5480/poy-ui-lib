using HarmonyLib;
using UnityEngine.SceneManagement;

using UILib.Behaviours;

namespace UILib.Patches {
    /**
     * <summary>
     * A class which helps with knowing when a scene has loaded/unloaded.
     *
     * In Peaks of Yore the ways the built-in scenes load is different
     * to custom levels made in the editor.
     *
     * Not all objects are loaded in the custom levels on the typical
     * Unity scene load, as they're added in dynamically afterwards.
     *
     * There's also a difference between full playtest/playing normally
     * and being in the editor switching between the orbit camera and quick playtest.
     * Switching to and from the orbit camera keeps loading/unloading some objects.
     *
     * This class invokes listeners when the scenes are loaded and unloaded,
     * simplifying the process.
     * </summary>
     */
    public static class SceneLoads {
        /**
         * <summary>
         * Invokes listeners on any scene load event.
         * This includes:
         * - A built-in level fully loads
         * - A custom level fully loads
         * - The player enters quick playtest in the editor
         * </summary>
         */
        public static ValueEvent<Scene> onAnyLoad { get; }
            = new ValueEvent<Scene>();

        /**
         * <summary>
         * Invokes listeners on any scene unload event.
         * This includes:
         * - A built-in level unloads
         * - A custom level unloads
         * - The player exits quick playtest in the editor
         * </summary>
         */
        public static ValueEvent<Scene> onAnyUnload { get; }
            = new ValueEvent<Scene>();

        /**
         * <summary>
         * Invokes listeners when any built-in or custom scene fully loads.
         *
         * This excludes switching in/out of quick playtest.
         * See <see cref="onQuickPlaytestLoad"/> if you'd like
         * an event for this.
         * </summary>
         */
        public static ValueEvent<Scene> onLoad { get; }
            = new ValueEvent<Scene>();

        /**
         * <summary>
         * Invokes listeners when any built-in or custom scene unloads.
         *
         * This excludes switching in/out of quick playtest.
         * See <see cref="onQuickPlaytestUnload"/> if you'd like
         * an event for this.
         * </summary>
         */
        public static ValueEvent<Scene> onUnload { get; }
            = new ValueEvent<Scene>();

        /**
         * <summary>
         * Invokes listeners when a built-in scene loads.
         * </summary>
         */
        public static ValueEvent<Scene> onBuiltinLoad { get; }
            = new ValueEvent<Scene>();


        /**
         * <summary>
         * Invokes listeners when a built-in scene unloads.
         * </summary>
         */
        public static ValueEvent<Scene> onBuiltinUnload { get; }
            = new ValueEvent<Scene>();

        /**
         * <summary>
         * Invokes listeners when a custom scene loads
         * through full playtesting/playing normally.
         * </summary>
         */
        public static ValueEvent<Scene> onCustomLoad { get; }
            = new ValueEvent<Scene>();

        /**
         * <summary>
         * Invokes listeners when a custom scene unloads
         * from full playtesting/playing normally.
         * </summary>
         */
        public static ValueEvent<Scene> onCustomUnload { get; }
            = new ValueEvent<Scene>();

        /**
         * <summary>
         * Invokes listeners when quick playtest is entered.
         * </summary>
         */
        public static ValueEvent<Scene> onQuickPlaytestLoad { get; }
            = new ValueEvent<Scene>();

        /**
         * <summary>
         * Invokes listeners when quick playtest is exited.
         * </summary>
         */
        public static ValueEvent<Scene> onQuickPlaytestUnload { get; }
            = new ValueEvent<Scene>();

        /**
         * <summary>
         * Custom level full playtest/normal play.
         * </summary>
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CustomLevel_DistanceActivator), "InitializeObjects")]
        private static void OnFullPlaytest() {
            Scene scene = SceneManager.GetActiveScene();
            onCustomLoad.Invoke(scene);
            onLoad.Invoke(scene);
            onAnyLoad.Invoke(scene);
        }

        /**
         * <summary>
         * Custom level quick playtest (entering/exiting orbit camera).
         * </summary>
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditorManager), "SetPlaymodeObjects")]
        private static void OnQuickPlaytest(bool isPlaymode) {
            Scene scene = SceneManager.GetActiveScene();

            // Quick playtest loaded
            if (isPlaymode == true) {
                onQuickPlaytestLoad.Invoke(scene);
                onAnyLoad.Invoke(scene);
            }
            // Unloaded
            else {
                onQuickPlaytestUnload.Invoke(scene);
                onAnyUnload.Invoke(scene);
            }
        }

        /**
         * <summary>
         * The normal Unity scene load.
         * </summary>
         */
        internal static void OnSceneLoaded(Scene scene) {
            // Only built-in scenes are loaded at this point
            if (scene.buildIndex != 69) {
                onBuiltinLoad.Invoke(scene);
                onLoad.Invoke(scene);
                onAnyLoad.Invoke(scene);
            }
        }

        /**
         * <summary>
         * The normal Unity scene unload.
         * </summary>
         */
        internal static void OnSceneUnloaded(Scene scene) {
            if (scene.buildIndex != 69) {
                onBuiltinUnload.Invoke(scene);
            }
            else {
                onCustomUnload.Invoke(scene);
            }

            onUnload.Invoke(scene);
            onAnyUnload.Invoke(scene);
        }
    }
}
