using UnityEngine;
using UnityEngine.Audio;

using UEResources = UnityEngine.Resources;

namespace UILib {
    public static class Audio {
        private static Logger logger = new Logger(typeof(Audio));

        private static GameObject gameObject;
        private static AudioSource source;

        /**
         * <summary>
         * Initializes Audio.
         * </summary>
         */
        internal static void Init() {
            if (gameObject != null) {
                return;
            }

            gameObject = new GameObject("UILib Audio");
            GameObject.DontDestroyOnLoad(gameObject);

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
         * Plays the default normal notification sound.
         * </summary>
         */
        public static void PlayNormal() {
            Play(Resources.notification, 0.8f);
        }

        /**
         * <summary>
         * Plays the default error sound.
         * </summary>
         */
        public static void PlayError() {
            Play(Resources.notificationError, 0.6f);
        }
    }
}
