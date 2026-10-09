using HarmonyLib;

namespace UILib.Patches {
    /**
     * <summary>
     * Patches the editor so it behaves nicely with pausing.
     * </summary>
     */
    internal static class EditorFixes {
        /**
         * <summary>
         * Prevents using the majority of peak editor controls
         * while paused.
         * </summary>
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LevelEditorOrbitCamera), "Update")]
        private static bool DisableCamera() {
            // Prevent executing if paused
            if (LockHandler.isPaused == true) {
                return false;
            }

            return true;
        }

        /**
         * <summary>
         * Prevents undoing/redoing while paused.
         * </summary>
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(HistoryManager), "Update")]
        private static bool DisableHistory() {
            // Prevent executing if paused
            if (LockHandler.isPaused == true) {
                return false;
            }

            return true;
        }
    }
}
