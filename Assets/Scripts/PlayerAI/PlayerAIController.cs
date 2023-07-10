using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public struct PendingOrders
{
    public Recipe recipe;
    public GameObject plate;
    public ClientAIController client;
}

public class PlayerAIController : MonoBehaviour
{
    [SerializeField] private ClientAISpawner clientAISpawner;
    private List<PendingOrders> pendingOrders;
    private int currentOrderIndex = 0;
    private NavMeshAgent agent;
    private bool isHandlingOrder = false;
    [SerializeField] int timeToPrepare = 5;
    [SerializeField] private Transform kitchenPreparePosition;
    [SerializeField] private Transform refillIngredientPosition;
    [SerializeField] private Transform InitialPosition;
    [SerializeField] private Transform SleepPosition;
    [SerializeField] private int timeToSleep= 15;
    [SerializeField] private FoodManager foodManager;
    [SerializeField] private CameraVisibilityController cameraVisibilityController;
    private Animator anim;
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();

        // Initialize the list of pending orders
        pendingOrders = new List<PendingOrders>();

        // Get a reference to the NavMeshAgent component attached to this GameObject
        agent = GetComponent<NavMeshAgent>();
    }
    private Coroutine handleOrder;
    bool hasBeenDoneSleep;
    private void Update()
    {
        // Check if an order is not currently being handled and there are pending orders available
        if(GameManager.Instance.RoundCount != 0 && GameManager.Instance.RoundCount%10 == 0 && goSleep == null && !hasBeenDoneSleep)
        {
            hasBeenDoneSleep = true;
            goSleep = StartCoroutine(GoSleep());
        }
        else if (!isHandlingOrder && pendingOrders.Count > 0 && handleRefill == null && goSleep == null)
        {
            // Start handling the next pending order
            handleOrder = StartCoroutine(HandleOrder(pendingOrders[currentOrderIndex]));

            // Move to the next order index, wrapping around if necessary
            currentOrderIndex = (currentOrderIndex + 1) % pendingOrders.Count;
        }

        /*
        else if(!isHandlingOrder && pendingOrders.Count > 0 && needsResources)
        {
            StartCoroutine(HandleRefill());
        }*/
    }

    private Coroutine goSleep;
    public IEnumerator GoSleep()
    {
        // Set the destination of the NavMeshAgent to the position of the order's GameObject
        anim.SetBool("isWalking", true);
        agent.SetDestination(SleepPosition.position);

        // Wait until the path to the destination is computed
        while (agent.pathPending)
        {
            yield return null;
        }

        // Flag to indicate if the halfway point has been reached
        bool halfwayReached = false;

        // Wait until the agent reaches the destination
        while (agent.remainingDistance > agent.stoppingDistance)
        {
            // Check if the agent is at the halfway point
            float totalDistance = Vector3.Distance(agent.transform.position, kitchenPreparePosition.position);
            float currentDistance = Vector3.Distance(agent.transform.position, agent.destination);

            if (currentDistance <= totalDistance * 0.5f && !halfwayReached)
            {
                // Agent is at the halfway point
                halfwayReached = true;
                cameraVisibilityController.EnableKitchenCamera();
            }

            yield return null;
        }

        agent.enabled = false;
        anim.SetBool("isWalking", false);
        transform.Rotate(0f, 180f, 0f);
        anim.SetBool("isSleeping", true);
        yield return new WaitForSeconds(timeToSleep);
        agent.enabled = true;
        anim.SetBool("isSleeping", false);

        anim.SetBool("isWalking", true);

        agent.SetDestination(InitialPosition.position);

        // Wait until the path to the starting point is computed
        while (agent.pathPending)
        {
            yield return null;
        }

        // Flag to indicate if the halfway point has been reached
        halfwayReached = false;

        // Wait until the agent reaches the destination
        while (agent.remainingDistance > agent.stoppingDistance)
        {
            // Check if the agent is at the halfway point
            float totalDistance = Vector3.Distance(agent.transform.position, kitchenPreparePosition.position);
            float currentDistance = Vector3.Distance(agent.transform.position, agent.destination);

            if (currentDistance <= totalDistance * 0.5f && !halfwayReached)
            {
                // Agent is at the halfway point
                halfwayReached = true;
                cameraVisibilityController.DisableSecondaryCameras();
            }

            yield return null;
        }

        anim.SetBool("isWalking", false);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        // Reset the flag to indicate that the order handling is complete


        yield return new WaitForSeconds(1f);
        goSleep = null;


    }

    public void AddToPendingOrders(Recipe recipe, ClientAIController client)
    {
        // Create a new PendingOrders struct and add it to the list of pending orders
        PendingOrders newOrder;
        newOrder.recipe = recipe;
        newOrder.plate = client.currentPlate;
        newOrder.client = client;

        pendingOrders.Add(newOrder);
    }


    private Coroutine assemblingTimed;



    private void EnableRequestedFood(PendingOrders order)
    {
        foreach (Transform child in order.plate.transform)
        {
            GameObject obj = child.gameObject;

            if (order.recipe.foodName.ToString() == obj.name)
            {
                obj.SetActive(true);
                break;
            }
            Debug.Log(order.recipe.foodName.ToString() + " " + obj.name);
        }
    }
    /*
    private void EnableRequestedFood(PendingOrders order)
    {
        foreach (var component in order.plate.GetComponentsInChildren<Component>())
        {
            if (order.recipe.foodName.ToString() == component.gameObject.name)
            {
                component.gameObject.SetActive(true);
                Debug.Log("reachedddd");
                break;
            }
            Debug.Log(order.recipe.foodName.ToString() + " " + component.gameObject.name);
        }
    }*/
    private Coroutine handleRefill;
    private Coroutine assemblingTimedRefill;
    private IEnumerator HandleRefill(PendingOrders order)
    {
        // Set the destination of the NavMeshAgent to the position of the order's GameObject
        anim.SetBool("isWalking", true);
        agent.SetDestination(refillIngredientPosition.position);

        // Wait until the path to the destination is computed
        while (agent.pathPending)
        {
            yield return null;
        }

        // Flag to indicate if the halfway point has been reached
        bool halfwayReached = false;

        // Wait until the agent reaches the destination
        while (agent.remainingDistance > agent.stoppingDistance)
        {
            // Check if the agent is at the halfway point
            float totalDistance = Vector3.Distance(agent.transform.position, kitchenPreparePosition.position);
            float currentDistance = Vector3.Distance(agent.transform.position, agent.destination);

            if (currentDistance <= totalDistance * 0.5f && !halfwayReached)
            {
                // Agent is at the halfway point
                halfwayReached = true;
                cameraVisibilityController.EnableDepositCamera();
            }

            yield return null;
        }

        
        anim.SetBool("isWalking", false);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        assemblingTimedRefill = StartCoroutine(AssemblingTimedRefill());

        while (assemblingTimedRefill != null)
        {
            yield return null;
        }

        foodManager.RefillIngredients();
        anim.SetBool("isWalking", true);

        agent.SetDestination(InitialPosition.position);

        // Wait until the path to the starting point is computed
        while (agent.pathPending)
        {
            yield return null;
        }

        // Flag to indicate if the halfway point has been reached
        halfwayReached = false;

        // Wait until the agent reaches the destination
        while (agent.remainingDistance > agent.stoppingDistance)
        {
            // Check if the agent is at the halfway point
            float totalDistance = Vector3.Distance(agent.transform.position, kitchenPreparePosition.position);
            float currentDistance = Vector3.Distance(agent.transform.position, agent.destination);

            if (currentDistance <= totalDistance * 0.5f && !halfwayReached)
            {
                // Agent is at the halfway point
                halfwayReached = true;
                cameraVisibilityController.DisableSecondaryCameras();
            }

            yield return null;
        }

        anim.SetBool("isWalking", false);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        // Reset the flag to indicate that the order handling is complete


        yield return new WaitForSeconds(1f);
        handleRefill = null;
        StopCoroutine(handleOrder);
        handleOrder=StartCoroutine(HandleOrder(order));
    }
    private IEnumerator HandleOrder(PendingOrders order)
    {
        // Set the flag to indicate that an order is being handled
        isHandlingOrder = true;
        // Set the destination of the NavMeshAgent to the position of the order's GameObject
        anim.SetBool("isWalking", true);
        agent.SetDestination(kitchenPreparePosition.position);

        // Wait until the path to the destination is computed
        while (agent.pathPending)
        {
            yield return null;
        }

        // Flag to indicate if the halfway point has been reached
        bool halfwayReached = false;

        // Wait until the agent reaches the destination
        while (agent.remainingDistance > agent.stoppingDistance)
        {
            // Check if the agent is at the halfway point
            float totalDistance = Vector3.Distance(agent.transform.position, kitchenPreparePosition.position);
            float currentDistance = Vector3.Distance(agent.transform.position, agent.destination);

            if (currentDistance <= totalDistance * 0.5f && !halfwayReached)
            {
                // Agent is at the halfway point
                halfwayReached = true;
                cameraVisibilityController.EnableKitchenCamera();
            }

            yield return null;
        }

        anim.SetBool("isWalking", false);

        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        assemblingTimed = StartCoroutine(AssemblingTimed());

        while (assemblingTimed != null)
        {
            yield return null;
        }

        // Return to the starting point
        agent.SetDestination(InitialPosition.position);
        FoodMakerReturns ret= foodManager.MakeFood(order.recipe.foodName);
        if(ret == FoodMakerReturns.InsufficientQuantity) {
            handleRefill = StartCoroutine(HandleRefill(order));
            yield break;
        }
        anim.SetBool("isWalking", true);

        // Wait until the path to the starting point is computed
        while (agent.pathPending)
        {
            yield return null;
        }

        // Flag to indicate if the halfway point has been reached
        halfwayReached = false;

        // Wait until the agent reaches the destination
        while (agent.remainingDistance > agent.stoppingDistance)
        {
            // Check if the agent is at the halfway point
            float totalDistance = Vector3.Distance(agent.transform.position, kitchenPreparePosition.position);
            float currentDistance = Vector3.Distance(agent.transform.position, agent.destination);

            if (currentDistance <= totalDistance * 0.5f && !halfwayReached)
            {
                // Agent is at the halfway point
                halfwayReached = true;
                cameraVisibilityController.DisableSecondaryCameras();
            }

            yield return null;
        }

        anim.SetBool("isWalking", false);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        // Reset the flag to indicate that the order handling is complete
        EnableRequestedFood(order);
        order.client.messageBox.DisableMessageBox();
        GameManager.Instance.IncreaseRoundCount();
        hasBeenDoneSleep = false;
        eatingCoroutine =StartCoroutine(order.client.StartEating());
        pendingOrders.Remove(order);
        yield return new WaitForSeconds(1f);
        handleOrder = null;
        isHandlingOrder = false;
    }
    public Coroutine eatingCoroutine;
    private IEnumerator AssemblingTimedRefill()
    {
        anim.SetBool("isAssembling",true);
        yield return new WaitForSeconds(timeToPrepare);
        anim.SetBool("isAssembling",false);
        assemblingTimedRefill = null;
    }

    private IEnumerator AssemblingTimed()
    {
        anim.SetBool("isAssembling", true);
        yield return new WaitForSeconds(timeToPrepare);
        anim.SetBool("isAssembling", false);
        assemblingTimed = null;
    }

}
