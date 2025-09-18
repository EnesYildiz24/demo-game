using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Portal Settings")]
    public Portal linkedPortal;
    public Transform exitPoint;
    public Color portalColor = Color.magenta;
    public float cooldownTime = 1f;
    
    private float lastTeleportTime;
    
    private void OnTriggerEnter(Collider other)
    {
        if (linkedPortal != null && Time.time - lastTeleportTime > cooldownTime)
        {
            TeleportObject(other.gameObject);
        }
    }
    
    private void TeleportObject(GameObject obj)
    {
        if (obj.CompareTag("Player"))
        {
            // Teleport player
            obj.transform.position = linkedPortal.exitPoint.position;
            obj.transform.rotation = linkedPortal.exitPoint.rotation;
            
            // Add small forward momentum to prevent getting stuck
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = linkedPortal.exitPoint.forward * 5f;
            }
        }
        else if (obj.GetComponent<Rigidbody>() != null)
        {
            // Teleport physics objects
            obj.transform.position = linkedPortal.exitPoint.position;
            obj.transform.rotation = linkedPortal.exitPoint.rotation;
            
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            rb.velocity = linkedPortal.exitPoint.forward * 3f;
        }
        
        lastTeleportTime = Time.time;
        linkedPortal.lastTeleportTime = Time.time;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = portalColor;
        Gizmos.DrawWireSphere(transform.position, 1f);
        
        if (linkedPortal != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, linkedPortal.transform.position);
        }
        
        if (exitPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(exitPoint.position, Vector3.one * 0.5f);
            Gizmos.DrawRay(exitPoint.position, exitPoint.forward * 2f);
        }
    }
}
