using HarmonyLib;

namespace UILib.Patches {
    /**
     * <summary>
     * Patches for fixing pausing of various
     * things in the game.
     * </summary>
     */
    internal static class PauseFixes {
        private static PauseHandle editorBenchHandle;
        private static PauseHandle inGameMenuHandle;

        /**
         * <summary>
         * Closes a pause handle.
         * </summary>
         * <param name="handle">The handle to close</param>
         */
        private static void CloseHandle(ref PauseHandle handle) {
            if (handle == null) {
                return;
            }

            handle.Close();
            handle = null;
        }

        /**
         * <summary>
         * Patches the workbench in the workshop, making sure
         * that once you enter it, it remains paused.
         * </summary>
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LevelEditor_LoadEditCustomLevel), "EnableLevelEditPanel")]
        private static void EditBooksEnable() {
            CloseHandle(ref editorBenchHandle);
            editorBenchHandle = new PauseHandle();
        }

        /**
         * <summary>
         * Patches the workbench in the workshop, making sure
         * that once you exit it, it unpauses.
         *
         * Also makes sure when selecting to edit a level
         * the pause handle is closed.
         * </summary>
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LevelEditor_LoadEditCustomLevel), "DisableLevelEditPanel")]
        [HarmonyPatch(typeof(LevelEditor_LoadEditCustomLevel), "EditLevel")]
        private static void EditBooksDisable() {
            CloseHandle(ref editorBenchHandle);
        }

        /**
         * <summary>
         * Fixes pausing with the in game menu.
         * </summary>
         * <param name="inGameMenu">The instance to work with</param>
         */
        internal static void InGameMenuCheck(InGameMenu inGameMenu) {
            if (inGameMenu.inMenu == true) {
                if (inGameMenuHandle != null) {
                    inGameMenuHandle = new PauseHandle();
                }
            }
            else {
                CloseHandle(ref inGameMenuHandle);
            }
        }

        /**
         * <summary>
         * Closes any active pause handles in
         * this set of patches on scene changes.
         * </summary>
         */
        internal static void CloseHandles() {
            CloseHandle(ref editorBenchHandle);
            CloseHandle(ref inGameMenuHandle);
        }
    }
}
