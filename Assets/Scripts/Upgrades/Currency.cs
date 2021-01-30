using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

// TODO convert this to a non-static pattern that
// is owner by player instances.

public class Currency : MonoBehaviour
{
    private static int coins;
    public static int Coins
    {
        get => coins;
        set
        {
            coins = value;
            CoinCountChanged?.Invoke(coins);
        }
    }

    public static event Action<int> CoinCountChanged;

    void Awake()
    {
        SetupNewGame();
    }

    private static void SetupNewGame()
    {
        //Coins = 100;
    }
}
