using System.Collections;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    #region Singleton
    public static CustomerSpawner Instance; // Singleton for easy access

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    #endregion

    #region Variables
    [Header("Customer Settings")]
    public GameObject[] customerPrefabs; // Array of customer prefabs (Assigned in Inspector)
    public Transform spawnPoint; // Fixed spawn position
    public int initialSpawnCount = 5; // Number of customers to spawn initially
    public float spawnInterval = 3f; // Time between spawns

    private bool canSpawn = true; // Controls spawning
    #endregion

    #region Unity Methods
    private void Start()
    {
        StartCoroutine(SpawnRoutine()); // Start automatic spawning
    }
    #endregion

    #region Spawning Logic
    private IEnumerator SpawnRoutine()
    {
        Debug.Log("SpawnRoutine started.");
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            Debug.Log("Attempting to spawn customer...");
            if (canSpawn && !QueueManager.Instance.IsQueueFull())
            {
                SpawnCustomer();
            }
        }
    }

    private void SpawnCustomer()
    {
        if (customerPrefabs.Length == 0)
        {
            Debug.LogError("customerPrefabs array is empty. Assign customer prefabs in the Inspector.");
            return;
        }

        GameObject randomCustomer = Instantiate(customerPrefabs[Random.Range(0, customerPrefabs.Length)], spawnPoint.position, Quaternion.identity);
        CustomerAI customerAI = randomCustomer.GetComponent<CustomerAI>();

        if (customerAI == null)
        {
            Debug.LogError("CustomerAI component not found on spawned customer prefab.");
            Destroy(randomCustomer);
            return;
        }

        customerAI.Initialize(); // Initialize the customer

        bool added = QueueManager.Instance.AddCustomerToQueue(customerAI);
        if (!added) // If queue is full, destroy the spawned customer
        {
            Debug.Log("Queue is full. Destroying spawned customer.");
            Destroy(randomCustomer);
        }
    }
    #endregion

    #region Spawning Control
    public void StopSpawning()
    {
        canSpawn = false;
    }

    public void StartSpawning()
    {
        canSpawn = true;
    }
    #endregion
}
