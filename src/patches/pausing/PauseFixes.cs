using HarmonyLib;

namespace UILib.Patches {
    /**
     * <summary>
     * Patches for fixing pausing of various
     * things in the game.
     * </summary>
     */
    internal static class PauseFixes {
        private static Lock editorBenchLock;
        private static Lock inGameMenuLock;

        /**
         * <summary>
         * Closes a lock.
         * </summary>
         * <param name="@lock">The lock to close</param>
         */
        private static void CloseLock(ref Lock @lock) {
            if (@lock == null) {
                return;
            }

            @lock.Close();
            @lock = null;
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
            CloseLock(ref editorBenchLock);
            editorBenchLock = new Lock();
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
            CloseLock(ref editorBenchLock);
        }

        /**
         * <summary>
         * Fixes pausing with the in game menu.
         * </summary>
         * <param name="inGameMenu">The instance to work with</param>
         */
        internal static void InGameMenuCheck(InGameMenu inGameMenu) {
            if (inGameMenu.inMenu == true) {
                if (inGameMenuLock == null) {
                    inGameMenuLock = new Lock();
                }
            }
            else {
                CloseLock(ref inGameMenuLock);
            }
        }

        /**
         * <summary>
         * Closes any active pause handles in
         * this set of patches on scene changes.
         * </summary>
         */
        internal static void CloseHandles() {
            CloseLock(ref editorBenchLock);
            CloseLock(ref inGameMenuLock);
        }
    }
}
