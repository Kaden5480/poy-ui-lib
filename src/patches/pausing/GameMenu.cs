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
        // Whether the player's velocity was being paused
        private static bool shouldPause = false;

        /**
         * <summary>
         * Whether the velocity should currently be paused.
         * </summary>
         */
        private static bool ShouldPause() {
            return LockHandler.isNavigationLocked == true
                || InputOverlay.waitingForInput == true
                || UI.InputFieldFix.isSelected == true;
        }

        /**
         * <summary>
         * Prevents the InGameMenu from running in certain situations.
         * </summary>
         */
        private static bool Prefix(bool ___pausedRB) {
            // Determine whether the player should be paused
            if (ShouldPause() == true || ___pausedRB == true) {
                shouldPause = true;
                PlayerVelocity.Pause();
            }

            // Only save the player's velocity when not pausing
            if (shouldPause == false) {
                PlayerVelocity.Save();
            }

            // If pausing is being managed in a special way
            // by UILib, prevent the execution of InGameMenu
            if (ShouldPause() == true) {
                return false;
            }

            return true;
        }

        /**
         * <summary>
         * Updates the <see cref="LockHandler/"> after the
         * `InGameMenu` to allow the lock handler to function.
         * </summary>
         */
        private static void Postfix(InGameMenu __instance, bool ___pausedRB) {
            PauseFixes.InGameMenuCheck(__instance);

            LockHandler.Update();

            // If pausing should no longer take effect, let
            // the player restore
            if (shouldPause == true
                && ShouldPause() == false
                && ___pausedRB == false
            ) {
                shouldPause = false;
                PlayerVelocity.Restore();
            }
        }
    }
}
