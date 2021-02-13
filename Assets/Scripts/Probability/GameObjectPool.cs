using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GameObjectPool : MonoBehaviour
{
    [SerializeField] private WeightEntry[] entries;

    [Serializable]
    private sealed class WeightEntry
    {
        public GameObject gameObject;
        public float weight;
    }

    public WeightedPool<GameObject> Retrieve()
    {
        WeightedPool<GameObject> pool = new WeightedPool<GameObject>();
        foreach (WeightEntry entry in entries)
            pool.AddEntry(entry.gameObject, entry.weight);
        return pool;
    }
}
