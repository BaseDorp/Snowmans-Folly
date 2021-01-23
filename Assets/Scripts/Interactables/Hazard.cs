using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : Interactable
{
    [Header("Hazard Parameters")]
    [Tooltip("How much speed the player loses when hitting this hazard.")]
    [SerializeField]
    private float velocityLoss;

    public override void OnPlayerEnter(SnowmanControl player)
    {
        player.GetComponent<Rigidbody2D>().AddForce(-player.gameObject.transform.right*velocityLoss);
        /*float x = player.GetComponent<Rigidbody2D>().velocity.x;
        float y = player.GetComponent<Rigidbody2D>().velocity.y;
        x = velocityLoss;
        y = velocityLoss;
        player.GetComponent<Rigidbody2D>().velocity.Set(x, y);*/
    }
}
