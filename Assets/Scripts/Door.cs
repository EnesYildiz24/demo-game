using UnityEngine;

public class Door : Interactable
{
    [Header("Door Settings")]
    public bool isOpen = false;
    public float openAngle = 90f;
    public float closeAngle = 0f;
    public float doorSpeed = 2f;
    public Transform doorPivot; // Pivot point for rotation
    
    [Header("Events")]
    public UnityEngine.Events.UnityEvent OnDoorOpen;
    public UnityEngine.Events.UnityEvent OnDoorClose;
    
    private bool isMoving = false;
    private float currentAngle;
    private float targetAngle;
    
    void Start()
    {
        if (doorPivot == null)
        {
            doorPivot = transform;
        }
        
        currentAngle = isOpen ? openAngle : closeAngle;
        targetAngle = currentAngle;
        UpdateDoorRotation();
    }
    
    void Update()
    {
        if (isMoving)
        {
            currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, doorSpeed * Time.deltaTime);
            UpdateDoorRotation();
            
            if (Mathf.Approximately(currentAngle, targetAngle))
            {
                isMoving = false;
                isOpen = (targetAngle == openAngle);

                // Event auslösen wenn Tür sich öffnet
                if (isOpen && OnDoorOpen != null)
                {
                    OnDoorOpen.Invoke();
                }
            }
        }
    }
    
    public override void Interact()
    {
        if (isMoving) return;
        
        isOpen = !isOpen;
        targetAngle = isOpen ? openAngle : closeAngle;
        isMoving = true;
        
        if (isOpen)
        {
            OnDoorOpen?.Invoke();
        }
        else
        {
            OnDoorClose?.Invoke();
        }
    }
    
    private void UpdateDoorRotation()
    {
        doorPivot.rotation = Quaternion.Euler(0, currentAngle, 0);
    }
    
    public override string GetPromptText()
    {
        return isOpen ? "Tür schließen" : "Tür öffnen";
    }
    
    // Public methods for external control
    public void OpenDoor()
    {
        if (isMoving || isOpen) return;
        Interact();
    }
    
    public void CloseDoor()
    {
        if (isMoving || !isOpen) return;
        Interact();
    }
    
    public void SetDoorState(bool open)
    {
        if (isMoving) return;
        
        isOpen = open;
        targetAngle = isOpen ? openAngle : closeAngle;
        isMoving = true;
    }
}
