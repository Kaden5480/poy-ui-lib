using HarmonyLib;

namespace UILib.Patches {
    /**
     * <summary>
     * Makes sure the PauseHandler runs after
     * InGameMenu, to make sure the game actually
     * pauses correctly.
     * </summary>
     */
    [HarmonyPatch(typeof(InGameMenu), "Update")]
    internal static class MenuFix {
        private static void Postfix() {
            PauseHandler.Update();
        }
    }
}
