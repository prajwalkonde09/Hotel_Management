using UnityEngine;
using System.Collections;

public class ReceptionManager : MonoBehaviour
{
    #region Singleton
    public static ReceptionManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    #endregion

    #region Variables
    private bool isPlayerAtReception = false;
    private Coroutine processingCoroutine = null;
    #endregion

    #region Player Detection
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerAtReception = true;
            if (processingCoroutine == null)
                processingCoroutine = StartCoroutine(ProcessCustomers());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerAtReception = false;
            if (processingCoroutine != null)
            {
                StopCoroutine(processingCoroutine);
                processingCoroutine = null;
            }
        }
    }
    #endregion

    #region Customer Processing
    private IEnumerator ProcessCustomers()
    {
        while (isPlayerAtReception)
        {
            yield return new WaitForSeconds(1.5f); // Wait before processing

            if (QueueManager.Instance.IsFirstQueuePositionOccupied())
            {
                CustomerAI firstCustomer = QueueManager.Instance.GetFirstCustomer();
                if (firstCustomer != null)
                {
                    firstCustomer.LeaveQueue();
                    yield return new WaitForSeconds(0.5f); // Small delay before next check
                }
            }
        }
        processingCoroutine = null;
    }
    #endregion
}