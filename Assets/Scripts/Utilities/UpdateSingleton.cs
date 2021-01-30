using System;
using UnityEngine;

/// <summary>
/// Exposes the unity update loop to non-mono classes.
/// </summary>
public static class UpdateContext
{
    #region Exposed Event Loops
    /// <summary>
    /// Callbacks bound to this will run during the unity update loop.
    /// </summary>
    public static event Action Update
    {
        add
        {
            CheckSingleton();
            monoBehavior.OnUpdate += value;
        }
        remove
        {
            monoBehavior.OnUpdate -= value;
        }
    }
    #endregion
    #region Utility Functions
    private static void CheckSingleton()
    {
        // Create the singleton if it doesn't exist yet.
        if (monoBehavior == null)
        {
            GameObject singletonHost = new GameObject();
            singletonHost.name = "RUNTIME_UPDATE_SINGLETON";
            monoBehavior = singletonHost.AddComponent<UpdateSingleton>();
        }
    }
    #endregion
    #region Singleton Definition
    private static UpdateSingleton monoBehavior;
    // Defines a singular scene instance to watch update that
    // other non-monobehaviours can hook on to.
    private sealed class UpdateSingleton : MonoBehaviour
    {
        public event Action OnUpdate;
        private void Update() { OnUpdate?.Invoke(); }
        private void OnDestroy() { OnUpdate = null; }
    }
    #endregion
}
