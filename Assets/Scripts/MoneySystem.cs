using UnityEngine;

public class MoneySystem : MonoBehaviour
{
    public int currentMoney { get; private set; } = 0;  // Tracks player's money
    public int coinValue = 10; // 1 coin = 10 money

    public static MoneySystem Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Add money (e.g., when collecting a coin)
    public void AddMoney(int amount)
    {
        currentMoney += amount;
        Debug.Log("Money Added: " + amount + " | Total Money: " + currentMoney);
    }

    // Subtract money (e.g., when making a purchase)
    public bool SubtractMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            Debug.Log("Money Deducted: " + amount + " | Total Money: " + currentMoney);
            return true;
        }
        else
        {
            Debug.Log("Not enough money!");
            return false;
        }
    }

    // Get current money
    public int GetCurrentMoney()
    {
        return currentMoney;
    }
}
