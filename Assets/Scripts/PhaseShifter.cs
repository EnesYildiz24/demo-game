using UnityEngine;

public class PhaseShifter : MonoBehaviour
{
    [Header("Phase Shift Settings")]
    public float phaseShiftDuration = 3f;
    public float transparency = 0.3f;
    public Color phaseColor = Color.green;
    
    private bool isPhased = false;
    private Collider objectCollider;
    private Renderer objectRenderer;
    private Color originalColor;
    
    void Start()
    {
        objectCollider = GetComponent<Collider>();
        objectRenderer = GetComponent<Renderer>();
        
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPhased)
        {
            StartPhaseShift();
        }
    }
    
    private void StartPhaseShift()
    {
        isPhased = true;
        
        // Make object passable
        if (objectCollider != null)
        {
            objectCollider.isTrigger = true;
        }
        
        // Make object semi-transparent
        if (objectRenderer != null)
        {
            Color phasedColor = originalColor;
            phasedColor.a = transparency;
            objectRenderer.material.color = phasedColor;
        }
        
        // Start coroutine to restore after duration
        StartCoroutine(RestoreAfterDelay());
    }
    
    private System.Collections.IEnumerator RestoreAfterDelay()
    {
        yield return new WaitForSeconds(phaseShiftDuration);
        
        // Restore original state
        isPhased = false;
        
        if (objectCollider != null)
        {
            objectCollider.isTrigger = false;
        }
        
        if (objectRenderer != null)
        {
            objectRenderer.material.color = originalColor;
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = phaseColor;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        
        if (isPhased)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, transform.localScale * 1.1f);
        }
    }
}
