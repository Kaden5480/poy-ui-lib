using UnityEngine;

namespace UILib.Patches {
    /**
     * <summary>
     * Patches the player's velocity when pausing/unpausing,
     * so instead of losing all velocity when moving upwards
     * it gets restored correctly.
     * </summary>
     */
    internal static class PlayerVelocityFix {
        private static Logger logger = new Logger(typeof(PlayerVelocityFix));

        private static bool hasRestored;
        private static Vector3 savedVelocity;

        /**
         * <summary>
         * Saves the player's velocity until they're paused.
         * </summary>
         * <param name="playerRb">The player's rigidbody</param>
         * <param name="playerPaused">Whether the player is currently paused</param>
         */
        internal static void Save(Rigidbody playerRb, bool playerPaused) {
            // Don't run when the rigidbody is null
            if (playerRb == null) {
                return;
            }

            // Only update stored velocity when
            // the player is unpaused
            if (playerPaused == false) {
                savedVelocity = playerRb.velocity;
            }

            // If the player just paused, wait for a restore
            if (playerPaused == true && hasRestored == true) {
                logger.LogDebug("Waiting for restore");
                hasRestored = false;
            }
        }

        /**
         * <summary>
         * Restores the player's velocity once they're unpaused.
         * </summary>
         * <param name="playerRb">The player's rigidbody</param>
         * <param name="playerPaused">Whether the player is currently paused</param>
         */
        internal static void Restore(Rigidbody playerRb, bool playerPaused) {
            // Don't run when the rigidbody is null
            if (playerRb == null) {
                return;
            }

            // If the player is paused, do nothing
            if (playerPaused == true) {
                return;
            }

            // Only restore velocity once
            if (hasRestored == true) {
                return;
            }

            logger.LogDebug("Restored player's velocity");
            playerRb.velocity = savedVelocity;
            hasRestored = true;
        }
    }
}
