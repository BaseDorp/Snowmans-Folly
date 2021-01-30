using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> interactables;

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

    private Vector2 playerLocation;
    private Vector2 lastPlayerSpawnLocation;
    private Vector2 spawnLocation;

    // Start is called before the first frame update
    void Start()
    {
        playerLocation = this.gameObject.transform.position;
        lastPlayerSpawnLocation = playerLocation;
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
            if(RandomizeSpawn())
            {
                Spawn();
            }
            spawnLocation.y += ySpawnStagger;
        }
    }

    public bool RandomizeSpawn()
    {
        int randomInt = Random.Range(0, spawnChance);
        //There's a 1/spawnChance chance that the interactable will spawn. 
        if(randomInt==0)
        {
            //TODO: Implement a method to determine if interactable is beneficial or hazardous
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Spawn()
    {
        if(interactables!=null)
        {
            if(interactables.Count!=0)
            {
                int randomInteractable = Random.Range(0, interactables.Count);
                float xSpawn = spawnLocation.x + (Random.Range(-2, 2));
                float ySpawn = spawnLocation.y + (Random.Range(-1, 1));
                Vector2 randomSpawnLocation = new Vector2(xSpawn, ySpawn);
                if(randomSpawnLocation.y>spawnPlane.transform.position.y)
                {
                    Instantiate(interactables[randomInteractable], randomSpawnLocation, Quaternion.identity);
                }
            }
        }
    }

}
