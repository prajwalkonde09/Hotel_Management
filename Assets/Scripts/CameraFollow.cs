using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothSpeed = 10f;

    void LateUpdate()
    {
        if (player == null) return;
        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(player);
    }
}
