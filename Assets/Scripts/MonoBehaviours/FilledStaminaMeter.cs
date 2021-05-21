using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class FilledStaminaMeter : MonoBehaviour
{
    [SerializeField] private float fadeRate = 1f;
    [SerializeField] private float visualDrainRate = 1.0f;

    [SerializeField] private StaminaSystem drivingSystem = null;
    [SerializeField] private Image meterRemaining = null;
    [SerializeField] private Image meterDrain = null;

    public StaminaSystem DrivingSystem
    {
        get => drivingSystem;
        set
        {
            if (drivingSystem != null)
                drivingSystem.StaminaChanged -= OnStaminaChanged;
            drivingSystem = value;
            drivingSystem.StaminaChanged += OnStaminaChanged;
            OnStaminaChanged(drivingSystem.Stamina, drivingSystem.MaxStamina);
        }
    }

    private void OnStaminaChanged(float currentStamina, float maxStamina)
    {
        meterRemaining.fillAmount = Mathf.InverseLerp(0f, maxStamina, currentStamina);
    }
    private void Update()
    {
        if (meterDrain.fillAmount > meterRemaining.fillAmount)
            meterDrain.fillAmount -= Time.deltaTime * visualDrainRate;
        if (meterDrain.fillAmount < meterRemaining.fillAmount)
            meterDrain.fillAmount = meterRemaining.fillAmount;
    }

    private void Awake()
    {
        // Force initialization of driving system.
        DrivingSystem = drivingSystem;
    }
    private void OnDestroy()
    {
        // Assist garbage collector when this instance is destroyed.
        if (drivingSystem != null)
            drivingSystem.StaminaChanged -= OnStaminaChanged;
    }
}
