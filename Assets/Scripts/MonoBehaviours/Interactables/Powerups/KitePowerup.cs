using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitePowerup : Powerup
{
    [Tooltip("How long the kite lasts")]
    [SerializeField]
    private float kiteDuration;
    [Tooltip("How fast the kite moves the player upwards")]
    [SerializeField]
    private float yVelocity;

    public override void OnPlayerEnter(SnowmanControl player)
    {
        player.SetKite(this.gameObject,kiteDuration,yVelocity);
    }
}
