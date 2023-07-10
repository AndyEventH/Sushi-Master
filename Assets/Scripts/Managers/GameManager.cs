using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private int clientsServed = 0;


    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    instance = obj.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    public int RoundCount
    {
        get { return clientsServed; }
    }

    public event Action<int> OnRoundCountChanged;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void IncreaseRoundCount()
    {
        clientsServed++;
        Debug.Log("Round count increased to: " + clientsServed);
        OnRoundCountChanged?.Invoke(clientsServed);
    }

    public void DecreaseRoundCount()
    {
        clientsServed--;
        Debug.Log("Round count decreased to: " + clientsServed);
        OnRoundCountChanged?.Invoke(clientsServed);
    }
}
