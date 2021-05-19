using UnityEngine;

/// <summary>
/// Base class for all objects that react to nearby players.
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    #region Parameters
    private static readonly Color GIZMOS_COLOR = Color.magenta;
    #endregion
    #region Base Inspector Fields
    [Header("Interactable Parameters")]
    [Tooltip("Controls the hitscan radius which the player is searched for in.")]
    [SerializeField] private float radius = 1f;
    protected virtual void OnValidate()
    {
        radius.Clamp(0.01f, float.MaxValue);
        radiusSquared = radius * radius;
    }
    protected virtual void OnDrawGizmosSelected()
    {
        // Draw a representation of the
        // radial hitbox for the designer.
        Gizmos.color = GIZMOS_COLOR;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    #endregion
    #region Private Fields
    // Radius squared is precalculated and stored,
    // this avoids square root calls for distance
    // checks between objects.
    private float radiusSquared;
    // Used to store the current state of all
    // player controllers relative to this "collider".
    private IntersectionState[] intersectionStates;
    private sealed class IntersectionState
    {
        public SnowmanControl player;
        public bool isIntersecting;
    }
    #endregion
    #region Initialization
    protected virtual void Awake()
    {
        if (intersectionStates == null)
            intersectionStates = new IntersectionState[0];
        // Capture the current players, and subscribe
        // to react to future changes in players.
        OnPlayersChanged(PlayerService.Players);
        PlayerService.PlayersChanged += OnPlayersChanged;
        // Precalculate radius squared and initialize
        // intersections to avoid execution order null checks.
        radiusSquared = radius * radius;
    }
    #endregion
    #region Players Changed Listener
    private void OnPlayersChanged(SnowmanControl[] players)
    {
        // Regenerate the intersections state data
        // to reflect the new collection of players.
        IntersectionState[] newStates = new IntersectionState[players.Length];
        int i = 0;
        foreach (SnowmanControl controller in players)
        {
            newStates[i] = new IntersectionState { player = controller };
            // Check to see if this player had any previous state.
            // This ensures adds/drops cannot interrupt a collision state.
            foreach (IntersectionState priorState in intersectionStates)
            {
                if (priorState.player == controller)
                {
                    // Ensure the new array contains the same collision state.
                    newStates[i].isIntersecting = priorState.isIntersecting;
                    break;
                }
            }
            i++;
        }
        intersectionStates = newStates;
    }
    #endregion
    #region Radial Trigger Scanning
    protected virtual void FixedUpdate()
    {
        // Check each registered player.
        foreach (IntersectionState intersection in intersectionStates)
        {
            bool foundHit = false;
            // Check the circle regions on the player.
            foreach (CircleRegion region in intersection.player.HitCircles)
            {
                // If their radii intersect then report a hit.
                if (((Vector2)intersection.player.transform.position + region.position
                    - (Vector2)transform.position).sqrMagnitude
                    < radiusSquared + region.radius * region.radius)
                {
                    foundHit = true;
                    break;
                }
            }
            if (foundHit)
            {
                // If the state has flipped change the stored
                // state and notify the sub-class.
                if (!intersection.isIntersecting)
                {
                    intersection.isIntersecting = true;
                    OnPlayerEnter(intersection.player);
                }
            }
            else
            {
                if (intersection.isIntersecting)
                {
                    intersection.isIntersecting = false;
                    OnPlayerExit(intersection.player);
                }
            }
        }
    }
    #endregion
    private void Update()
    {
        // If the camera has gone past the interactable, destroy it.
        if (Camera.main.WorldToViewportPoint(transform.position).x < 0)
        {
            Destroy(this.gameObject);
        }
    }
    #region Subclass Listeners
    /// <summary>
    /// Called when a player enters the trigger region.
    /// </summary>
    /// <param name="player">The player that entered.</param>
    public abstract void OnPlayerEnter(SnowmanControl player);
    /// <summary>
    /// Called when a player exits the trigger region.
    /// </summary>
    /// <param name="player">The player that exited.</param>
    public virtual void OnPlayerExit(SnowmanControl player) { }
    #endregion
}
