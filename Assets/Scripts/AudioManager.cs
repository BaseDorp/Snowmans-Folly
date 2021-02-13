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

    private bool sfxMuted = false;
    private bool bgmMuted = false;

    private void Start()
    {
        sfxSource.outputAudioMixerGroup = sfxMixer;
        bgmSource.outputAudioMixerGroup = bgmMixer;
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
        int x = Random.Range(0, slidingClips.Length);
        PlaySFX(slidingClips[x]);
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
