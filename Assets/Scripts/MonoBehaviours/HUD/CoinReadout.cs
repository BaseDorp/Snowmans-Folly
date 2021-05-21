using UnityEngine;
using TMPro;

/// <summary>
/// Readout driven by a player's coin count.
/// </summary>
public sealed class CoinReadout : MonoBehaviour
{
    #region Parameters
    private const string TOTAL_PREFIX = "Total: ";
    #endregion
    #region Inspector Fields
    [Tooltip("The text for the current coins this run.")]
    [SerializeField] private TMP_Text currentRunText = null;
    [Tooltip("The text for the total coins.")]
    [SerializeField] private TMP_Text totalText = null;
    [Tooltip("The text that will update the shops display of coins.")]
    [SerializeField] private TMP_Text coinsText;
    #endregion
    #region Private Fields
    private int coinsAtStartOfRun;
    #endregion
    #region Intialization
    private void Awake()
    {
        // TODO this will have to be changed to be player specific
        // when coins are made non-static.
        Currency.CoinCountChanged += OnCoinsChanged;
        // TODO this should not be invoked here!
        // for testing purposes only. Link this
        // up to an event.
        OnNewRun();
    }
    #endregion
    #region Destructor Clean Up
    private void OnDestroy()
    {
        // This assists the garbage collector.
        Currency.CoinCountChanged -= OnCoinsChanged;
    }
    #endregion
    #region State Change Listeners
    private void OnNewRun()
    {
        // Used to keep track of coins earned specifically this run.
        coinsAtStartOfRun = Currency.Coins;
        // Reset both counter values to reflect
        // new run and shop purchases.
        currentRunText.text = 0.ToString();
        totalText.text = $"{TOTAL_PREFIX}{Currency.Coins}";
    }
    private void OnCoinsChanged(int newCoins)
    {
        // Update the values of both counters.
        currentRunText.text = (newCoins - coinsAtStartOfRun).ToString();
        totalText.text = $"{TOTAL_PREFIX}{newCoins}";
        coinsText.text = $"{newCoins}";
    }
    #endregion
}
