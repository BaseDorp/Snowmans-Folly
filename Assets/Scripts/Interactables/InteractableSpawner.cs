using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSpawner : MonoBehaviour
{

    [Tooltip("The stats for this snowman.")]
    [SerializeField]
    private StatProfile stats;

    [Tooltip("How far to the right of the player can interactables spawn.")]
    [SerializeField]
    private float xSpawnDistance;

    [Tooltip("How far above and below the player can interactables spawn.")]
    [SerializeField]
    private float ySpawnDistance;

    [Tooltip("How far apart on the y axis interactables can spawn at.")]
    [SerializeField]
    private float ySpawnStagger;

    [Tooltip("How far apart on the x axis interactables can spawn at.")]
    [SerializeField]
    private float xSpawnStagger;

    [Tooltip("1/spawnChance chance to spawn an interactable at any given location. (Higher means less likely)")]
    [SerializeField]
    private int spawnChance;

    [Tooltip("The marker used to determine the lowest the interactables can spawn at.")]
    [SerializeField]
    private GameObject spawnPlane;

    [Tooltip("The parent component for the instantiated interactables.")]
    [SerializeField]
    private Transform interactableParent;

    [Header("Spawn Rate Parameters")]

    [SerializeField]
    private GameObjectPool spawnablesPool = null;
    private WeightedPool<GameObject> pool;

    [SerializeField]
    private LuckWeightFactor[] luckEffects = null;
    [Serializable]
    private sealed class LuckWeightFactor
    {
        public GameObject weightToAugment;
        public float addedWeightPerLuck;
    }

    private Vector2 playerLocation;
    private Vector2 lastPlayerSpawnLocation;
    private Vector2 spawnLocation;

    // Start is called before the first frame update
    void Start()
    {
        playerLocation = this.gameObject.transform.position;
        lastPlayerSpawnLocation = playerLocation;
    }

    private void OnEnable()
    {
        SnowmanControl.Launched += ResetInteractables;
    }

    // Update is called once per frame
    void Update()
    {
        playerLocation = this.gameObject.transform.position;
        //Consider decreasing the spawn stagger based on velocity
        if(playerLocation.x>lastPlayerSpawnLocation.x)
        {
            SpawnInteractables();
        }
    }
    
    public void SpawnInteractables()
    {
        for (float furtherX = 0; furtherX < playerLocation.x + xSpawnDistance * 10; furtherX += xSpawnDistance)
        {
            float xSpawn = playerLocation.x + xSpawnDistance+furtherX;
            float ySpawn = playerLocation.y - ySpawnDistance;
            spawnLocation = new Vector2(xSpawn, ySpawn);
            while (spawnLocation.y < playerLocation.y + ySpawnDistance)
            {
                Spawn();
                spawnLocation.y += ySpawnStagger;
            }
            lastPlayerSpawnLocation = new Vector2(xSpawn,playerLocation.y);
        }
    }

    public void Spawn()
    {
        float xSpawn = spawnLocation.x + (UnityEngine.Random.Range(-2, 2));
        float ySpawn = spawnLocation.y + (UnityEngine.Random.Range(-1, 1));
        Vector2 randomSpawnLocation = new Vector2(xSpawn, ySpawn);
        if(randomSpawnLocation.y>spawnPlane.transform.position.y)
        {
            GameObject poolNext = pool.Next();
            if (poolNext != null)
                Instantiate(poolNext, randomSpawnLocation, Quaternion.identity, interactableParent);
        }
    }

    public void ResetInteractables()
    {
        ClearInteractables();
        spawnLocation = Vector2.zero;
        lastPlayerSpawnLocation = Vector2.zero;

        //As the player improves luck, hazard spawn goes down and powerup spawn goes up
        pool = spawnablesPool.Retrieve();
        foreach (LuckWeightFactor effect in luckEffects)
        {
            pool[effect.weightToAugment] += stats[StatType.Luck].Value * effect.addedWeightPerLuck;
        }

        SpawnInteractables();
    }

    public void ClearInteractables()
    {
        GameObject[] spawnedInteractables = new GameObject[interactableParent.childCount];
        int x = 0;
        foreach(Transform interactable in interactableParent)
        {
            spawnedInteractables[x] = interactable.gameObject;
            x++;
        }
        foreach(GameObject interactable in spawnedInteractables)
        {
            Destroy(interactable);
        }
    }
}
