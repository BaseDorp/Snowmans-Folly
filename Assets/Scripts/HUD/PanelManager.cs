using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject upgradePanel;
    [SerializeField]
    private SnowmanControl snowman;
    [SerializeField]
    private TerrainGenerator terrain;

    private void OnEnable()
    {
        SnowmanControl.ControlDisabled += ShowPanel;
    }

    private void Start()
    {
        HidePanel();
    }

    public void ShowPanel()
    {
        upgradePanel.SetActive(true);
    }

    public void BeginRun()
    {
        HidePanel();
        snowman.Mode = SnowmanControl.ControlMode.Launching;
        terrain.ResetGeneration();
    }

    public void HidePanel()
    {
        upgradePanel.SetActive(false);
    }
}
