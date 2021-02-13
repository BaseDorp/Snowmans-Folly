using UnityEngine;

/// <summary>
/// Grants coins to any intersecting player body.
/// </summary>
public sealed class CoinPickup : Interactable
{
    #region Inspector Fields
    [Header("Currency Parameters")]
    [Tooltip("The base value of the coin pickup.")]
    [SerializeField] private int coinValue = 1;
    protected override sealed void OnValidate()
    {
        base.OnValidate();
        coinValue.Clamp(1, int.MaxValue);
    }
    #endregion
    #region Player Enter Listener
    public override void OnPlayerEnter(SnowmanControl player)
    {
        //TODO: Make coins not rely on the audio manager being on the player game object
        player.gameObject.GetComponent<AudioManager>().PlayCookie();
        Currency.Coins += coinValue;
        Destroy(gameObject);
    }
    #endregion
}
