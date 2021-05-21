using System;

// TODO this service should be type generic to allow
// for reuse of any player control type.
// This would require a non-static service instance.

/// <summary>
/// Service for accessing and adding/dropping players.
/// </summary>
public static class PlayerService
{
    #region Static Initialization
    static PlayerService()
    {
        // Eliminates the need for certain null checks.
        Players = new SnowmanControl[0];
    }
    #endregion
    #region Service Events
    /// <summary>
    /// Invoked when the player count changes, passing the updated array of player controllers.
    /// </summary>
    public static event Action<SnowmanControl[]> PlayersChanged;
    #endregion
    #region Player Properties
    /// <summary>
    /// The currently registered player controllers.
    /// </summary>
    public static SnowmanControl[] Players { get; private set; }
    #endregion
    #region Add/Drop Player Methods
    /// <summary>
    /// Adds a new player to the registered players collection.
    /// </summary>
    /// <param name="player">The player controller to add.</param>
    public static void AddPlayer(SnowmanControl player)
    {
        // Check to see if the player has already been added.
        foreach (SnowmanControl existingPlayer in Players)
            if (existingPlayer == player)
                throw new Exception(
                    "Registered a player when it was already registered to the player service.");
        // Insert the new player to the end of the players collection.
        SnowmanControl[] players = Players;
        Array.Resize(ref players, players.Length + 1);
        players[players.Length - 1] = player;
        Players = players;
        // Notify other scripts that a change in player count has occured.
        PlayersChanged?.Invoke(Players);
    }
    /// <summary>
    /// Removes a player from the registered players collection.
    /// </summary>
    /// <param name="player">The player controller to remove.</param>
    public static void DropPlayer(SnowmanControl player)
    {
        // Search for the player to remove.
        int indexToRemove = -1;
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] == player)
            {
                indexToRemove = i;
                break;
            }
        }
        // If the sentinel value is found then the caller tried
        // to remove a player that was never registered.
        if (indexToRemove == -1)
            throw new Exception("Attempted to drop player that was not registered.");
        else
        {
            // Remove the index of the dropped player.
            Players = Players.WithIndexRemoved(indexToRemove);
            // Notify other scripts that a change in player count has occured.
            PlayersChanged?.Invoke(Players);
        }
    }
    #endregion
}
