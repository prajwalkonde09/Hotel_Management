using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Ensure the player has the "Player" tag
        {
            MoneySystem.Instance.AddMoney(MoneySystem.Instance.coinValue);
            Destroy(gameObject); // Destroy coin after collection
        }
    }
}
