using UnityEngine;

namespace UILib.Patches {
    /**
     * <summary>
     * A class which caches objects in the current
     * scene which UILib needs to use.
     * </summary>
     */
    internal static class Cache {
        private static Logger logger = new Logger(typeof(Cache));

        internal static InGameMenu inGameMenu       { get; private set; }
        internal static PeakSummited peakSummited   { get; private set; }
        internal static PlayerManager playerManager { get; private set; }

        /**
         * <summary>
         * Finds objects in the current scene.
         * </summary>
         */
        internal static void FindObjects() {
            inGameMenu = GameObject.FindObjectOfType<InGameMenu>();
            peakSummited = GameObject.FindObjectOfType<PeakSummited>();
            playerManager = GameObject.FindObjectOfType<PlayerManager>();

            logger.LogDebug("Cached objects");
        }

        /**
         * <summary>
         * Clears objects from the cache.
         * </summary>
         */
        internal static void Clear() {
            inGameMenu = null;
            peakSummited = null;
            playerManager = null;

            logger.LogDebug("Cleared objects");
        }
    }
}
