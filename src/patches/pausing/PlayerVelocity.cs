using UnityEngine;

namespace UILib.Patches {
    /**
     * <summary>
     * Completely takes control over the player's velocity.
     *
     * Makes saving/restoring actually work properly.
     * </summary>
     */
    internal static class PlayerVelocity {
        private static Logger logger = new Logger(typeof(PlayerVelocity));

        // The currently tracked player velocity
        private static Vector2 velocity = Vector2.zero;

        /**
         * <summary>
         * Restores the player's velocity.
         * </summary>
         * <param name="force">Whether to force a restore</param>
         */
        internal static void Restore() {
            if (Cache.playerRb == null) {
                return;
            }

            Cache.playerRb.drag = 0f;
            Cache.playerRb.velocity = velocity;
            logger.LogDebug($"Restored player's velocity to: {velocity}");
        }

        /**
         * <summary>
         * Saves the player's velocity.
         * </summary>
         */
        internal static void Save() {
            if (Cache.playerRb == null) {
                return;
            }

            velocity = Cache.playerRb.velocity;
        }

        /**
         * <summary>
         * Clears the currently saved velocity.
         * </summary>
         */
        internal static void Clear() {
            logger.LogDebug("Cleared stored velocity");
            velocity = Vector2.zero;
        }

        /**
         * <summary>
         * Keeps the player in a paused state.
         * </summary>
         */
        internal static void Pause() {
            if (Cache.playerRb == null) {
                return;
            }

            Cache.playerRb.drag = 1000f;
            Cache.playerRb.velocity = Vector2.zero;
        }
    }
}
