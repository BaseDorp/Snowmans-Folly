using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class WeightedPool<T>
{

    private float totalWeight;
    private Dictionary<T, float> entries;

    public float this[T entry]
    {
        get => entries[entry];
        set
        {
            entries[entry] = Mathf.Clamp(value, 0f, float.MaxValue);
            CalculateTotalWeight();
        }
    }

    public T Next()
    {
        float random = Random.value * totalWeight;
        float weightAccumulator = 0f;
        foreach (KeyValuePair<T, float> entry in entries)
        {
            weightAccumulator += entry.Value;
            if (random < weightAccumulator)
                return entry.Key;
        }
        return default;
    }

    public void AddEntry(T entry, float weight)
    {
        entries.Add(entry, weight);
        CalculateTotalWeight();
    }

    private void CalculateTotalWeight()
    {
        totalWeight = 0f;
        foreach (KeyValuePair<T, float> entry in entries)
            totalWeight += entry.Value;
    }

    public WeightedPool()
    {
        entries = new Dictionary<T, float>();
        totalWeight = 0f;
    }
    public WeightedPool(params KeyValuePair<T, float>[] entries)
    {
        this.entries = new Dictionary<T, float>();

        foreach (KeyValuePair<T, float> entry in entries)
            this.entries.Add(entry.Key, entry.Value);

        CalculateTotalWeight();
    }
}
