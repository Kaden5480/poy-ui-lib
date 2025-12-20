using System.Reflection;

using HarmonyLib;
using UnityEngine;

namespace UILib.Patches {
    /**
     * <summary>
     * Makes sure the <see cref="PauseHandler"/> runs after
     * `InGameMenu`, to make sure the game actually
     * pauses correctly.
     *
     * Also prevent execution while <see cref="InputOverlay"/>
     * is reading an input.
     *
     * Will also manage pause handles for the in game menu.
     *
     * Hooks into <see cref="PlayerVelocityFix"/> as well
     * to make sure the player's velocity gets saved/restored
     * correctly.
     * </summary>
     */
    [HarmonyPatch(typeof(InGameMenu), "Update")]
    internal static class MenuFix {
        /**
         * <summary>
         * Prevents running the InGameMenu when:
         * - The <see cref="InputOverlay"/> is waiting for an input
         * - A <see cref="Components.TextField"/> is currently selected
         * </summary>
         */
        private static bool Prefix(bool ___pausedRB, Rigidbody ___playerRB) {
            // Save player velocity
            PlayerVelocityFix.Save(___playerRB, ___pausedRB);

            if (InputOverlay.waitingForInput == true) {
                return false;
            }

            if (UI.InputFieldFix.isSelected == true) {
                return false;
            }

            return true;
        }

        // Update the PauseHandler
        private static void Postfix(
            InGameMenu __instance,
            bool ___pausedRB,
            Rigidbody ___playerRB
        ) {
            PauseFixes.InGameMenuCheck(__instance);
            PauseHandler.Update();

            // Restore player velocity
            PlayerVelocityFix.Restore(___playerRB, ___pausedRB);
        }
    }
}
