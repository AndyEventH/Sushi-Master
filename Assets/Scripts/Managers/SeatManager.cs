using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public class SeatManager : MonoBehaviour
{
    private Dictionary<Transform, bool> seatAvailability = new Dictionary<Transform, bool>();
    private Transform[] seats;
    [SerializeField]private GameObject[] plates;

    [SerializeField] private List<Vector3> allSeatsPosition;
    private void Awake()
    {
        // Iterate through each vector position
        seats = GetComponentsInChildren<Transform>();
        InitializeSeatAvailability();
    }

    private void Start()
    {
        for (int i = 1; i < seats.Length; i++)
        {
            allSeatsPosition.Add(seats[i].position);
        }
    }

    private void InitializeSeatAvailability()
    {
        //Transform[] seats = GetComponentsInChildren<Transform>();

        foreach (Transform seat in seats)
        {
            if (seat != transform) // Exclude the parent transform
            {
                seatAvailability.Add(seat, true);
            }
        }
    }

    public bool IsSeatAvailable(Transform seat)
    {
        return seatAvailability.ContainsKey(seat) && seatAvailability[seat];
    }

    public bool ReserveSeat(Transform seat)
    {
        if (IsSeatAvailable(seat))
        {
            seatAvailability[seat] = false;
            return true;
        }

        return false;
    }

    public bool ReleaseSeat(Transform seat)
    {
        if (seatAvailability.ContainsKey(seat))
        {
            seatAvailability[seat] = true;
            return true;
        }

        return false;
    }

    public Transform ReserveRandomAvailableSeat()
    {
        List<Transform> availableSeats = GetAvailableSeats();

        if (availableSeats.Count == 0)
        {
            Debug.Log("No available seats to reserve.");
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, availableSeats.Count);
        Transform randomSeat = availableSeats[randomIndex];
        
        
        if (ReserveSeat(randomSeat))
        {
            return randomSeat;
        }
        return null;
    }



    public List<Transform> GetAvailableSeats()
    {
        List<Transform> availableSeats = new List<Transform>();

        foreach (KeyValuePair<Transform, bool> seat in seatAvailability)
        {
            if (seat.Value)
            {
                availableSeats.Add(seat.Key);
            }
        }

        return availableSeats;
    }


    [SerializeField] private Vector3[] actualSeatPosition; 
    [SerializeField] private Vector3[] actualSeatRotation;

    public PositionRotation FindNearestPositionAndGetDish(Vector3 comparePosition, ClientAIController clientAI)
    {
        Vector3 nearestPosition = Vector3.zero;
        Vector3 nearestRotation = Vector3.zero;
        float nearestDistance = Mathf.Infinity;

        int tempCounter = 0;

        

        foreach (Vector3 position in actualSeatPosition)
        {
            float distance = Vector3.Distance(position, comparePosition);
            //Debug.Log(position + "   " + distance);
            // Check if the distance is smaller than the current nearest distance
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPosition = actualSeatPosition[tempCounter];
                nearestRotation = actualSeatRotation[tempCounter];
                clientAI.UpdateCurrentPlate(plates[tempCounter]);
                Debug.Log(plates[tempCounter].name);
            }
            tempCounter++;
        }

        PositionRotation result;
        result.position = nearestPosition;
        result.rotation = nearestRotation;
        return result;
    }


    public bool IsAnySeatAvailable()
    {
        List<Transform> availableSeats = GetAvailableSeats();
        return availableSeats.Count > 0;
    }

}
