using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Currency : MonoBehaviour
{
    [SerializeField]
    private Text coinText;

    public static int Coins;
    public static float profitRate = 0.125f;

    void Awake()
    {
        SetupNewGame();
    }

    private static void SetupNewGame()
    {
        Coins = 100;
    }

    public static void AddRevenue(float distance)
    {
        Coins += (int)(profitRate * distance);
    }

    public static void Spend(int cost)
    {
        Coins -=  cost;
    }

    private void Update()
    {
        if(coinText!=null)
        {
            coinText.text = Coins.ToString();
        }
    }
}
