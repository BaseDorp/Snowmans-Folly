﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject upgradePanel;
    [SerializeField]
    private GameObject gameplayPanel;
    [SerializeField]
    private GameObject menuPanel;
    [SerializeField]
    private GameObject creditsPanel;
    [SerializeField]
    private SnowmanControl snowman;
    [SerializeField]
    private TerrainGenerator terrain;

    private void OnEnable()
    {
        SnowmanControl.ControlDisabled += OpenShop;
    }

    private void Start()
    {
        HidePanel(upgradePanel);
        HidePanel(gameplayPanel);
        HidePanel(creditsPanel);
        ShowPanel(menuPanel);
    }

    public void BeginRun()
    {
        HidePanel(upgradePanel);
        HidePanel(menuPanel);
        HidePanel(creditsPanel);
        ShowPanel(gameplayPanel);
        snowman.Mode = SnowmanControl.ControlMode.Launching;
        terrain.ResetGeneration();
    }

    public void ShowPanel(GameObject Panel)
    {
        Panel.SetActive(true);
    }

    public void HidePanel(GameObject Panel)
    {
        Panel.SetActive(false);
    }

    public void OpenShop()
    {
        ShowPanel(upgradePanel);
        HidePanel(menuPanel);
        HidePanel(gameplayPanel);
        HidePanel(creditsPanel);
    }

    public void DisplayCredits()
    {
        ShowPanel(creditsPanel);
        HidePanel(menuPanel);
        HidePanel(upgradePanel);
        HidePanel(gameplayPanel);
    }

    public void DisplayMenu()
    {
        ShowPanel(creditsPanel);
        HidePanel(menuPanel);
        HidePanel(upgradePanel);
        HidePanel(gameplayPanel);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
