using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// A unity scene instance for a weighted object pool.
/// </summary>
public sealed class WeightedGameObjectPool : MonoBehaviour
{
    #region Inspector Fields
    [Tooltip("The entries of the weighted table.")]
    [SerializeField] private WeightEntry[] entries = default;
    #endregion
    #region Inspector POCOs
    [Serializable]
    private sealed class WeightEntry
    {
        [HideInInspector] public string name;
        [Tooltip("The GameObject associated with this weight entry.")]
        public GameObject gameObject = default;
        [Tooltip("The relative weight of this GameObject.")]
        public float weight = 0f;
    }
    #endregion
#if UNITY_EDITOR
    #region Inspector Validation
    private void OnValidate()
    {
        if (!(entries is null))
        {
            // Calculate the total weight.
            float totalWeight = 0f;
            foreach (WeightEntry entry in entries)
                if (!(entry.gameObject is null))
                    totalWeight += entry.weight;
            // Prevent division by zero designer UX.
            if (totalWeight is 0f)
                totalWeight = 1f;
            // Validate the data.
            for (int i = 0; i < entries.Length; i++)
            {
                // Keep weights above 0.
                entries[i].weight = Mathf.Max(0f, entries[i].weight);
                // Label the pool objects.
                if (entries[i].gameObject is null)
                    entries[i].name = "Null Element";
                else
                    entries[i].name = $"{ObjectNames.NicifyVariableName(entries[i].gameObject.name)} - {entries[i].weight:0.##} ({(entries[i].weight / totalWeight * 100f):#0.##}%)";
            }
        }
    }
    #endregion
    #region Runtime Validation
    private void Start()
    {
        // Check for an edge case where a null key causes a web assembly exception.
        foreach (WeightEntry entry in entries)
            if (entry.gameObject is null)
                Debug.LogWarning(
                    "Weighted entries cannot contain an empty GameObject!" +
                    "Null entries will be ignored, please remove them from the entry list.",
                    gameObject);
    }
    #endregion
#endif
    #region Weighted Pool Accessor
    /// <summary>
    /// Generates the weighted pool from the scene instance.
    /// Changing this weighted pool will not effect future
    /// requests to this object, as a new pool object is
    /// generated each time this is called.
    /// </summary>
    /// <returns>The weighted pool.</returns>
    public WeightedPool<GameObject> Retrieve()
    {
        WeightedPool<GameObject> pool = new WeightedPool<GameObject>();
        // Populate the pool.
        if (!(entries is null))
            foreach (WeightEntry entry in entries)
                if (!(entry.gameObject is null))
                    pool.AddEntry(entry.gameObject, entry.weight);
        return pool;
    }
    #endregion
}
