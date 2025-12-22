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
            if (source.clip == null) {
                return true;
            }

            if (source.clip.name != "click") {
                return true;
            }

            UILib.Audio.PlayNavigation();
            return false;
        }
    }
}
