using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    [Header("Physics Object Settings")]
    public float mass = 1f;
    public float drag = 0.5f;
    public float angularDrag = 0.5f;
    public PhysicMaterial physicMaterial;
    
    void Start()
    {
        // Ensure object has Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Set physics properties
        rb.mass = mass;
        rb.drag = drag;
        rb.angularDrag = angularDrag;
        
        // Set physics material
        Collider collider = GetComponent<Collider>();
        if (collider != null && physicMaterial != null)
        {
            collider.material = physicMaterial;
        }
        
        // Add visual effects
        AddVisualEffects();
    }
    
    private void AddVisualEffects()
    {
        // Add a subtle glow effect
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Create a simple emission effect
            Material mat = renderer.material;
            if (mat != null)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.white * 0.2f);
            }
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
