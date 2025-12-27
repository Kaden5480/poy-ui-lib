using UnityEngine;
using UnityEngine.Audio;

using UEResources = UnityEngine.Resources;

namespace UILib {
    /**
     * <summary>
     * A class for interacting with UILib's audio.
     *
     * Note:
     * If you intend to play multiple arbitrary sounds at once,
     * you should not use this class.
     *
     * It's intended for most simple use cases, and has
     * an `AudioSource` per `Play*` method, but it can't
     * handle playing multiple sounds at once through the same
     * `Play*` method.
     * </summary>
     */
    public static class Audio {
        private static Logger logger = new Logger(typeof(Audio));

        // Root GameObject
        private static GameObject gameObject;

        // Sources
        private static AudioSource genericSource;
        private static AudioSource notificationSource;
        private static AudioSource notificationErrorSource;
        private static AudioSource navigationSource;

        /**
         * <summary>
         * Creates a new GameObject with an AudioSource attached to it.
         * Also attaches this object to the Audio's root object.
         * </summary>
         * <returns>The AudioSource which was created</returns>
         */
        private static AudioSource MakeSource(string name) {
            GameObject obj = new GameObject(name);
            UIObject.SetParent(gameObject, obj);

            AudioSource source = obj.AddComponent<AudioSource>();
            source.bypassReverbZones = true;
            source.bypassEffects = true;
            source.bypassListenerEffects = true;
            source.volume = 0.5f;

            logger.LogDebug($"Made source: {name}");

            return source;
        }

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

            genericSource = MakeSource("Generic");
            notificationSource = MakeSource("Notification");
            notificationErrorSource = MakeSource("NotificationError");
            navigationSource = MakeSource("Navigation");

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

                genericSource.outputAudioMixerGroup = mixerGroup;
                navigationSource.outputAudioMixerGroup = mixerGroup;
                notificationSource.outputAudioMixerGroup = mixerGroup;
                notificationErrorSource.outputAudioMixerGroup = mixerGroup;
                logger.LogDebug("Assigned to SFX mixer group");
            }
        }

        /**
         * <summary>
         * Plays the specified audio clip using a provided source.
         *
         * This is only usable from <see cref="UIRoot.onInit"/> onwards.
         * </summary>
         * <param name="source">The source to play from</param>
         * <param name="clip">The clip to play</param>
         * <param name="volume">The volume to play the clip with</param>
         */
        private static void Play(AudioSource source, AudioClip clip, float volume) {
            if (source == null) {
                logger.LogError("Unable to play audio, the AudioSource was null");
                return;
            }

            source.clip = clip;
            source.volume = volume;
            source.Play();
        }

        /**
         * <summary>
         * Plays the specified audio clip.
         *
         * This is only usable from <see cref="UIRoot.onInit"/> onwards.
         * </summary>
         * <param name="clip">The clip to play</param>
         * <param name="volume">The volume to play the clip with</param>
         */
        public static void Play(AudioClip clip, float volume) {
            Play(genericSource, clip, volume);
        }

        /**
         * <summary>
         * Plays the navigation sound from the provided theme.
         *
         * Passing `null` for the theme means the current theme will be used instead.
         *
         * This is only usable from <see cref="UIRoot.onInit"/> onwards.
         * </summary>
         */
        public static void PlayNavigation(Theme theme = null) {
            if (theme == null) {
                theme = Theme.GetThemeUnsafe();
            }

            Play(
                navigationSource,
                theme.navigationSound,
                theme.navigationSoundVolume
            );
        }

        /**
         * <summary>
         * Plays the notification sound from the provided theme.
         *
         * Passing `null` for the theme means the current theme will be used instead.
         *
         * This is only usable from <see cref="UIRoot.onInit"/> onwards.
         * </summary>
         */
        public static void PlayNormal(Theme theme = null) {
            if (theme == null) {
                theme = Theme.GetThemeUnsafe();
            }

            Play(
                notificationSource,
                theme.notification,
                theme.notificationVolume
            );
        }

        /**
         * <summary>
         * Plays the error notification sound from the provided theme.
         *
         * Passing `null` for the theme means the current theme will be used instead.
         *
         * This is only usable from <see cref="UIRoot.onInit"/> onwards.
         * </summary>
         */
        public static void PlayError(Theme theme = null) {
            if (theme == null) {
                theme = Theme.GetThemeUnsafe();
            }

            Play(
                notificationErrorSource,
                theme.notificationError,
                theme.notificationErrorVolume
            );
        }
    }
}
