using System;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Processes input and applies control to the snowman actor.
/// </summary>
public sealed class SnowmanControl : MonoBehaviour
{
    /// <summary>
    /// This event is called whenever the snowman controller becomes disabled.
    /// </summary>
    public event Action ControlDisabled;

    #region Local Enums
    /// <summary>
    /// Holds the current interaction mode for the snowman actor.
    /// </summary>
    public enum ControlMode : byte
    {
        Disabled,
        Launching,
        Sledding,
        Flying,
        Sliding
    }
    #endregion
    #region Local Fields
    private Vector2 currentNormal;
    private Vector2 normalsAccumulator;
    #endregion
    #region Inspector Fields
    [Tooltip("The stats for this snowman controller.")]
    [SerializeField] private StatProfile stats = null;
    [Tooltip("The current behaviour state of the controller.")]
    [SerializeField] private ControlMode controlMode = ControlMode.Launching;
    [Header("Actor Components")]
    [Tooltip("The body that drives the physics interactions for the controller.")]
    [SerializeField] private Rigidbody2D body = null;
    [Tooltip("The root transform for all renderers.")]
    [SerializeField] private Transform cosmeticsRoot = null;
    [Header("Launch Parameters")]
    [Tooltip("Describes the location and orientation that the actor is returned to during launch.")]
    [SerializeField] private Transform launchPoint = null;
    [Tooltip("Describes the world x location that defines the end of the launch ramp.")]
    [SerializeField] private Transform rampEndMarker = null;
    [Tooltip("Describes the world y location that defines the out of bounds plane.")]
    [SerializeField] private Transform outOfBoundsMarker = null;
    [Header("Flight Parameters")]
    [Tooltip("The stamina system used during flight.")]
    [SerializeField] private StaminaSystem staminaSystem = null;
    [Tooltip("Seconds elapsed to transition from sledding to flight.")]
    [SerializeField] private float flightTransitionTime = 0f;
    [Tooltip("Determines the base strength of the flapping control.")]
    [SerializeField] private float baseFlapStrength = 0f;
    [Tooltip("The amount of stamina required to flap.")]
    [SerializeField] private float staminaUsePerFlap = 30f;
    [Header("Input Channels")]
    [Tooltip("The button broadcaster for when the player should launch.")]
    [SerializeField] private ButtonDownBroadcaster onLaunchBroadcaster = null;
    [Tooltip("The button broadcaster for when the player should flap to gain altitude.")]
    [SerializeField] private ButtonDownBroadcaster onWingFlapBroadcaster = null;
    #endregion
#if DEBUG
    #region Gizmos Implementation
    private void OnDrawGizmosSelected()
    {
        if (rampEndMarker != null)
        {
            // Draw a vertical dashed line to denote the ramp end location.
            Gizmos.color = Color.white;
            GizmosHelper.DrawAsymptote(rampEndMarker.position.x);
            Handles.Label(rampEndMarker.position + Vector3.right * 0.5f, "Ramp End");
        }
    }
    #endregion
#endif
    #region Initialization
    private void Start()
    {
        PlayerService.AddPlayer(this);
        Mode = controlMode;
    }
    #endregion
    #region Properties
    /// <summary>
    /// The stats for this snowman controller.
    /// </summary>
    public StatProfile StatProfile { get => stats; }
    /// <summary>
    /// Whether the snowman is currently sliding along a surface.
    /// </summary>
    public bool IsOnSurface { get; private set; }
    /// <summary>
    /// The current interaction mode for the snowman actor.
    /// </summary>
    public ControlMode Mode
    {
        set
        {
            controlMode = value;
            // Clear all input channels, before applying them
            // in the context of each mode.
            onLaunchBroadcaster.Listener = null;
            onWingFlapBroadcaster.Listener = null;
            // Set lowest common denominator values for all states.
            body.isKinematic = false;

            // Initialize the state for each mode type.
            switch (value)
            {
                case ControlMode.Launching:
                    cosmeticsRoot.up = Vector3.up;
                    onLaunchBroadcaster.Listener = OnLaunchPressed;
                    transform.position = launchPoint.position;
                    body.isKinematic = true;
                    body.velocity = Vector2.zero;
                    // Ensure stats are up to date. TODO should not be in this setter.
                    staminaSystem.MaxStamina = stats[StatType.Propulsion].Value;
                    staminaSystem.Stamina = staminaSystem.MaxStamina;
                    break;
                case ControlMode.Sledding:
                    cosmeticsRoot.up = Vector3.up;
                    break;
                case ControlMode.Flying:
                    cosmeticsRoot.up = Vector3.right;
                    onWingFlapBroadcaster.Listener = OnWingFlapPressed;
                    break;
                case ControlMode.Sliding:
                    cosmeticsRoot.up = Vector3.right;
                    break;
                case ControlMode.Disabled:
                    body.isKinematic = true;
                    body.velocity = Vector2.zero;
                    ControlDisabled?.Invoke();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
    #endregion
    #region Collisions Implementation
    private Vector2 priorFrameVelocity;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // This bounce threshold ensure that at some velocity
        // the snowman will stop bouncing and begin sliding.
        if (controlMode != ControlMode.Sledding
            && priorFrameVelocity.y < -0.2f)
        {
            body.velocity = new Vector2
            {
                x = body.velocity.x,
                y = -priorFrameVelocity.y * stats[StatType.Bounce].Value
            };
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Track and add up collision normals this fixed update.
        foreach (ContactPoint2D contact in collision.contacts)
            normalsAccumulator += contact.normal;
    }
    private void FixedUpdate()
    {
        // Set set according to whether there were collisions
        // on this fixed update step.
        if (normalsAccumulator != Vector2.zero)
        {
            currentNormal = normalsAccumulator.normalized;
            IsOnSurface = true;
        }
        else
        {
            currentNormal = Vector2.zero;
            IsOnSurface = false;
        }
        // Reset the normals for next frame.
        normalsAccumulator = Vector2.zero;

        priorFrameVelocity = body.velocity;
    }
    #endregion
    #region Update Implementation
    private void Update()
    {
        // Switch drawing procedure based on state.
        switch (controlMode)
        {
            case ControlMode.Sledding: UpdateSledding(); break;
            case ControlMode.Flying: UpdateFlying(); break;
            case ControlMode.Sliding: UpdateSliding(); break;
        }
        void UpdateSledding()
        {
            // Align the cosmetics to the current slope.
            if (IsOnSurface)
                cosmeticsRoot.up = currentNormal;
            // Check to see if the extents of the ramp have been exceeded.
            if (body.position.x > rampEndMarker.position.x)
                Mode = ControlMode.Flying;
        }
        void UpdateFlying()
        {
            // Check for changes in state.
            if (IsOnSurface)
                Mode = ControlMode.Sliding;
            else if (body.position.y < outOfBoundsMarker.position.y
                || body.velocity.x < 0f)
                Mode = ControlMode.Disabled;
        }
        void UpdateSliding()
        {
            // Check for changes in state.
            if (!IsOnSurface)
                Mode = ControlMode.Flying;
            else if (body.position.y < outOfBoundsMarker.position.y
                || body.velocity.x < 0f)
                Mode = ControlMode.Disabled;
            else
            {
                // Align the cosmetics such that the snowman
                // is sliding along the current slope.
                cosmeticsRoot.right = -currentNormal;
            }
        }
    }
    #endregion
    #region Input Listeners
    private void OnLaunchPressed()
    {
        Mode = ControlMode.Sledding;
        body.velocity = launchPoint.right * stats[StatType.LaunchSpeed].Value;
    }
    private void OnWingFlapPressed()
    {
        if (staminaSystem.Stamina > staminaUsePerFlap)
        {
            staminaSystem.Stamina -= staminaUsePerFlap;
            if (body.velocity.y < 0f)
            {
                body.velocity = new Vector2
                {
                    x = body.velocity.x,
                    y = baseFlapStrength
                };
            }
            else
            {
                body.velocity = new Vector2
                {
                    x = body.velocity.x,
                    y = body.velocity.y + baseFlapStrength
                };
            }
        }
    }
    #endregion
}
