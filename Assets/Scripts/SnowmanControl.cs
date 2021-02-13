using System;
using System.Collections;
using UnityEngine;
using UnityEditor;

[Serializable]
public sealed class CircleRegion
{
    public Vector2 position = Vector2.zero;
    public float radius = 1f;
}

/// <summary>
/// Processes input and applies control to the snowman actor.
/// </summary>
public sealed class SnowmanControl : MonoBehaviour
{
    /// <summary>
    /// This event is called whenever the snowman controller becomes disabled.
    /// </summary>
    public static event Action ControlDisabled;
    /// <summary>
    /// This event is called whenever the snowman begins launching. 
    /// </summary>
    public static event Action Launched;

    // TODO this is a hack.
    [SerializeField] private TerrainGenerator generator = null;
    [SerializeField] private Transform followCam = null;

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
    private int flapStateHash;
    private float currentBounceEffectiveness;
    // These are used to track a simple simulation
    // of the sled as it departs from the player.
    private Vector2 sledVelocity;
    private Vector2 sledPosition;
    //Shield powerup that attaches to the player
    private GameObject shield;
    //Kite powerup that attaches to the player
    private GameObject kite;
    private Vector2 kiteVelocity;
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
    [Tooltip("The transform pivot for the head.")]
    [SerializeField] private Transform headPivot = null;
    [Header("Collision Parameters")]
    [Tooltip("Controls the visual collision parameters for interactables.")]
    [SerializeField] private CircleRegion[] hitCircles = null;
    [Header("Animation Parameters")]
    [Tooltip("The animator that drives the snowman cosmetics.")]
    [SerializeField] private Animator animator = null;
    [Tooltip("The name associated with triggering a wing flap.")]
    [SerializeField] private string wingFlapStateName = string.Empty;
    [Header("Launch Parameters")]
    [Tooltip("Describes the location and orientation that the actor is returned to during launch.")]
    [SerializeField] private Transform launchPoint = null;
    [Tooltip("Describes the world x location that defines the end of the launch ramp.")]
    [SerializeField] private Transform rampEndMarker = null;
    [Tooltip("Describes the world y location that defines the out of bounds plane.")]
    [SerializeField] private Transform outOfBoundsMarker = null;
    [Tooltip("The sprite renderer for the sled.")]
    [SerializeField] private SpriteRenderer sledRenderer = null;
    [Header("Flight Parameters")]
    [Tooltip("The stamina system used during flight.")]
    [SerializeField] private StaminaSystem staminaSystem = null;
    [Tooltip("Controls the damping factor on the rotation of the snowman.")]
    [SerializeField] private float flightDampingFactor = 5f;
    [Tooltip("Determines the base strength of the flapping control.")]
    [SerializeField] private float baseFlapStrength = 0f;
    [Tooltip("The amount of stamina required to flap.")]
    [SerializeField] private float staminaUsePerFlap = 30f;
    [Header("Bounce Parameters")]
    [Tooltip("Controls how the bounce effectiveness decays with each bounce.")]
    [SerializeField][Range(0f, 1f)] private float bounceDecayFactor = 0.9f;
    [Tooltip("At this bounce effectiveness the snowman body will no longer bounce.")]
    [SerializeField][Range(0f, 1f)] private float minBounceFactor = 0.1f;
    [Header("Input Channels")]
    [Tooltip("The button broadcaster for when the player should launch.")]
    [SerializeField] private ButtonDownBroadcaster onLaunchBroadcaster = null;
    [Tooltip("The button broadcaster for when the player should flap to gain altitude.")]
    [SerializeField] private ButtonDownBroadcaster onWingFlapBroadcaster = null;
    [SerializeField] private PhysicsMaterial2D groundPhysics;
    private void OnValidate()
    {
        if (hitCircles != null)
            foreach (CircleRegion circle in hitCircles)
                circle.radius.Clamp(0.1f, float.MaxValue);
    }
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
        Gizmos.color = Color.red;
        if (hitCircles != null)
            foreach (CircleRegion circle in hitCircles)
                GizmosHelper.DrawCircle((Vector2)transform.position + circle.position, circle.radius);
    }
    #endregion
#endif
    #region Initialization
    private void Start()
    {
        PlayerService.AddPlayer(this);
        body.isKinematic = true;
        body.velocity = Vector2.zero;
        flapStateHash = Animator.StringToHash(wingFlapStateName);
        currentBounceEffectiveness = 1f;
        shield = null;
        kite = null;
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
                    sledRenderer.enabled = true;
                    SetPhysics();
                    shield = null;
                    kite = null;
                    Launched?.Invoke();
                    break;
                case ControlMode.Sledding:
                    cosmeticsRoot.up = Vector3.up;
                    break;
                case ControlMode.Flying:
                    if (controlMode == ControlMode.Sledding)
                    {
                        sledVelocity = body.velocity;
                        sledPosition = sledRenderer.transform.position;
                        StartCoroutine(DetachSled());
                    }
                    onWingFlapBroadcaster.Listener = OnWingFlapPressed;
                    break;
                case ControlMode.Sliding:
                    cosmeticsRoot.up = Vector3.right;
                    break;
                case ControlMode.Disabled:
                    body.isKinematic = true;
                    body.velocity = Vector2.zero;
                    // TODO this should not be here.
                    currentBounceEffectiveness = 1f;
                    StartCoroutine(EndRunJumpUp());
                    break;
                default:
                    throw new NotImplementedException();
            }
            controlMode = value;
        }
    }
    /// <summary>
    /// Represents the local circles associated with visual collision.
    /// </summary>
    public CircleRegion[] HitCircles
    {
        get => hitCircles;
    }
    #endregion
    #region Public Methods
    /// <summary>
    /// Applys a leftwards slowing force against the snowman.
    /// </summary>
    /// <param name="force">The intensity of the force.</param>
    public void ApplySlowingForce(float force)
    {
        if(shield==null)
        {
            body.velocity = new Vector2
            {
                x = body.velocity.x / (1 + (force / 2 * (2 - stats[StatType.Durability].Value))),
                y = body.velocity.y
            };
        }
        else
        {
            Destroy(shield);
        }
    }

    public void SetPhysics()
    {
        groundPhysics.friction=StatProfile[StatType.Friction].Value;
    }

    public void SetShielded(GameObject newShield)
    {
        if(shield!=null)
        {
            Destroy(shield);
        }
        shield = newShield;
        shield.transform.parent = gameObject.transform;
    }

    public void SetKite(GameObject newKite,float duration, float yVelocity)
    {
        if(kite!=null)
        {
            Destroy(kite);
        }
        kite = newKite;
        kite.transform.parent = gameObject.transform;
        kiteVelocity = new Vector2(body.velocity.x,yVelocity);
        StartCoroutine(KiteTimer(duration));
    }

    public IEnumerator KiteTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        if(kite!=null)
        {
            Destroy(kite);
        }
    }

    #endregion
    #region Collisions Implementation
    private Vector2 priorFrameVelocity;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // A threshold is placed here to prevent state from
        // flipping each frame in low number physics weirdness.
        if (controlMode != ControlMode.Sledding
            && priorFrameVelocity.y < -0.2f)
        {
            // Stop bouncing at some point.
            if (currentBounceEffectiveness > minBounceFactor)
            {
                // Reverse the velocity along the Y,
                // with intensity based on the bounce stat.
                body.velocity = new Vector2
                {
                    x = body.velocity.x,
                    y = -priorFrameVelocity.y * stats[StatType.Bounce].Value
                        * currentBounceEffectiveness
                };
                // Make the next bounce less effective.
                currentBounceEffectiveness *= bounceDecayFactor;
            }
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
        //If the player is using the kite powerup, momentarily disable velocity clamping
        if(kite==null)
        {
            body.velocity = Vector2.ClampMagnitude(body.velocity, StatProfile[StatType.MaxSpeed].Value);
        }
        else
        {
            body.velocity = kiteVelocity;
        }
        // Update common stuff.
        if (body.velocity.magnitude > 0.2f)
        {
            headPivot.right = Vector3.Slerp(
                headPivot.right,
                body.velocity.normalized,
                flightDampingFactor * Time.deltaTime);
        }
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
                || body.velocity.x <= 0f)
                Mode = ControlMode.Disabled;
            // Add a smoothing function to prevent abrupt cosmetic changes.
            cosmeticsRoot.up = Vector3.Slerp(
                cosmeticsRoot.up,
                body.velocity.normalized,
                flightDampingFactor * Time.deltaTime);
        }
        void UpdateSliding()
        {
            // Check for changes in state.
            if (!IsOnSurface)
            {
                gameObject.GetComponent<AudioManager>().StopPlayingSlide();
                Mode = ControlMode.Flying;
            }
            else if (body.position.y < outOfBoundsMarker.position.y
                || body.velocity.x < 0f)
            {
                gameObject.GetComponent<AudioManager>().StopPlayingSlide();
                Mode = ControlMode.Disabled;
            }
            else
            {
                // Align the cosmetics such that the snowman
                // is sliding along the current slope.
                cosmeticsRoot.right = -currentNormal;
                gameObject.GetComponent<AudioManager>().PlaySlide();
            }
        }
    }
    #endregion
    #region Sled Detach Routine
    private IEnumerator DetachSled()
    {
        // Local state of the transform needs to be stored
        // so the sled can be reset. TODO this is kinda hacky.
        Transform sledParent = sledRenderer.transform.parent;
        Vector3 localPosition = sledRenderer.transform.localPosition;
        Quaternion localRotation = sledRenderer.transform.localRotation;
        sledRenderer.transform.parent = null;
        while (sledRenderer.transform.position.y > outOfBoundsMarker.position.y)
        {
            sledVelocity += Vector2.up * Physics2D.gravity * Time.deltaTime;
            sledPosition += sledVelocity * Time.deltaTime;
            sledRenderer.transform.position = sledPosition;
            yield return null;
        }
        sledRenderer.enabled = false;
        sledRenderer.transform.parent = sledParent;
        sledRenderer.transform.localPosition = localPosition;
        sledRenderer.transform.localRotation = localRotation;
    }
    #endregion

    #region Snowman Jump Up Routine
    private IEnumerator EndRunJumpUp()
    {
        float startY = transform.position.y;
        float startRotation = cosmeticsRoot.eulerAngles.z.Wrapped(0f, 360f);
        float startHeadRotation = headPivot.localEulerAngles.z.Wrapped(0f, 360f);
        // TODO it is sad how hard scripted this is!!!
        float interpolant = 0f;
        while (true)
        {
            interpolant += Time.deltaTime * 0.9f;
            if (interpolant >= 1f)
            {
                headPivot.localEulerAngles = Vector3.zero;
                cosmeticsRoot.eulerAngles = Vector3.zero;
                transform.position = new Vector3
                {
                    x = transform.position.x,
                    y = startY,
                    z = transform.position.z
                };
                break;
            }
            else
            {
                headPivot.localEulerAngles =
                    Vector3.forward * Mathf.Lerp(startHeadRotation, 0f, interpolant);
                cosmeticsRoot.eulerAngles =
                    Vector3.forward * Mathf.Lerp(startRotation, 0f, interpolant);
                transform.position = new Vector3
                {
                    x = transform.position.x,
                    y = startY + 2f * (4f * interpolant) * (1f - interpolant),
                    z = transform.position.z
                };
            }
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        ControlDisabled?.Invoke();
    }
    #endregion
    #region Input Listeners
    private void OnLaunchPressed()
    {
        // TODO this check is a hack.
        if (((Vector2)followCam.position - (Vector2)transform.position).sqrMagnitude < 500f)
        {
            generator.ResetGeneration();
            Mode = ControlMode.Sledding;
            body.velocity = launchPoint.right * stats[StatType.LaunchSpeed].Value;
        }
    }
    private void OnWingFlapPressed()
    {
        if (staminaSystem.Stamina > staminaUsePerFlap)
        {
            // Exhaust stamina and update velocity.
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
            // Play the wing flap animation.
            animator.Play(flapStateHash, 0, 0f);
        }
    }
    #endregion
}
