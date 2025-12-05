using HarmonyLib;

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
        // Don't run when the input overlay is
        // waiting for an input
        private static bool Prefix() {
            if (InputOverlay.waitingForInput == true) {
                return false;
            }

            return true;
        }

        // Update the PauseHandler
        private static void Postfix(InGameMenu __instance) {
            PauseFixes.InGameMenuCheck(__instance);
            PauseHandler.Update();
        }
    }
}
