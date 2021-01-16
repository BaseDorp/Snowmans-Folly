using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeInfo : MonoBehaviour
{
    [Tooltip("The name of the object as it will appear in the menu UI.")]
    [SerializeField]
    private string upgradeName;

    [Tooltip("The name of the variable in Stats.cs")]
    [SerializeField]
    private string associatedVariable;

    [Tooltip("The text that will display when the player selects this upgrade in the menu.")]
    [SerializeField]
    private string upgradeDescription;

    [Tooltip("Icon to display for this upgrade in the menu.")]
    [SerializeField]
    private Sprite upgradeIcon;

    [Tooltip("The amount in which the upgrade will improve with each purchase (as a percentage).")]
    [SerializeField]
    private float upgradeAmount;

    [Tooltip("The amount of coins that the upgrade costs.")]
    [SerializeField]
    private int upgradeCost; 

    public Sprite UpgradeIcon => upgradeIcon;
    public string UpgradeDescription => upgradeDescription;
    public string UpgradeName => upgradeName;
    public float UpgradeAmount => upgradeAmount;
    public int UpgradeCost => upgradeCost;

    public void PurchaseUpgrade()
    {
        if(Currency.Coins>=upgradeCost)
        {
            Currency.Spend(upgradeCost);
            Stats.UpgradeStats(associatedVariable,upgradeAmount);
        }
    }
}
