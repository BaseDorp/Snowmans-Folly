using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoostPowerup : Powerup
{
    [Header("Speed Parameter")]
    [Tooltip("How much the velocity is added by when hitting this powerup.")]
    [SerializeField]
    private float velocityGain;

    public override void OnPlayerEnter(SnowmanControl player)
    {
        StatProfile playerStats = player.gameObject.GetComponent<StatProfile>();
        if(playerStats!=null)
        {
            velocityGain += playerStats[StatType.Acceleration].Value;
        }
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x + velocityGain, player.GetComponent<Rigidbody2D>().velocity.y);
        PlayPowerupSound(player);
        Destroy(gameObject);
    }
}
