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

    [SerializeField]
    private List<GameObject> hazards;

    [SerializeField]
    private List<GameObject> powerups;

    [SerializeField]
    private List<GameObject> coins;

    [Header("Spawn rates, must total 100")]
    [Tooltip("Percentage, max of 100")]
    [SerializeField]
    private int baseHazardSpawnRate;

    [Tooltip("Percentage, max of 100")]
    [SerializeField]
    private int basePowerupSpawnRate;

    [Tooltip("Percentage, max of 100")]
    [SerializeField]
    private int baseCoinSpawnRate;

    private int hazardSpawnRate;
    private int powerupSpawnRate;
    private int coinSpawnRate;

    private Vector2 playerLocation;
    private Vector2 lastPlayerSpawnLocation;
    private Vector2 spawnLocation;

    // Start is called before the first frame update
    void Start()
    {
        playerLocation = this.gameObject.transform.position;
        lastPlayerSpawnLocation = playerLocation;
        hazardSpawnRate = baseHazardSpawnRate;
        powerupSpawnRate = basePowerupSpawnRate;
        coinSpawnRate = baseCoinSpawnRate;
        BalancePercentage();
    }

    private void BalancePercentage()
    {
        while (hazardSpawnRate + powerupSpawnRate + coinSpawnRate > 100)
        {
            if (hazardSpawnRate > 0)
            {
                hazardSpawnRate -= 1;
            }
            else if (powerupSpawnRate > 0)
            {
                powerupSpawnRate -= 1;
            }
            else
            {
                coinSpawnRate -= 1;
            }
        }
        while (hazardSpawnRate + powerupSpawnRate + coinSpawnRate < 100)
        {
            if (hazardSpawnRate < 100)
            {
                hazardSpawnRate += 1;
            }
            else if (powerupSpawnRate < 100)
            {
                powerupSpawnRate += 1;
            }
            else
            {
                coinSpawnRate += 1;
            }
        }
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
        if(playerLocation.x-lastPlayerSpawnLocation.x>xSpawnStagger)
        {
            lastPlayerSpawnLocation = playerLocation;
            SpawnInteractables();
        }
    }
    
    public void SpawnInteractables()
    {
        float xSpawn = playerLocation.x + xSpawnDistance;
        float ySpawn = playerLocation.y - ySpawnDistance;
        spawnLocation = new Vector2(xSpawn, ySpawn);
        while(spawnLocation.y<playerLocation.y+ySpawnDistance)
        {
            RandomizeSpawn();
            spawnLocation.y += ySpawnStagger;
        }
    }

    public void RandomizeSpawn()
    {
        int randomInt = Random.Range(0, spawnChance);
        //There's a 1/spawnChance chance that the interactable will spawn. 
        if(randomInt==0)
        {
            int chooseInteractable = Random.Range(0, 100);
            if(chooseInteractable>=0&&chooseInteractable<hazardSpawnRate)
            {
                Spawn(hazards);
            }
            else if(chooseInteractable>=hazardSpawnRate&&chooseInteractable<hazardSpawnRate+powerupSpawnRate)
            {
                Spawn(powerups);
            }
            else
            {
                Spawn(coins);
            }
        }
    }

    public void Spawn(List<GameObject> interactablesList)
    {
        if(interactablesList!=null)
        {
            if(interactablesList.Count!=0)
            {
                int randomInteractable = Random.Range(0, interactablesList.Count);
                float xSpawn = spawnLocation.x + (Random.Range(-2, 2));
                float ySpawn = spawnLocation.y + (Random.Range(-1, 1));
                Vector2 randomSpawnLocation = new Vector2(xSpawn, ySpawn);
                if(randomSpawnLocation.y>spawnPlane.transform.position.y)
                {
                    Instantiate(interactablesList[randomInteractable], randomSpawnLocation, Quaternion.identity,interactableParent);
                }
            }
        }
    }

    public void ResetInteractables()
    {
        ClearInteractables();
        lastPlayerSpawnLocation = transform.position;

        //As the player improves luck, hazard spawn goes down and powerup spawn goes up
        hazardSpawnRate = baseHazardSpawnRate-(int)stats[StatType.Luck].Value;
        powerupSpawnRate = basePowerupSpawnRate + (int)stats[StatType.Luck].Value;
        coinSpawnRate = baseCoinSpawnRate;

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
