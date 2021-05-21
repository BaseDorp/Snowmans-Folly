using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : Interactable
{
    public override void OnPlayerEnter(SnowmanControl player)
    {
        PlayPowerupSound();
    }

    public void PlayPowerupSound()
    {
        AudioManager.CurrentManager.PlayPowerup();
    }
}
