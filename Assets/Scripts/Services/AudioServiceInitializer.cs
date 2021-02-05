using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

#region Exposed Audio Enums
/// <summary>
/// Identifies a sound effect clip group.
/// </summary>
public enum SoundEffect : byte
{
    ArcticWind,
    Sliding,
    SnowSplat,
    CrashFall,
    CrashShatter,
    PowerupChime,
    PowerupCoin,
    PowerupCookie
}
/// <summary>
/// Identifies a background music track.
/// </summary>
public enum BackgroundTrack : byte
{
    JingleRockin,
    SnowSurfin
}
#endregion

/// <summary>
/// Provides an inspector to initialize the audio service for a given scene.
/// </summary>
public sealed class AudioServiceInitializer : MonoBehaviour
{
    #region Inspector Fields
    [Tooltip("The audio mixer that will be used in this stage.")]
    [SerializeField] private AudioMixer mixer = null;
    [Tooltip("The background tracks that are expected to be requested in this stage.")]
    [SerializeField] private BGMTrackPair[] backgroundTracks = null;
    [Tooltip("The sound effect clips that are expected to be requested in this stage.")]
    [SerializeField] private SFXTrackPair[] effectClips = null;
    [Serializable]
    private sealed class BGMTrackPair
    {
        public BackgroundTrack effect = default;
        public AudioClip track = null;
    }
    [Serializable]
    private sealed class SFXTrackPair
    {
        public SoundEffect effect = default;
        public AudioClip[] clips = null;
    }
    #endregion
    #region Initialization
    private void Awake()
    {
        // Pull the inspector values into dictionaries.
        Dictionary<BackgroundTrack, AudioClip> bgmTracks
             = new Dictionary<BackgroundTrack, AudioClip>();
        if (backgroundTracks != null)
            foreach (BGMTrackPair track in backgroundTracks)
                bgmTracks.Add(track.effect, track.track);
        Dictionary<SoundEffect, AudioClip[]> sfxTracks
            = new Dictionary<SoundEffect, AudioClip[]>();
        if (effectClips != null)
            foreach (SFXTrackPair clip in effectClips)
                sfxTracks.Add(clip.effect, clip.clips);
        // Invoke the audio service to initialize its state.
        AudioService.Initialize(bgmTracks, sfxTracks, mixer);
    }
    #endregion
}

/// <summary>
/// Exposes a basic audio service for playing non-spatial effects and music.
/// </summary>
public static class AudioService
{
    #region Audio Parameters
    private const string SOUND_EFFECTS_TRACK = "Sound Effects";
    private const string BACKGROUND_MUSIC_TRACK = "Background Music";
    #endregion
    #region Debug Parameters
    private const string NOT_INITIALIZED_MESSAGE =
        "Tried to retrieve audio group before audio service was initialized";
    #endregion
    #region Private Fields
    private static bool hasInitialized;
    private static AudioSource sfxSource;
    private static AudioSource bgmSource;
    private static Dictionary<BackgroundTrack, AudioClip> backgroundTracks;
    private static Dictionary<SoundEffect, AudioClip[]> soundEffects;
    private static AudioMixer mixer;
    private static AudioMixerGroup sfxGroup;
    private static AudioMixerGroup bgmGroup;
    #endregion
    #region Initialization
    /// <summary>
    /// Initializes the audio services for a given stage.
    /// </summary>
    /// <param name="backgroundTracks">The tracks that may play in this state.</param>
    /// <param name="soundEffects">The sound effects that may play in this state.</param>
    /// <param name="mixer">The mixer that audio will be routed through.</param>
    public static void Initialize(
        Dictionary<BackgroundTrack, AudioClip> backgroundTracks,
        Dictionary<SoundEffect, AudioClip[]> soundEffects,
        AudioMixer mixer)
    {
        AudioService.backgroundTracks = backgroundTracks;
        AudioService.soundEffects = soundEffects;
        AudioService.mixer = mixer;
        if (!hasInitialized)
        {
            // Create a universal gameobject to hold audio components.
            GameObject sceneObject = new GameObject();
            sceneObject.name = "AUDIO_SINGLETON";
            UnityEngine.Object.DontDestroyOnLoad(sceneObject);
            // Create audio sources for music and effects.
            sfxSource = sceneObject.AddComponent<AudioSource>();
            bgmSource = sceneObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            bgmSource.loop = true;
            sfxGroup = sfxSource.outputAudioMixerGroup = mixer.FindMatchingGroups(SOUND_EFFECTS_TRACK)[0];
            bgmGroup = bgmSource.outputAudioMixerGroup = mixer.FindMatchingGroups(BACKGROUND_MUSIC_TRACK)[0];
            hasInitialized = true;
        }
    }
    #endregion
    #region Exposed Properties
    public static AudioMixerGroup SFXGroup
    {
        get
        {
            if (!hasInitialized)
                throw new Exception(NOT_INITIALIZED_MESSAGE);
            else
                return sfxGroup;
        }
    }
    public static AudioMixerGroup BGMGroup
    {
        get
        {
            if (!hasInitialized)
                throw new Exception(NOT_INITIALIZED_MESSAGE);
            else
                return bgmGroup;
        }
    }
    #endregion
    #region Exposed Methods
    /// <summary>
    /// Plays a random sound effect from the requested sound effect group.
    /// </summary>
    /// <param name="effect">The effect group.</param>
    public static void PlaySoundEffect(SoundEffect effect)
    {
        if (hasInitialized)
            sfxSource.PlayOneShot(soundEffects[effect].RandomElement());
    }
    /// <summary>
    /// Plays and loops a given background track.
    /// </summary>
    /// <param name="track"></param>
    public static void PlayBackgroundTrack(BackgroundTrack track)
    {
        StopBackgroundTrack();
        if (hasInitialized)
        {
            bgmSource.clip = backgroundTracks[track];
            bgmSource.Play();
        }
    }
    /// <summary>
    /// Stops the background track if one is currently playing.
    /// </summary>
    public static void StopBackgroundTrack()
    {
        bgmSource.Stop();
    }
    #endregion
}
