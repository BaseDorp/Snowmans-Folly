using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : Interactable
{
    [Header("Hazard Parameters")]
    [Tooltip("How much the velocity is divided by when hitting this hazard.")]
    [SerializeField]
    private float velocityLoss;

    public override void OnPlayerEnter(SnowmanControl player)
    {
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x/velocityLoss, player.GetComponent<Rigidbody2D>().velocity.y);
    }
}
