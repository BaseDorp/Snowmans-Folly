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
    private StatType upgradeStat;

    [Tooltip("The text that will display when the player selects this upgrade in the menu.")]
    [SerializeField]
    private string upgradeDescription;

    [Tooltip("Icon to display for this upgrade in the menu.")]
    [SerializeField]
    private Sprite upgradeIcon;

    [Tooltip("The amount of coins that the upgrade costs.")]
    [SerializeField]
    private int upgradeCost; 

    public Sprite UpgradeIcon => upgradeIcon;
    public string UpgradeDescription => upgradeDescription;
    public string UpgradeName => upgradeName;
    public StatType UpgradeStat => upgradeStat;
    public int UpgradeCost => upgradeCost;

    public void PurchaseUpgrade(StatProfile snowmanProfile)
    {
        if(Currency.Coins>=upgradeCost)
        {
            Currency.Coins -= upgradeCost;
            snowmanProfile[upgradeStat].Level++;
        }
    }

    private void Awake()
    {
    }
}
