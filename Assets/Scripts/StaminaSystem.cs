using UnityEngine;

/// <summary>
/// Implements a basic stamina regeneration system.
/// </summary>
public sealed class StaminaSystem : MonoBehaviour
{
    #region State Change Events
    /// <summary>
    /// A listener for changes in a stamina system.
    /// </summary>
    /// <param name="currentStamina">The new current stamina of the system.</param>
    /// <param name="maxStamina">The new max stamina of the system.</param>
    public delegate void StaminaChangedListener(float currentStamina, float maxStamina);
    // This event is used primarily to broadcast to linked in UI.
    /// <summary>
    /// Called whenever the stamina value changes.
    /// </summary>
    public event StaminaChangedListener StaminaChanged;
    #endregion
    #region Inspector Fields
    [Tooltip("The current stamina in the system.")]
    [SerializeField] private float stamina = 0.0f;
    [Header("Stamina Parameters")]
    [Tooltip("The maximum amount of stamina the system can contain.")]
    [SerializeField] private float maxStamina = 100.0f;
    [Header("Regeneration Parameters")]
    [Tooltip("Number of seconds of inactivity before the system begins regenerating.")]
    [SerializeField] private float regenerationDelay = 0.5f;
    [Tooltip("Stamina regenerated per second.")]
    [SerializeField] private float regenerationRate = 20.0f;
#if DEBUG
    private void OnValidate()
    {
        stamina.Clamp(0f, float.MaxValue);
        maxStamina.Clamp(0f, float.MaxValue);
        regenerationDelay.Clamp(0f, float.MaxValue);
        regenerationRate.Clamp(0f, float.MaxValue);
    }
#endif
    #endregion
    #region Local Fields
    // Keeps track of the last moment in
    // time that this system was effected
    // by an outside actor/controller.
    private float lastEffectedTime;
    #endregion
    #region Properties
    /// <summary>
    /// The stamina within the system, bound between 0 and max stamina.
    /// </summary>
    public float Stamina
    {
        get => stamina;
        set
        {
            float valueClamped = Mathf.Clamp(value, 0f, maxStamina);
            if (valueClamped != stamina)
            {
                stamina = valueClamped;
                lastEffectedTime = Time.time;
                StaminaChanged?.Invoke(stamina, maxStamina);
            }
        }
    }
    /// <summary>
    /// The max stamina that the system can hold.
    /// </summary>
    public float MaxStamina
    {
        get => maxStamina;
        set
        {
            maxStamina = Mathf.Max(0f, value);
            if (stamina > maxStamina)
                stamina = maxStamina;
            StaminaChanged?.Invoke(stamina, maxStamina);
        }
    }
    /// <summary>
    /// The seconds of inactivity before regeneration starts.
    /// </summary>
    public float RegenerationDelay
    {
        get => regenerationDelay;
        set { regenerationDelay = Mathf.Max(0f, value); }
    }
    /// <summary>
    /// The amount of energy regenerated per second.
    /// </summary>
    public float RegenerationRate
    {
        get => regenerationRate;
        set { regenerationRate = Mathf.Max(0f, value); }
    }
    #endregion
    #region Regeneration Implementation
    private void Update()
    {
        if (stamina < maxStamina
            && Time.time - lastEffectedTime > regenerationDelay)
        {
            stamina += Time.deltaTime * regenerationRate;
            if (stamina > maxStamina)
                stamina = maxStamina;
            StaminaChanged?.Invoke(stamina, maxStamina);
        }
    }
    #endregion
}
