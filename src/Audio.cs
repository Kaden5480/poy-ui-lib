using UnityEngine;
using UnityEngine.Audio;

using UEResources = UnityEngine.Resources;

namespace UILib {
    /**
     * <summary>
     * A class for interacting with UILib's audio.
     * </summary>
     */
    public static class Audio {
        private static Logger logger = new Logger(typeof(Audio));

        private static GameObject gameObject;
        private static AudioSource source;

        /**
         * <summary>
         * Initializes the audio.
         * </summary>
         */
        internal static void Init() {
            if (gameObject != null) {
                return;
            }

            gameObject = new GameObject($"{typeof(Audio)}");
            UIObject.SetParent(UIRoot.gameObject, gameObject);

            source = gameObject.AddComponent<AudioSource>();
            source.bypassReverbZones = true;
            source.bypassEffects = true;
            source.bypassListenerEffects = true;
            source.volume = 0.5f;

            logger.LogDebug("Initialized");

            FindMixer();
        }

        /**
         * <summary>
         * Try finding the SFX mixer group and
         * then assign the audio source to it.
         * </summary>
         */
        internal static void FindMixer() {
            logger.LogDebug("Trying to find mixer group");

            foreach (AudioMixerGroup mixerGroup
                in UEResources.FindObjectsOfTypeAll<AudioMixerGroup>()
            ) {
                if (mixerGroup.name.Equals("SFX") == false) {
                    continue;
                }

                logger.LogDebug("Assigned to SFX mixer group");
                source.outputAudioMixerGroup = mixerGroup;
            }
        }

        /**
         * <summary>
         * Plays the specified audio clip.
         * </summary>
         * <param name="clip">The clip to play</param>
         * <param name="volume">The volume to play the clip with</param>
         */
        public static void Play(AudioClip clip, float volume) {
            source.clip = clip;
            source.volume = volume;
            source.Play();
        }

        /**
         * <summary>
         * Plays the navigation sound from the provided theme.
         *
         * Passing `null` for the theme means the current theme will be used instead.
         * </summary>
         */
        public static void PlayNavigation(Theme theme = null) {
            if (theme == null) {
                theme = Theme.GetThemeUnsafe();
            }

            source.clip = theme.navigationSound;
            source.volume = theme.navigationSoundVolume;

            source.Play();
        }

        /**
         * <summary>
         * Plays the notification sound from the provided theme.
         *
         * Passing `null` for the theme means the current theme will be used instead.
         * </summary>
         */
        public static void PlayNormal(Theme theme = null) {
            if (theme == null) {
                theme = Theme.GetThemeUnsafe();
            }

            Play(theme.notification, theme.notificationVolume);
        }

        /**
         * <summary>
         * Plays the error notification sound from the provided theme.
         *
         * Passing `null` for the theme means the current theme will be used instead.
         * </summary>
         */
        public static void PlayError(Theme theme = null) {
            if (theme == null) {
                theme = Theme.GetThemeUnsafe();
            }

            Play(theme.notificationError, theme.notificationErrorVolume);
        }
    }
}
