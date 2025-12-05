using HarmonyLib;

namespace UILib.Patches {
    /**
     * <summary>
     * A variety of patches to prevent interacting
     * with things when a UI is enabled.
     * </summary>
     */
    internal static class InteractFixes {
        [HarmonyPrefix]
        // Journals
        [HarmonyPatch(typeof(AlpsJournal),                     "Update")]
        [HarmonyPatch(typeof(AlpsPeakSelection),               "Update")]
        [HarmonyPatch(typeof(AdvancedJournal),                 "Update")]
        [HarmonyPatch(typeof(AdvancedPeakSelection),           "Update")]
        [HarmonyPatch(typeof(Category4Journal),                "Update")]
        [HarmonyPatch(typeof(CustomPeakJournal_PeakSelection), "Update")]
        [HarmonyPatch(typeof(IntermediateJournal),             "Update")]
        [HarmonyPatch(typeof(IntermediatePeakSelection),       "Update")]
        [HarmonyPatch(typeof(PeakJournal),                     "Update")]
        [HarmonyPatch(typeof(PeakSelection),                   "Update")]

        // Alps Ticket
        [HarmonyPatch(typeof(AlpsGateway), "Update")]

        // Bag for leaving the scene
        [HarmonyPatch(typeof(LeavePeakScene), "Update")]

        // A big class for handling lots of cabin interactions
        [HarmonyPatch(typeof(RopeCabinDescription), "Update")]

        // The routing flag
        [HarmonyPatch(typeof(RoutingFlag), "Update")]

        // The sundown and cruelty monuments
        [HarmonyPatch(typeof(SundownCrueltyMonument), "OnTriggerStay")]
        private static bool DisableInteract() {
            // If should pause, return false
            // to bypass the normal execution of these methods
            if (PauseHandler.isPaused == true
                || InGameMenu.isCurrentlyNavigationMenu == true
            ) {
                return false;
            }

            return true;
        }
    }
}
