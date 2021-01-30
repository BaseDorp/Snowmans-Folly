using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoostPowerup : Interactable
{
    [Header("Speed Parameter")]
    [Tooltip("How much the velocity is added by when hitting this powerup.")]
    [SerializeField]
    private float velocityGain;

    public override void OnPlayerEnter(SnowmanControl player)
    {
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x + velocityGain, player.GetComponent<Rigidbody2D>().velocity.y);
        Destroy(gameObject);
    }
}
