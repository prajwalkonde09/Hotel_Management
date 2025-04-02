using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CustomerAI : MonoBehaviour
{
    #region Variables
    private NavMeshAgent agent;
    private Transform assignedRoom = null;
    private Transform targetPosition;
    private Animator animator;

    [Header("Customer Settings")]
    public Transform Endpoint; // Endpoint where the customer will be destroyed
    public float waitTimeInRoom = 5f; // Time to wait in the room

    private bool isInQueue = false;
    private bool roomassigned = false;
    #endregion

    #region Initialization
    public void Initialize()
    {
        Debug.Log("Customer initialized.");
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found.");
        }

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found.");
            return;
        }
        MoveToQueuePosition(); // Move to the first available queue position
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found on customer.");
        }
        else if (!agent.isActiveAndEnabled)
        {
            Debug.LogError("NavMeshAgent is not enabled.");
        }

        if (animator == null)
        {
            Debug.LogError("Animator component not found.");
        }
    }
    #endregion

    #region AI Logic
    private void Update()
    {
        // Update animation based on movement speed
        if (agent.velocity.magnitude > 0.1f)
        {
            animator.SetTrigger("Walk");
        }
        else
        {
            Vector3 direction = agent.velocity.normalized;
            direction.y = 0; // Keep rotation flat on the XZ plane
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
            animator.SetTrigger("Idle");
        }
        // Check if the customer has reached their target
        if (targetPosition != null && Vector3.Distance(transform.position, targetPosition.position) < 0.1f)
        {
            OnReachedTarget();
        }
    }

    public void SetTarget(Transform target)
    {
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent is null on " + gameObject.name);
            return;
        }

        if (target == null)
        {
            Debug.LogError("Target is null for " + gameObject.name);
            return;
        }

        targetPosition = target;
        agent.SetDestination(target.position);
    }
    #endregion

    #region Queue Management
    private void MoveToQueuePosition()
    {
        Transform queueSpot = QueueManager.Instance.GetNextQueueSpot();
        if (queueSpot != null)
        {
            if (QueueManager.Instance.AddCustomerToQueue(this))  // Ensure they are properly enqueued
            {
                //Debug.Log("Queue spot found: " + queueSpot.name);
                SetTarget(queueSpot); // Move to the next queue position
            }
        }
        else
        {
            Debug.LogError("No available queue spot found.");
        }
    }


    public void LeaveQueue()
{

    isInQueue = false;
    QueueManager.Instance.RemoveCustomerFromQueue(this); // Ensure removal from queue
    Transform room = RoomManager.Instance.GetAvailableRoom();

    if (room != null && assignedRoom == null)
    {
        assignedRoom = room;
        SetTarget(room); // Move to the RoomCenter
        RoomManager.Instance.AssignRoom(room);
    }
    else
    {
        Debug.LogError("No available room found.");
    }
}
    #endregion

    #region Room Handling
    private void OnReachedTarget()
    {
        if (targetPosition == null) return;

        if (isInQueue && QueueManager.Instance.IsFirstInQueue(this))
        {
            Debug.Log("Customer is first in queue. Waiting for room assignment.");
            return;
        }

        if (assignedRoom != null && targetPosition == assignedRoom)
        {
            Debug.Log("Customer reached room. Waiting for " + waitTimeInRoom + " seconds.");
            StartCoroutine(WaitInRoom());
        }
        if (targetPosition == Endpoint)
        {
            Debug.Log("Customer reached endpoint. Destroying customer.");
            Destroy(gameObject);
        }

        targetPosition = null; // Reset target position after reaching it
    }


    private IEnumerator WaitInRoom()
    {
        yield return new WaitForSeconds(waitTimeInRoom); // Wait in the room
        RoomManager.Instance.ReleaseRoom(assignedRoom); // Release the room
        SetTarget(Endpoint); // Move to the endpoint
    }
    #endregion
}