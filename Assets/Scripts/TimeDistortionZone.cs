using UnityEngine;

public class TimeDistortionZone : MonoBehaviour
{
    [Header("Time Distortion Settings")]
    public float timeScale = 0.3f; // 0.3 = slow motion, 2.0 = fast forward, -1.0 = reverse
    public float radius = 4f;
    public Color gizmoColor = Color.yellow;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, radius);
        
        // Draw clock-like indicators
        for (int i = 0; i < 12; i++)
        {
            float angle = i * 30f * Mathf.Deg2Rad;
            Vector3 startPos = transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius * 0.7f;
            Vector3 endPos = startPos + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius * 0.3f;
            Gizmos.DrawLine(startPos, endPos);
        }
    }
}
