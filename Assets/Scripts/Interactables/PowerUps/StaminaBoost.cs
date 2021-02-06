using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaBoost : Interactable
{
    [Header("Stamina Parameter")]
    [Tooltip("How much Stamina is added when the player hits this interactable.")]
    [SerializeField]
    private float staminaFlatGain;
    [Tooltip("The percent stamina that is added when the player hits this interactable.")]
    [SerializeField]
    [Range(0,100)]
    private float staminaPercentGain;
    [Tooltip("If true, will use percent gain")]
    [SerializeField]
    private bool usePercentGain;

    public override void OnPlayerEnter(SnowmanControl player)
    {
        if(usePercentGain)
        {
            player.GetComponent<StaminaSystem>().Stamina += player.GetComponent<StaminaSystem>().MaxStamina*(staminaPercentGain/100);
        }
        else
        {
            player.GetComponent<StaminaSystem>().Stamina += staminaFlatGain;
        }
        Destroy(gameObject);
    }
}
