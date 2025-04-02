using UnityEngine;
using System.Collections.Generic;

public class CameraWallTransparency : MonoBehaviour
{
    public Transform player;
    private List<MeshRenderer> hiddenWalls = new List<MeshRenderer>();

    void Update()
    {
        if (player == null) return;

        RestoreHiddenWalls();

        Vector3 direction = player.position - transform.position;
        Ray ray = new Ray(transform.position, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, direction.magnitude);

        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject == player.gameObject) continue;
            
            if (hitObject.CompareTag("Wall"))
            {
                MeshRenderer meshRenderer = hitObject.GetComponent<MeshRenderer>();
                if (meshRenderer != null && meshRenderer.enabled)
                {
                    meshRenderer.enabled = false;
                    hiddenWalls.Add(meshRenderer);
                }
            }
        }
    }

    void RestoreHiddenWalls()
    {
        foreach (MeshRenderer renderer in hiddenWalls)
        {
            if (renderer != null)
                renderer.enabled = true;
        }
        hiddenWalls.Clear();
    }
}
