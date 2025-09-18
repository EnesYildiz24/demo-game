using UnityEngine;

public class AntiGravityZone : MonoBehaviour
{
    [Header("Anti-Gravity Settings")]
    public float antiGravityForce = 15f;
    public float radius = 5f;
    public Color gizmoColor = Color.cyan;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            // Apply upward force instead of downward gravity
            Vector3 forceDirection = Vector3.up;
            float distance = Vector3.Distance(transform.position, other.transform.position);
            float forceMultiplier = Mathf.Lerp(1f, 0f, distance / radius);
            
            other.attachedRigidbody.AddForce(forceDirection * antiGravityForce * forceMultiplier, ForceMode.Force);
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, radius);
        
        // Draw upward arrows to show anti-gravity direction
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f * Mathf.Deg2Rad;
            Vector3 startPos = transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius * 0.8f;
            Vector3 endPos = startPos + Vector3.up * 2f;
            Gizmos.DrawLine(startPos, endPos);
        }
    }
}
