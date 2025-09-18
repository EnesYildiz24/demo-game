using UnityEngine;

public class GravityRotator : MonoBehaviour
{
    [Header("Gravity Rotation Settings")]
    public Vector3 gravityDirection = Vector3.down;
    public float rotationSpeed = 2f;
    public float radius = 6f;
    public Color gizmoColor = Color.red;
    
    private void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody != null)
        {
            // Apply custom gravity direction
            other.attachedRigidbody.AddForce(gravityDirection * Physics.gravity.magnitude, ForceMode.Force);
        }
    }
    
    private void Update()
    {
        // Rotate gravity direction over time
        gravityDirection = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up) * gravityDirection;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, radius);
        
        // Draw gravity direction arrow
        Gizmos.color = Color.red;
        Vector3 arrowStart = transform.position;
        Vector3 arrowEnd = arrowStart + gravityDirection * radius;
        Gizmos.DrawLine(arrowStart, arrowEnd);
        
        // Draw arrow head
        Vector3 right = Vector3.Cross(gravityDirection, Vector3.up).normalized * 0.5f;
        Vector3 left = -right;
        Vector3 arrowHead = arrowEnd - gravityDirection * 0.5f;
        
        Gizmos.DrawLine(arrowEnd, arrowHead + right);
        Gizmos.DrawLine(arrowEnd, arrowHead + left);
    }
}
