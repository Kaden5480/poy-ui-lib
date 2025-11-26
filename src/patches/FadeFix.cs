using HarmonyLib;

namespace UILib.Patches {
    /**
     * <summary>
     * The `LevelLoader` decides to disable all canvases, which
     * is a bit annoying. This patch fixes that by re-enabling
     * all UILib canvases.
     * </summary>
     */
    [HarmonyPatch(typeof(LevelLoader), "LoadAfterFade", MethodType.Enumerator)]
    internal static class FadeFix {
        private static void Postfix() {
            UIRoot.EnableCanvases();
        }
    }
}
