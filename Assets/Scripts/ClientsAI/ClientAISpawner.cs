using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientAISpawner : MonoBehaviour
{
    public Transform spawnPoint;
    [SerializeField] private GameObject[] spawnObjects;
    public List<ClientAIController> clientAIs;
    private void Start()
    {
        if (spawnPoint == null || spawnObjects.Length == 0)
        {
            Debug.LogWarning("Spawn point or spawn objects are not assigned.");
            return;
        }

        SpawnRandomObject();
    }

    public void SpawnRandomObject()
    {
        int randomIndex = Random.Range(0, spawnObjects.Length);
        GameObject objectToSpawn = spawnObjects[randomIndex];
        GameObject temp = Instantiate(objectToSpawn, spawnPoint.position, new Quaternion(0,180f,0,0));
        clientAIs.Add(temp.GetComponent<ClientAIController>());
    }
}
