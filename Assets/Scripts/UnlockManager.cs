using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class UnlockManager : MonoBehaviour
{
    #region Singleton
    public static UnlockManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    #endregion

    #region Variables
    public List<GameObject> doors; // List of all door objects
    public GameObject winPanel; // Panel to show when all levels are unlocked
    public GameObject nextLevel;

    private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();
    #endregion

    #region Player Detection
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject parentQuad = gameObject; // The quad the player stepped on
            if (!activeCoroutines.ContainsKey(parentQuad))
            {
                Coroutine coroutine = StartCoroutine(StartCountdown(parentQuad));
                activeCoroutines[parentQuad] = coroutine;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject parentQuad = gameObject;
            if (activeCoroutines.ContainsKey(parentQuad))
            {
                StopCoroutine(activeCoroutines[parentQuad]);
                activeCoroutines.Remove(parentQuad);
            }
        }
    }
    #endregion

    #region Unlock Logic
    private IEnumerator StartCountdown(GameObject parentQuad)
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second before starting deduction

        TextMeshPro textMesh = parentQuad.GetComponentInChildren<TextMeshPro>();
        if (textMesh == null) yield break;

        int unlockCost = int.Parse(textMesh.text); // Initial cost from the quad
        int remainingCost = unlockCost;
        int deductionAmount;

        while (remainingCost > 0)
        {
            deductionAmount = Random.Range(5, 10); // Deduct in small amounts

            if (MoneySystem.Instance.GetCurrentMoney() >= deductionAmount)
            {
                MoneySystem.Instance.SubtractMoney(deductionAmount);
                remainingCost -= deductionAmount;
                textMesh.text = Mathf.Max(remainingCost, 0).ToString(); // Update quad text
            }
            else
            {
                Debug.Log("Not enough money to continue unlocking!");
                break;
            }

            yield return new WaitForSeconds(0.1f); // Control deduction speed
        }

        if (remainingCost <= 0)
        {
            if (parentQuad.CompareTag("LevelUnlocker"))
            {
                UnlockNextLevel(parentQuad);
            }

            DisableDoor(parentQuad);
        }
    }
    #endregion

    #region Door Handling
    private void DisableDoor(GameObject parentQuad)
    {
        Transform bedroom = parentQuad.transform.parent?.parent?.parent?.parent; // Get the Bedroom

        if (bedroom != null)
        {
            RoomManager.Instance.AddRoom(bedroom.Find("RoomCenter")); // Add RoomCenter to available rooms
            Debug.Log($"Bedroom '{bedroom.name}' added to available room list.");
        }
        else
        {
            Debug.LogError("Failed to find Bedroom from Quad!");
            return;
        }

        // Disable the "default" (Door)
        Transform door = parentQuad.transform.parent; // "default" is the parent of Quad
        if (door != null)
        {
            door.gameObject.SetActive(false);
            Debug.Log($"Door '{door.name}' disabled.");
        }
        else
        {
            Debug.LogError("Failed to disable door!");
        }

        activeCoroutines.Remove(parentQuad);
    }

    #endregion

    #region Level Unlocking
    private void UnlockNextLevel(GameObject parentQuad)
    {
        if (nextLevel != null)
        {
            nextLevel.SetActive(true); // Enable the next level
            Debug.Log("Next level unlocked!");
        }
        else
        {
            winPanel.SetActive(true);
            Debug.Log("All levels unlocked! Showing Win Panel.");
        }

        // Disable the parent of LevelUnlocker
        Transform levelParent = parentQuad.transform.parent;
        if (levelParent != null)
        {
            levelParent.gameObject.SetActive(false);
            Debug.Log($"Level parent '{levelParent.name}' disabled.");
        }
        else
        {
            Debug.LogError("Failed to disable level parent!");
        }
    }
    #endregion
}
