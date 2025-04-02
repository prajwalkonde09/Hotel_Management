using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    #region Singleton
    public static QueueManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        if (queuePositions == null || queuePositions.Length == 0)
        {
            Debug.LogError("Queue positions are not assigned.");
            return;
        }

        isPositionOccupied = new bool[queuePositions.Length]; // Initialize occupancy tracking
    }
    #endregion

    #region Variables
    [Header("Queue Settings")]
    public Transform[] queuePositions; // Assign queue positions in Inspector
    public Transform EndPoint;
    private Queue<CustomerAI> customersQueue = new Queue<CustomerAI>(); // Tracks customers in queue
    private bool[] isPositionOccupied; // Track which positions are occupied
    #endregion

    #region Queue Management
    public bool AddCustomerToQueue(CustomerAI customer)
    {
        for (int i = 0; i < queuePositions.Length; i++)
        {
            if (!isPositionOccupied[i]) // Find the first available queue position
            {
                isPositionOccupied[i] = true;
                customersQueue.Enqueue(customer);
                customer.SetTarget(queuePositions[i]); // Move customer to position
                customer.Endpoint = EndPoint;
                Debug.Log($"Customer added to queue at position {i}. Queue count: {customersQueue.Count}");

                if (IsQueueFull()) // Stop spawning when full
                {
                    Debug.Log("Queue is full. Stopping spawning.");
                    CustomerSpawner.Instance.StopSpawning();
                }
                return true;
            }
        }
        Debug.LogError("Queue is full. Cannot add customer.");
        return false;
    }

    public void RemoveCustomerFromQueue(CustomerAI leavingCustomer)
    {
        if (customersQueue.Count == 0) return; // No customers to remove

        List<CustomerAI> tempQueue = new List<CustomerAI>(customersQueue);
        customersQueue.Clear();
        bool found = false;

        for (int i = 0; i < tempQueue.Count; i++)
        {
            if (!found && tempQueue[i] == leavingCustomer)
            {
                found = true; // Mark the customer for removal
                continue;
            }
            customersQueue.Enqueue(tempQueue[i]); // Re-add remaining customers
        }

        // Reset queue position occupancy
        for (int i = 0; i < queuePositions.Length; i++)
        {
            isPositionOccupied[i] = i < customersQueue.Count;
        }

        ShiftQueueForward(); // Move the queue forward
    }


    private void ShiftQueueForward()
    {
        List<CustomerAI> remainingCustomers = new List<CustomerAI>(customersQueue);
        customersQueue.Clear();

        for (int i = 0; i < queuePositions.Length; i++)
        {
            if (i < remainingCustomers.Count)
            {
                customersQueue.Enqueue(remainingCustomers[i]); // Re-enqueue
                remainingCustomers[i].SetTarget(queuePositions[i]); // Move to next position
            }
        }
    }
    #endregion

    #region Utility Methods
    public Transform GetNextQueueSpot()
    {
        for (int i = 0; i < queuePositions.Length; i++)
        {
            if (!isPositionOccupied[i])
            {
                Debug.Log($"Next queue spot found at position {i}.");
                return queuePositions[i]; // Return available queue spot
            }
        }
        Debug.LogError("No available queue spot found.");
        return null;
    }


    public bool IsFirstInQueue(CustomerAI customer)
    {
        return customersQueue.Count > 0 && customersQueue.Peek() == customer;
    }

    public bool IsFirstQueuePositionOccupied()
    {
        return customersQueue.Count > 0;
    }

    public CustomerAI GetFirstCustomer()
    {
        return customersQueue.Count > 0 ? customersQueue.Peek() : null;
    }

    public bool IsQueueFull()
    {
        bool isFull = customersQueue.Count >= queuePositions.Length;
        Debug.Log($"IsQueueFull: {isFull}. Queue count: {customersQueue.Count}, Queue positions: {queuePositions.Length}");
        return isFull;
    }
    #endregion
}