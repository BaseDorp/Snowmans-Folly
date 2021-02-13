using UnityEngine;
using TMPro;

/// <summary>
/// Readout driven by a player's distance relative to a transform.
/// </summary>
public sealed class DistanceReadout : MonoBehaviour
{
    #region Parameters
    private const string RECORD_PREFIX = "Best: ";
    private const string DISTANCE_SUFFIX = "m";
    #endregion
    #region Inspector Fields
    [Tooltip("The text for the current run distance.")]
    [SerializeField] private TMP_Text currentRunText = null;
    [Tooltip("The text for the best run distance.")]
    [SerializeField] private TMP_Text recordText = null;
    [Tooltip("Marks the x-coordinate where distance starts to become tracked.")]
    [SerializeField] private Transform zeroDistanceMarker = null;
    [Tooltip("The transform of the object being tracked.")]
    [SerializeField] private Transform distanceTransform = null;
    #endregion
    #region Private Fields
    private int bestDistance;
    // Prevents the text from updating every frame.
    // Ensures that if we add a value changed animation
    // it will not get stuck on frame 0.
    private int lastCheckDistance;
    #endregion
    #region Initialization
    private void Awake()
    {
        // TODO this should not be invoked here.
        // This should be bound to an event that fires when the run starts.
        OnRunStart();
    }

    private void OnEnable()
    {
        SnowmanControl.ControlDisabled += OnRunEnd;
    }
    #endregion
    #region Destructor Clean Up
    private void OnDestroy()
    {
        // This assists the garbage collector.
        UpdateContext.Update -= UpdateDistance;
    }
    #endregion
    #region Update Routine
    private void UpdateDistance()
    {
        // What is the current distance truncated to an int
        // or zero if it is less than zero.
        int currentDistance = Mathf.Max(0, (int)(distanceTransform.position.x - zeroDistanceMarker.position.x));
        if (currentDistance > lastCheckDistance)
        {
            // Update the current run distance.
            currentRunText.text = $"{currentDistance}{DISTANCE_SUFFIX}";
            // Update the record distance if it has been beat.
            if (currentDistance > bestDistance)
            {
                bestDistance = currentDistance;
                recordText.text = $"{RECORD_PREFIX}{currentDistance}{DISTANCE_SUFFIX}";
            }
            lastCheckDistance = currentDistance;
        }
    }
    #endregion
    
    #region Run State Listeners
    // Detach from Update when a run is not in progress.
    private void OnRunStart()
    {
        UpdateContext.Update += UpdateDistance;
        lastCheckDistance = 0;
    }
    private void OnRunEnd()
    {
        //TODO: Make this not a hard call
        StatProfile snowmanStats = distanceTransform.gameObject.GetComponent<StatProfile>();
        Currency.Coins += ((int)snowmanStats[StatType.Profit].Value+1) * (int)(lastCheckDistance / 100);
        UpdateContext.Update -= UpdateDistance;
    }
    #endregion
}
