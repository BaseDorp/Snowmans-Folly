using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource sfxSource;
    [SerializeField]
    private AudioSource bgmSource;
    [SerializeField]
    private AudioSource slidingSource;
    [SerializeField]
    private AudioClip[] collisionClips;
    [SerializeField]
    private AudioClip[] powerupClips;
    [SerializeField]
    private AudioClip[] slidingClips;
    [SerializeField]
    private AudioClip menuBGMClip;
    [SerializeField]
    private AudioClip[] bgmClips;
    [SerializeField]
    private AudioClip cookieClip;
    [SerializeField]
    private AudioMixerGroup sfxMixer;
    [SerializeField]
    private AudioMixerGroup bgmMixer;
    [SerializeField]
    private AudioMixerGroup slidingMixer;

    private bool sfxMuted = false;
    private bool bgmMuted = false;

    public static AudioManager CurrentManager { get; private set; }

    private void Start()
    {
        sfxSource.outputAudioMixerGroup = sfxMixer;
        bgmSource.outputAudioMixerGroup = bgmMixer;
        slidingSource.outputAudioMixerGroup = slidingMixer;

        CurrentManager = this;

        PlayBGM(menuBGMClip);
    }

    private void OnEnable()
    {
        SnowmanControl.Launched += PlayRandomBGM;
    }

    public void PlaySFX(AudioClip SFX)
    {
        sfxSource.PlayOneShot(SFX);
    }

    public void PlayBGM(AudioClip BGM)
    {
        bgmSource.clip = BGM;
        bgmSource.Play();
    }

    public void PlayCookie()
    {
        PlaySFX(cookieClip);
    }

    public void PlayPowerup()
    {
        int x = Random.Range(0, powerupClips.Length);
        PlaySFX(powerupClips[x]);
    }

    public void PlayCollision()
    {
        int x = Random.Range(0, collisionClips.Length);
        PlaySFX(collisionClips[x]);
    }
    
    public void PlaySlide()
    {
        if(slidingSource.isPlaying==false)
        {
            int x = Random.Range(0, slidingClips.Length);
            slidingSource.clip = slidingClips[x];
            slidingSource.Play();
        }
    }

    public void StopPlayingSlide()
    {
        slidingSource.Stop();
    }

    public void PlayRandomBGM()
    {
        int x = Random.Range(0, bgmClips.Length);
        PlayBGM(bgmClips[x]);
    }

    public void SetBGMVolume()
    {
    }

    public void SetSFXVolume()
    {
    }

    public void ToggleBGMMute()
    {
            bgmSource.mute = !bgmSource.mute;
    }

    public void ToggleSFXMute()
    {
        sfxSource.mute = !sfxSource.mute;
    }
}
