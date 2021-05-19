using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : Interactable
{
    public override void OnPlayerEnter(SnowmanControl player)
    {
        //TODO: Make hazards not rely on the audio manager being on the player game object
        PlayPowerupSound(player);
    }

    public void PlayPowerupSound(SnowmanControl player)
    {
        AudioManager.CurrentManager.PlayPowerup();
    }
}
