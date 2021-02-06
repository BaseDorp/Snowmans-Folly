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
    [Tooltip("How long infinite stamina should last.")]
    [SerializeField]
    private float infiniteStaminaDuration;
    [Tooltip("What type of stamina powerup")]
    [SerializeField]
    private StaminaType staminaType;

    public enum StaminaType { flat,percent,infinite}

    private StaminaSystem storedPlayerStamina;
    private bool infiniteActivated;

    public override void OnPlayerEnter(SnowmanControl player)
    {
        storedPlayerStamina = player.GetComponent<StaminaSystem>();
        switch (staminaType)
        {
            case StaminaType.percent:
                storedPlayerStamina.Stamina += storedPlayerStamina.MaxStamina * (staminaPercentGain / 100);
                break;
            case StaminaType.flat:
                storedPlayerStamina.Stamina += staminaFlatGain;
                break;
            case StaminaType.infinite:
                infiniteActivated = true;
                StartCoroutine(InfiniteTimer());
                return;
        }
        Destroy(gameObject);
    }

    private void Update()
    {
        if(infiniteActivated)
        {
            storedPlayerStamina.Stamina = storedPlayerStamina.MaxStamina;
        }
    }

    private IEnumerator InfiniteTimer()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(infiniteStaminaDuration);
        Destroy(gameObject);
    }
}
