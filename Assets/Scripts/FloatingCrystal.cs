using UnityEngine;

public class FloatingCrystal : MonoBehaviour
{
    [Header("Floating Animation")]
    public float floatHeight = 0.5f;
    public float floatSpeed = 2f;
    public float rotationSpeed = 30f;
    
    private Vector3 startPosition;
    private float timeOffset;
    
    void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0f, 2f * Mathf.PI); // Random phase offset
    }
    
    void Update()
    {
        // Floating up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed + timeOffset) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        
        // Gentle rotation
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
