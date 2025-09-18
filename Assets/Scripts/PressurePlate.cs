using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Header("Pressure Plate Settings")]
    public float activationWeight = 1f; // Minimum weight to activate
    public float deactivationDelay = 0.1f; // Delay before deactivation
    public bool requirePlayer = false; // Only player can activate
    public bool requireObjects = false; // Only objects can activate
    public bool requireBoth = false; // Both player and objects needed
    
    [Header("Visual Settings")]
    public float pressDepth = 0.1f; // How far the plate sinks
    public Color inactiveColor = Color.gray;
    public Color activeColor = Color.green;
    public float colorTransitionSpeed = 5f;
    
    [Header("Audio Settings")]
    public AudioClip activateSound;
    public AudioClip deactivateSound;
    public float audioVolume = 0.7f;
    
    [Header("Events")]
    public UnityEvent OnActivated;
    public UnityEvent OnDeactivated;
    
    // Private variables
    private bool isActivated = false;
    private float currentWeight = 0f;
    private float deactivationTimer = 0f;
    private Vector3 originalPosition;
    private Vector3 pressedPosition;
    private Renderer plateRenderer;
    private AudioSource audioSource;
    private Color targetColor;
    
    // Objects currently on the plate
    private System.Collections.Generic.List<Collider> objectsOnPlate = new System.Collections.Generic.List<Collider>();
    private bool playerOnPlate = false;
    
    void Start()
    {
        // Store original position
        originalPosition = transform.position;
        pressedPosition = originalPosition - Vector3.up * pressDepth;
        
        // Get renderer
        plateRenderer = GetComponent<Renderer>();
        if (plateRenderer == null)
        {
            plateRenderer = GetComponentInChildren<Renderer>();
        }
        
        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = audioVolume;
        }
        
        // Set initial color
        targetColor = inactiveColor;
        if (plateRenderer != null)
        {
            plateRenderer.material.color = inactiveColor;
        }
    }
    
    void Update()
    {
        CheckActivation();
        UpdateVisuals();
        HandleDeactivation();
    }
    
    void CheckActivation()
    {
        // Calculate current weight
        currentWeight = 0f;
        
        // Add player weight if on plate
        if (playerOnPlate)
        {
            currentWeight += 1f; // Player weight
        }
        
        // Add object weights
        foreach (Collider obj in objectsOnPlate)
        {
            if (obj != null)
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    currentWeight += rb.mass;
                }
                else
                {
                    currentWeight += 0.5f; // Default weight for objects without rigidbody
                }
            }
        }
        
        // Check if activation requirements are met
        bool shouldActivate = false;
        
        if (requireBoth)
        {
            shouldActivate = playerOnPlate && objectsOnPlate.Count > 0 && currentWeight >= activationWeight;
        }
        else if (requirePlayer)
        {
            shouldActivate = playerOnPlate && currentWeight >= activationWeight;
        }
        else if (requireObjects)
        {
            shouldActivate = objectsOnPlate.Count > 0 && currentWeight >= activationWeight;
        }
        else
        {
            shouldActivate = currentWeight >= activationWeight;
        }
        
        // Handle activation state change
        if (shouldActivate && !isActivated)
        {
            Activate();
        }
        else if (!shouldActivate && isActivated)
        {
            deactivationTimer = deactivationDelay;
        }
    }
    
    void Activate()
    {
        isActivated = true;
        targetColor = activeColor;
        
        // Play activation sound
        if (activateSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(activateSound);
        }
        
        // Invoke activation event
        OnActivated?.Invoke();
        
        Debug.Log($"Pressure plate activated! Weight: {currentWeight}");
    }
    
    void Deactivate()
    {
        isActivated = false;
        targetColor = inactiveColor;
        
        // Play deactivation sound
        if (deactivateSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deactivateSound);
        }
        
        // Invoke deactivation event
        OnDeactivated?.Invoke();
        
        Debug.Log("Pressure plate deactivated!");
    }
    
    void HandleDeactivation()
    {
        if (deactivationTimer > 0f)
        {
            deactivationTimer -= Time.deltaTime;
            if (deactivationTimer <= 0f && isActivated)
            {
                Deactivate();
            }
        }
    }
    
    void UpdateVisuals()
    {
        // Update position
        Vector3 targetPosition = isActivated ? pressedPosition : originalPosition;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
        
        // Update color
        if (plateRenderer != null)
        {
            plateRenderer.material.color = Color.Lerp(plateRenderer.material.color, targetColor, Time.deltaTime * colorTransitionSpeed);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if it's the player
        if (other.CompareTag("Player"))
        {
            playerOnPlate = true;
            Debug.Log("Player stepped on pressure plate");
        }
        // Check if it's a grabbable object
        else if (other.GetComponent<Grabbable>() != null || other.GetComponent<Rigidbody>() != null)
        {
            if (!objectsOnPlate.Contains(other))
            {
                objectsOnPlate.Add(other);
                Debug.Log($"Object {other.name} placed on pressure plate");
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // Check if it's the player
        if (other.CompareTag("Player"))
        {
            playerOnPlate = false;
            Debug.Log("Player left pressure plate");
        }
        // Check if it's a grabbable object
        else if (objectsOnPlate.Contains(other))
        {
            objectsOnPlate.Remove(other);
            Debug.Log($"Object {other.name} removed from pressure plate");
        }
    }
    
    // Public methods for external control
    public bool IsActivated()
    {
        return isActivated;
    }
    
    public float GetCurrentWeight()
    {
        return currentWeight;
    }
    
    public int GetObjectCount()
    {
        return objectsOnPlate.Count;
    }
    
    public bool IsPlayerOnPlate()
    {
        return playerOnPlate;
    }
    
    // Method to manually activate/deactivate (for testing or special cases)
    public void SetActivated(bool activated)
    {
        if (activated && !isActivated)
        {
            Activate();
        }
        else if (!activated && isActivated)
        {
            Deactivate();
        }
    }
}
