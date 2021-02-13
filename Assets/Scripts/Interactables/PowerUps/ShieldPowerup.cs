using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerup : Powerup
{
    public override void OnPlayerEnter(SnowmanControl player)
    {
        player.SetShielded(this.gameObject);
    }
}
