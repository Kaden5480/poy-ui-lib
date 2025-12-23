using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace UILib.Patches {
    /**
     * <summary>
     * A class which caches objects in the current
     * scene which UILib needs to use.
     * </summary>
     */
    internal static class Cache {
        private static Logger logger = new Logger(typeof(Cache));

        internal static List<AudioSource> menuClicks { get; private set; }
            = new List<AudioSource>();
        internal static InGameMenu inGameMenu        { get; private set; }
        internal static PeakSummited peakSummited    { get; private set; }
        internal static PlayerManager playerManager  { get; private set; }

        /**
         * <summary>
         * Finds an object even if it's inactive.
         * </summary>
         * <returns>The object if found, null otherwise</returns>
         */
        private static T Find<T>() where T : UnityEngine.Object {
            T[] all = UnityEngine.Resources.FindObjectsOfTypeAll<T>();
            if (all.Length < 1) {
                logger.LogDebug($"Unable to find any objects of type: {typeof(T)}");
                return null;
            }

            logger.LogDebug($"Found {typeof(T)}");
            return all[0];
        }

        /**
         * <summary>
         * Finds objects in the current scene.
         * </summary>
         */
        internal static void FindObjects(Scene scene) {
            menuClicks.Clear();

            inGameMenu = Find<InGameMenu>();
            if (inGameMenu != null) {
                AudioSource click = inGameMenu.GetComponent<AudioSource>();
                if (click != null) {
                    menuClicks.Add(click);
                }
                else {
                    logger.LogError("Failed finding InGameMenu's audio source");
                }
            }
            else {
                logger.LogError("Failed finding InGameMenu");
            }

            // Try finding other clicks
            if (scene.buildIndex == 0) {
                GameObject click = GameObject.Find("Click");
                if (click != null) {
                    AudioSource clickSource = click.GetComponent<AudioSource>();
                    if (clickSource != null) {
                        menuClicks.Add(clickSource);
                    }
                    else {
                        logger.LogError("Failed finding main menu Click AudioSource");
                    }
                }
                else {
                    logger.LogError("Failed finding main menu Click");
                }
            }

            peakSummited = Find<PeakSummited>();
            playerManager = Find<PlayerManager>();

            logger.LogDebug("Cached objects");
        }

        /**
         * <summary>
         * Clears objects from the cache.
         * </summary>
         */
        internal static void Clear() {
            menuClicks.Clear();
            inGameMenu = null;
            peakSummited = null;
            playerManager = null;

            logger.LogDebug("Cleared objects");
        }
    }
}
