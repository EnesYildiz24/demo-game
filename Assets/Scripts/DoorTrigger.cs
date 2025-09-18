using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Header("Door Settings")]
    public Door targetDoor;
    public bool openOnActivation = true;
    public bool closeOnDeactivation = true;
    public float delay = 0f; // Delay before opening/closing
    
    [Header("Multiple Triggers")]
    public PressurePlate[] requiredPlates = new PressurePlate[0]; // All plates must be activated
    public bool requireAllPlates = true; // If false, any plate can trigger
    
    [Header("Visual Feedback")]
    public GameObject indicatorLight; // Optional light to show state
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.red;
    
    private bool isTriggered = false;
    private Renderer indicatorRenderer;
    
    void Start()
    {
        // Find target door if not assigned
        if (targetDoor == null)
        {
            targetDoor = FindObjectOfType<Door>();
        }
        
        // Setup indicator light
        if (indicatorLight != null)
        {
            indicatorRenderer = indicatorLight.GetComponent<Renderer>();
            if (indicatorRenderer == null)
            {
                indicatorRenderer = indicatorLight.GetComponentInChildren<Renderer>();
            }
        }
        
        // Initialize requiredPlates array if null
        if (requiredPlates == null)
        {
            requiredPlates = new PressurePlate[0];
        }
        
        // Subscribe to pressure plate events
        foreach (PressurePlate plate in requiredPlates)
        {
            if (plate != null)
            {
                plate.OnActivated.AddListener(CheckTrigger);
                plate.OnDeactivated.AddListener(CheckTrigger);
            }
        }
        
        UpdateIndicator();
    }
    
    void CheckTrigger()
    {
        bool shouldTrigger = false;
        
        // Safety check for requiredPlates array
        if (requiredPlates == null || requiredPlates.Length == 0)
        {
            shouldTrigger = false;
        }
        else if (requireAllPlates)
        {
            // All plates must be activated
            shouldTrigger = true;
            foreach (PressurePlate plate in requiredPlates)
            {
                if (plate == null || !plate.IsActivated())
                {
                    shouldTrigger = false;
                    break;
                }
            }
        }
        else
        {
            // Any plate can trigger
            shouldTrigger = false;
            foreach (PressurePlate plate in requiredPlates)
            {
                if (plate != null && plate.IsActivated())
                {
                    shouldTrigger = true;
                    break;
                }
            }
        }
        
        if (shouldTrigger != isTriggered)
        {
            isTriggered = shouldTrigger;
            
            if (isTriggered)
            {
                TriggerDoor(openOnActivation);
            }
            else
            {
                TriggerDoor(!openOnActivation);
            }
            
            UpdateIndicator();
        }
    }
    
    void TriggerDoor(bool open)
    {
        if (targetDoor == null) return;
        
        if (delay > 0f)
        {
            Invoke(nameof(ExecuteDoorAction), delay);
        }
        else
        {
            ExecuteDoorAction();
        }
        
        void ExecuteDoorAction()
        {
            if (open)
            {
                targetDoor.OpenDoor();
            }
            else
            {
                targetDoor.CloseDoor();
            }
        }
    }
    
    void UpdateIndicator()
    {
        if (indicatorRenderer != null)
        {
            indicatorRenderer.material.color = isTriggered ? activeColor : inactiveColor;
        }
    }
    
    // Public methods
    public bool IsTriggered()
    {
        return isTriggered;
    }
    
    public void SetTargetDoor(Door door)
    {
        targetDoor = door;
    }
    
    public void AddPressurePlate(PressurePlate plate)
    {
        if (plate != null)
        {
            // Initialize array if null
            if (requiredPlates == null)
            {
                requiredPlates = new PressurePlate[0];
            }
            
            // Check if plate already exists
            if (!System.Array.Exists(requiredPlates, p => p == plate))
            {
                // Resize array and add new plate
                PressurePlate[] newPlates = new PressurePlate[requiredPlates.Length + 1];
                requiredPlates.CopyTo(newPlates, 0);
                newPlates[requiredPlates.Length] = plate;
                requiredPlates = newPlates;
                
                // Subscribe to events
                plate.OnActivated.AddListener(CheckTrigger);
                plate.OnDeactivated.AddListener(CheckTrigger);
            }
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (requiredPlates != null)
        {
            foreach (PressurePlate plate in requiredPlates)
            {
                if (plate != null)
                {
                    plate.OnActivated.RemoveListener(CheckTrigger);
                    plate.OnDeactivated.RemoveListener(CheckTrigger);
                }
            }
        }
    }
}
