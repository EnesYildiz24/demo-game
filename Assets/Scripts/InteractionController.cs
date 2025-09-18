using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    
    [Header("Interaction Settings")]
    public float interactionRange = 5f;
    public LayerMask interactionLayers = ~0;
    public KeyCode interactKey = KeyCode.E;
    
    private Interactable currentInteractable;
    
    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }
    }
    
    void Update()
    {
        CheckForInteractables();
        HandleInteraction();
    }
    
    void CheckForInteractables()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, interactionRange, interactionLayers, QueryTriggerInteraction.Ignore))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            
            if (interactable != null && interactable.CanInteract())
            {
                currentInteractable = interactable;
            }
            else
            {
                currentInteractable = null;
            }
        }
        else
        {
            currentInteractable = null;
        }
    }
    
    void HandleInteraction()
    {
        if (currentInteractable != null && Input.GetKeyDown(interactKey))
        {
            currentInteractable.Interact();
        }
    }
    
    public Interactable GetCurrentInteractable()
    {
        return currentInteractable;
    }
    
    public bool HasInteractable()
    {
        return currentInteractable != null;
    }
    
    void OnDrawGizmosSelected()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactionRange);
        }
    }
}
