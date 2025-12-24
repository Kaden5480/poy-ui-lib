using HarmonyLib;

namespace UILib.Patches {
    /**
     * <summary>
     * Applies patches to the `InGameMenu` to better
     * integrate with UILib.
     * </summary>
     */
    [HarmonyPatch(typeof(InGameMenu), "Update")]
    internal static class GameMenu {
        private static bool wasPausing = false;

        /**
         * <summary>
         * Prevents the InGameMenu from running in certain situations.
         * </summary>
         */
        private static bool Prefix(bool ___pausedRB) {
            // Only save the player's velocity when not pausing
            if (LockHandler.isPaused == true || ___pausedRB == true) {
                wasPausing = true;
            }
            else {
                PlayerVelocity.Save();
            }

            // Prevent execution of InGameMenu when input is locked
            if (LockHandler.isNavigationLocked == true
                || InputOverlay.waitingForInput == true
                || UI.InputFieldFix.isSelected == true
            ) {
                return false;
            }

            // Otherwise, let it run
            return true;
        }

        /**
         * <summary>
         * Updates the <see cref="LockHandler/"> after the
         * `InGameMenu` to allow the lock handler to function.
         * </summary>
         */
        private static void Postfix(InGameMenu __instance, bool ___pausedRB) {
            // Update the InGameMenu's lock
            PauseFixes.InGameMenuCheck(__instance);

            // Update the lock handler
            LockHandler.Update();

            // Only restore if player velocity was being paused
            // and the LockHandler and InGameMenu stopped messing around with it
            if (wasPausing == true
                && LockHandler.isPaused == false
                && ___pausedRB == false
            ) {
                PlayerVelocity.Restore();
                wasPausing = false;
            }
        }
    }
}
