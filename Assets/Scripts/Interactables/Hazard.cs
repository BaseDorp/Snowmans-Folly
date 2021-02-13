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
        //TODO: Make hazards not rely on the audio manager being on the player game object
        player.gameObject.GetComponent<AudioManager>().PlayCollision();
        player.ApplySlowingForce(velocityLoss);
        Destroy(gameObject);
    }
}
