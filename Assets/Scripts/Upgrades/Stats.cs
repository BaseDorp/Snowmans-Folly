using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    /// <summary>
    /// Base Value of the stats
    /// </summary>
    public static float maxSpeed;
    public static float acceleration;
    public static float propulsion;
    public static float luck;
    public static float profit;
    public static float durability;
    public static float launchSpeed;
    public static float friction;
    public static float bounce;

   /// <summary>
   /// Amount of times the stat has been upgraded
   /// </summary>
    public static int maxSpeedLevel;
    public static int accelerationLevel;
    public static int propulsionLevel;
    public static int luckLevel;
    public static int profitLevel;
    public static int durabilityLevel;
    public static int launchSpeedLevel;
    public static int frictionLevel;
    public static int bounceLevel;

    /// <summary>
    /// Current value of the stats
    /// </summary>
    public static float currentMaxSpeed;
    public static float currentAcceleration;
    public static float currentPropulsion;
    public static float currentLuck;
    public static float currentProfit;
    public static float currentDurability;
    public static float currentLaunchSpeed;
    public static float currentFriction;
    public static float currentBounce;

    public static void UpgradeStats(string statName,float upgradeValue)
    {
        switch (statName)
        {
            case "maxSpeed":
                {
                    maxSpeedLevel++;
                    currentMaxSpeed = maxSpeed + (maxSpeed * (maxSpeedLevel * (upgradeValue/100)));
                    return;
                }
            case "acceleration":
                {
                    accelerationLevel++;
                    currentAcceleration = acceleration + (acceleration * (accelerationLevel * (upgradeValue/100)));
                    return;
                }
            case "propulsion":
                {
                    propulsionLevel++;
                    currentPropulsion = propulsion + (propulsion * (propulsionLevel * (upgradeValue / 100)));
                    return;
                }
            case "luck":
                {
                    luckLevel++;
                    currentLuck = luck + (luck * (luckLevel * (upgradeValue / 100)));
                    return;
                }
            case "profit":
                {
                    profitLevel++;
                    currentProfit = profit + (profit * (profitLevel * (upgradeValue / 100)));
                    return;
                }
            case "durability":
                {
                    durabilityLevel++;
                    currentDurability = durability + (durability * (durabilityLevel * (upgradeValue / 100)));
                    return;
                }
            case "launchSpeed":
                {
                    launchSpeedLevel++;
                    currentLaunchSpeed = launchSpeed + (launchSpeed * (launchSpeedLevel * (upgradeValue / 100)));
                    return;
                }
            case "friction":
                {
                    frictionLevel++;
                    currentFriction = friction + (friction * (frictionLevel * (upgradeValue / 100)));
                    return;
                }
            case "bounce":
                {
                    bounceLevel++;
                    currentBounce = bounce + (bounce * (bounceLevel * (upgradeValue / 100)));
                    return;
                }
        }
    }
}
