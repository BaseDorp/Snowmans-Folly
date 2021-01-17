using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InfoPanel : MonoBehaviour
{
    [Tooltip("The Text object that contains the upgrade name.")]
    [SerializeField]
    private Text nameText;

    [Tooltip("The Text object that contains the current stats and how much will change.")]
    [SerializeField]
    private Text statText;

    [Tooltip("The Text object that contains the upgrade description.")]
    [SerializeField]
    private Text descriptionText;

    [Tooltip("The Text object that contains the cost of the upgrade.")]
    [SerializeField]
    private Text costText;

    [Tooltip("The Image that contains the currently selected upgrades icon.")]
    [SerializeField]
    private Image iconImage;

    private UpgradeInfo selectedUpgrade;

    public void SetUpgrade(UpgradeInfo newUpgrade)
    {
        selectedUpgrade = newUpgrade;
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        nameText.text = selectedUpgrade.UpgradeName;
        //statText.text = selectedUpgrade.UpgradeAmount.ToString()+"% increase.";
        descriptionText.text = selectedUpgrade.UpgradeDescription;
        costText.text = selectedUpgrade.UpgradeCost.ToString()+" coins";
        iconImage.sprite = selectedUpgrade.UpgradeIcon;
    }

    public void PurchaseSelectedUpgrade()
    {
        selectedUpgrade.PurchaseUpgrade();
    }
}
