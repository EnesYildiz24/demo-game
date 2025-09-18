using UnityEngine;
using UnityEngine.UI;

public class GrabUI : MonoBehaviour
{
    [Header("References")]
    public GrabController grabController;
    public Camera playerCamera;

    [Header("UI Settings")]
    public string interactPromptText = "E aufnehmen";
    public Color crosshairColor = new Color(1f, 1f, 1f, 0.8f);
    public Color promptColor = new Color(1f, 1f, 1f, 0.9f);
    public int crosshairSize = 18;
    public int promptSize = 18;

    private Canvas canvas;
    private Text crosshair;
    private Text prompt;

    void Start()
    {
        if (grabController == null)
        {
            grabController = FindObjectOfType<GrabController>();
        }
        if (playerCamera == null && grabController != null)
        {
            playerCamera = grabController.playerCamera;
        }

        CreateUI();
    }

    void CreateUI()
    {
        GameObject canvasGO = new GameObject("GrabUI");
        canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<GraphicRaycaster>();

        // Crosshair
        GameObject crossGO = new GameObject("Crosshair");
        crossGO.transform.SetParent(canvas.transform, false);
        crosshair = crossGO.AddComponent<Text>();
        crosshair.text = "+";
        crosshair.fontSize = crosshairSize;
        crosshair.color = crosshairColor;
        crosshair.alignment = TextAnchor.MiddleCenter;
        crosshair.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        RectTransform chRect = crosshair.rectTransform;
        chRect.anchorMin = new Vector2(0.5f, 0.5f);
        chRect.anchorMax = new Vector2(0.5f, 0.5f);
        chRect.anchoredPosition = Vector2.zero;
        chRect.sizeDelta = new Vector2(40, 40);

        // Prompt
        GameObject promptGO = new GameObject("InteractPrompt");
        promptGO.transform.SetParent(canvas.transform, false);
        prompt = promptGO.AddComponent<Text>();
        prompt.text = interactPromptText;
        prompt.fontSize = promptSize;
        prompt.color = promptColor;
        prompt.alignment = TextAnchor.UpperCenter;
        prompt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        RectTransform pRect = prompt.rectTransform;
        pRect.anchorMin = new Vector2(0.5f, 0.5f);
        pRect.anchorMax = new Vector2(0.5f, 0.5f);
        pRect.anchoredPosition = new Vector2(0f, -30f);
        pRect.sizeDelta = new Vector2(400, 60);
        prompt.enabled = false;
    }

    void Update()
    {
        if (grabController == null || playerCamera == null)
        {
            if (prompt != null) prompt.enabled = false;
            return;
        }

        // If holding, hide prompt
        bool isHolding = IsHolding();
        if (isHolding)
        {
            prompt.enabled = false;
            return;
        }

        // Check for interactables first (doors, switches)
        InteractionController interactionController = FindObjectOfType<InteractionController>();
        if (interactionController != null && interactionController.HasInteractable())
        {
            Interactable interactable = interactionController.GetCurrentInteractable();
            if (interactable != null)
            {
                prompt.text = interactable.GetPromptText();
                prompt.enabled = true;
                return;
            }
        }

        // Raycast for grabbable
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, grabController.grabRange, grabController.grabbableLayers, QueryTriggerInteraction.Ignore))
        {
            Grabbable g = hit.collider.GetComponentInParent<Grabbable>();
            if (g != null)
            {
                prompt.text = interactPromptText;
                prompt.enabled = true;
                return;
            }
        }

        prompt.enabled = false;
    }

    bool IsHolding()
    {
        // reflect via public property via pattern: held != null is private; use heuristic: when distance scrolling enabled and object is kinematic
        // Simpler: do a small overlap at hold point when E is pressed? Instead, expose through presence: we can't access private. So infer by another raycast to targetDistance position not reliable.
        // Use workaround: prompt hides only when grab key is used; practical approach: prompt shown only when looking at a grabbable not currently picked.
        // We'll ask indirectly by trying to detect a child temporary object - not available. So always return false here and rely on look test for prompt.
        return false;
    }
}


