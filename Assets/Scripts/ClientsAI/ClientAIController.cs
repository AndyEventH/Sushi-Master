using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ClientAIController : MonoBehaviour
{
    private SeatManager seatManager;
    private ClientAISpawner spawner;
    public FloatingText messageBox;
    private FoodManager foodManager;
    private Animator anim;
    private PlayerAIController playerAIController;
    public GameObject currentPlate;
    [SerializeField] private int timeToEat = 5;
    private void Start()
    {

        playerAIController = FindObjectOfType<PlayerAIController>();
        anim = GetComponentInChildren<Animator>();
        foodManager = FindObjectOfType<FoodManager>();
        messageBox = GetComponentInChildren<FloatingText>();
        spawner = FindObjectOfType<ClientAISpawner>();
        seatManager = FindObjectOfType<SeatManager>();
        MoveTowardsRandomTarget();
    }

    public void UpdateCurrentPlate(GameObject obj)
    {
        currentPlate = obj;
    }

    private IEnumerator RequestRandomRecipe()
    {
        yield return new WaitForSeconds(1);
        Recipe recipe = foodManager.GetRandomRecipe();
        string originalString = recipe.foodName.ToString();
        string formattedString = Regex.Replace(originalString, @"((?<!^)[A-Z])", " $1");
        messageBox.GenerateMessageBox(formattedString);
        playerAIController.AddToPendingOrders(recipe,gameObject.GetComponent<ClientAIController>());
    }
    private IEnumerator WaitDestReachedTeleport()
    {
        float destinationThreshold = 0.01f; // Adjust the threshold value to fit your needs

        NavMeshAgent agent = gameObject.GetComponent<NavMeshAgent>();

        while (Vector3.Distance(agent.transform.position, agent.destination) > destinationThreshold)
        {
            yield return null;
        }

        if (agent != null)
        {
            Debug.Log("Reached");
            TeleportToLocation();
        }

    }
    private Transform currentSeat;
    public void MoveTowardsRandomTarget()
    {
        currentSeat = seatManager.ReserveRandomAvailableSeat();
        if (currentSeat == null)
        {
            Debug.Log("No available target positions.");
            return;
        }

        NavMeshAgent agent = gameObject.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            //Debug.Log(randomTarget.position);
            agent.SetDestination(currentSeat.position);
            anim.SetBool("isWalking", true);
            StartCoroutine(WaitDestReachedTeleport());
        }
        else
        {
            Debug.LogWarning("NavMeshAgent component not found on the object.");
        }
    }

    public IEnumerator StartEating()
    {
        anim.SetBool("isEating", true);
        yield return new WaitForSeconds(timeToEat);
        DisableAllChildrenOfPlate(currentPlate.transform);
        anim.SetBool("isEating", false);
        playerAIController.eatingCoroutine = null;
        NavMeshAgent agent = gameObject.GetComponent<NavMeshAgent>();
        agent.enabled = true;
        MoveTowardsExit();
    }


    private void DisableAllChildrenOfPlate(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            child.gameObject.SetActive(false);

            // Recursively disable children of the child
            //DisableAllChildren(child);
        }
    }

    public void MoveTowardsExit()
    {

        NavMeshAgent agent = gameObject.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            anim.SetBool("isSitting", false);
            anim.SetBool("isWalking", true);
            agent.SetDestination(spawner.spawnPoint.position);  
            StartCoroutine(WaitDestReachedToDestroy());
        }
        else
        {
            Debug.LogWarning("NavMeshAgent component not found on the object.");
        }
    }

    private IEnumerator WaitDestReachedToDestroy()
    {
        float destinationThreshold = 0.01f; // Adjust the threshold value to fit your needs

        NavMeshAgent agent = gameObject.GetComponent<NavMeshAgent>();

        while (Vector3.Distance(agent.transform.position, agent.destination) > destinationThreshold)
        {
            yield return null;
        }

        if (agent != null)
        {
            seatManager.ReleaseSeat(currentSeat);
            Destroy(gameObject);
        }

    }

    private IEnumerator WaitForAvailableSeat()
    {
        while (!seatManager.IsAnySeatAvailable())
        {
           // Debug.Log("Waiting for avaialable seat");
            yield return null;
        }
        spawner.SpawnRandomObject();
    }
    public void TeleportToLocation()
    {
        NavMeshAgent agent = gameObject.GetComponent<NavMeshAgent>();

        agent.enabled = false;
        anim.SetBool("isWalking", false);
        anim.SetBool("isSitting", true);
        PositionRotation teleportTo= seatManager.FindNearestPositionAndGetDish(transform.position, gameObject.GetComponent<ClientAIController>());
        transform.position = teleportTo.position;
        transform.rotation = Quaternion.Euler(teleportTo.rotation);
        StartCoroutine(RequestRandomRecipe());
        if (spawner != null)
        {
            if (seatManager.IsAnySeatAvailable())
            {
                spawner.SpawnRandomObject();
            }
            else
            {
                StartCoroutine(WaitForAvailableSeat());
            }

        }
        
    }
}
