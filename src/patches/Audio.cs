using HarmonyLib;
using UnityEngine;

namespace UILib.Patches {
    /**
     * <summary>
     * Patches the game's audio to play a custom
     * navigation sound instead.
     * </summary>
     */
    internal static class Audio {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(AudioSource), "PlayHelper")]
        private static bool PatchPlay(AudioSource source) {
            // Do nothing on null sources
            if (Cache.menuClicks.Count < 1) {
                return true;
            }

            // Make sure it's the in game menu one
            foreach (AudioSource src in Cache.menuClicks) {
                if (src == source) {
                    UILib.Audio.PlayNavigation();
                    return false;
                }
            }

            return true;
        }
    }
}
