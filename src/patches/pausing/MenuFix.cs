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
     * </summary>
     */
    [HarmonyPatch(typeof(InGameMenu), "Update")]
    internal static class MenuFix {
        private static bool pausedPlayer;
        private static Vector3 playerVelocity;

        // Don't run when the input overlay is
        // waiting for an input
        private static bool Prefix(bool ___pausedRB, Rigidbody ___playerRB) {
            // Track the player's velocity
            if (___pausedRB == false && ___playerRB != null) {
                playerVelocity = ___playerRB.velocity;
            }

            // If the in game menu paused the player's velocity
            // indicate that
            if (___pausedRB == true) {
                pausedPlayer = true;
            }

            if (InputOverlay.waitingForInput == true) {
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

            if (___playerRB == null) {
                return;
            }

            if (___pausedRB == false && pausedPlayer == true) {
                ___playerRB.velocity = playerVelocity;
                pausedPlayer = false;
            }
        }
    }
}
