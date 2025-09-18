using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interactable Settings")]
    public string interactPrompt = "E drücken";
    public float interactionRange = 3f;
    public bool canInteract = true;
    
    public abstract void Interact();
    
    public virtual bool CanInteract()
    {
        return canInteract;
    }
    
    public virtual string GetPromptText()
    {
        return interactPrompt;
    }
}
