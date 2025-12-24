using System;
using System.Collections.Generic;

using HarmonyLib;
using UnityEngine.SceneManagement;

using UILib.Behaviours;

namespace UILib.Patches {
    /**
     * <summary>
     * The types of scene loads/unloads which you can
     * add listeners to <see cref="SceneLoads"/> for.
     * </summary>
     */
    [Flags]
    public enum SceneType {
        /**
         * <summary>
         * Any built-in scene.
         * </summary>
         */
        BuiltIn = 1 << 0,

        /**
         * <summary>
         * Any custom made scene (but only normal playtest
         * or playing it normally through the book).
         * </summary>
         */
        Custom = 1 << 1,

        /**
         * <summary>
         * Quick playtest in the editor.
         * </summary>
         */
        QuickPlaytest = 1 << 3,

        /**
         * <summary>
         * A reasonable default to use.
         *
         * Listens to built-in and custom levels in normal play.
         * </summary>
         */
        Default = BuiltIn | Custom,
    }

    /**
     * <summary>
     * A scene listener.
     *
     * Holds a callback and the cases which the
     * callback should run in.
     * </summary>
     */
    internal class SceneListener {
        /**
         * <summary>
         * The callback to run.
         * </summary>
         */
        private Action<Scene> callback;

        /**
         * <summary>
         * The types of scenes this listener
         * will listen for.
         * </summary>
         */
        private SceneType sceneType;

        /**
         * <summary>
         * Initializes a scene listener.
         * </summary>
         * <param name="callback">The callback to run</param>
         * <param name="sceneType">The types of scene loads/unloads to listen for</param>
         */
        internal SceneListener(Action<Scene> callback, SceneType sceneType) {
            this.callback = callback;
            this.sceneType = sceneType;
        }

        /**
         * <summary>
         * Invokes the callback with the provided scene.
         * </summary>
         * <param name="scene">The scene which loaded/unloaded</param>
         * <param name="sceneType">The scene type which loaded/unloaded</param>
         */
        internal void Invoke(Scene scene, SceneType sceneType) {
            if (this.sceneType.HasFlag(sceneType) == false) {
                return;
            }

            callback(scene);
        }
    }

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
         * The listeners for scene loads.
         * </summary>
         */
        private static List<SceneListener> loadListeners = new List<SceneListener>();

        /**
         * <summary>
         * The listeners for scene unloads.
         * </summary>
         */
        private static List<SceneListener> unloadListeners = new List<SceneListener>();

        /**
         * <summary>
         * Adds a scene listener for scene loads.
         * </summary>
         * <param name="callback">The callback to run on the provided load types</param>
         * <param name="sceneType">The types of scene loads to listen for</param>
         */
        public static void AddLoadListener(
            Action<Scene> callback,
            SceneType sceneType = SceneType.Default
        ) {
            if (callback == null) {
                return;
            }

            loadListeners.Add(
                new SceneListener(callback, sceneType)
            );
        }

        /**
         * <summary>
         * Adds a scene listener for scene unloads.
         * </summary>
         * <param name="callback">The callback to run on the provided unload types</param>
         * <param name="sceneType">The types of scene unloads to listen for</param>
         */
        public static void AddUnloadListener(
            Action<Scene> callback,
            SceneType sceneType = SceneType.Default
        ) {
            if (callback == null) {
                return;
            }

            unloadListeners.Add(
                new SceneListener(callback, sceneType)
            );
        }

        /**
         * <summary>
         * Invokes a collection of listeners with the provided
         * scene type.
         * </summary>
         * <param name="listeners">The listeners to invoke</param>
         * <param name="sceneType">The scene type which loaded/unloaded</param>
         */
        private static void Invoke(List<SceneListener> listeners, SceneType sceneType) {
            Scene scene = SceneManager.GetActiveScene();

            foreach (SceneListener listener in listeners) {
                listener.Invoke(scene, sceneType);
            }
        }

        /**
         * <summary>
         * Custom level full playtest/normal play.
         * </summary>
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CustomLevel_DistanceActivator), "InitializeObjects")]
        private static void OnFullPlaytest() {
            Invoke(loadListeners, SceneType.Custom);
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

            if (isPlaymode == true) {
                Invoke(loadListeners, SceneType.QuickPlaytest);
            }
            else {
                Invoke(unloadListeners, SceneType.QuickPlaytest);
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
                Invoke(loadListeners, SceneType.BuiltIn);
            }
        }

        /**
         * <summary>
         * The normal Unity scene unload.
         * </summary>
         */
        internal static void OnSceneUnloaded(Scene scene) {
            if (scene.buildIndex != 69) {
                Invoke(loadListeners, SceneType.BuiltIn);
            }
            else {
                Invoke(loadListeners, SceneType.Custom);
            }
        }
    }
}
