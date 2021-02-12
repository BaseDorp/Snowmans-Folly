using System.Collections;
using System.Collections.Generic;
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
    private AudioClip powerupClips;
    [SerializeField]
    private AudioClip slidingClips;
    [SerializeField]
    private AudioClip[] bgmClips;

    private bool sfxMuted = false;
    private bool bgmMuted = false;

    public void PlaySFX(AudioClip SFX)
    {
        sfxSource.PlayOneShot(SFX);
    }

    public void PlayBGM(AudioClip BGM)
    {
        bgmSource.clip = BGM;
        bgmSource.Play();
    }

}
