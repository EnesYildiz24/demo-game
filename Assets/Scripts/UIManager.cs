using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public Canvas mainCanvas;
    public TextMeshProUGUI crystalCountText;
    public TextMeshProUGUI instructionsText;
    public GameObject winPanel;
    public Button restartButton;
    public Button quitButton;
    
    [Header("UI Settings")]
    public Color crystalColor = Color.yellow;
    public Color instructionColor = Color.white;
    public Color winColor = Color.green;
    
    private GameManager gameManager;
    
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        SetupUI();
        HideInstructions(); // Hide instructions immediately
    }
    
    private void SetupUI()
    {
        // Create main canvas if it doesn't exist
        if (mainCanvas == null)
        {
            GameObject canvasGO = new GameObject("MainCanvas");
            mainCanvas = canvasGO.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            mainCanvas.sortingOrder = 100;
            
            // Add CanvasScaler
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // Add GraphicRaycaster
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        CreateCrystalCounter();
        CreateInstructions();
        CreateWinPanel();
    }
    
    private void CreateCrystalCounter()
    {
        if (crystalCountText == null)
        {
            GameObject crystalGO = new GameObject("CrystalCounter");
            crystalGO.transform.SetParent(mainCanvas.transform, false);
            
            crystalCountText = crystalGO.AddComponent<TextMeshProUGUI>();
            crystalCountText.text = "Kristalle: 0/5";
            crystalCountText.fontSize = 24;
            crystalCountText.color = crystalColor;
            crystalCountText.alignment = TextAlignmentOptions.TopLeft;
            
            RectTransform rectTransform = crystalCountText.rectTransform;
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(20, -20);
            rectTransform.sizeDelta = new Vector2(200, 50);
        }
    }
    
    private void CreateInstructions()
    {
        // Don't create instructions by default - keep screen clean
        // Instructions can be shown by calling ShowInstructions() if needed
        
        // Hide any existing instructions
        if (instructionsText != null)
        {
            instructionsText.gameObject.SetActive(false);
        }
    }
    
    private void CreateWinPanel()
    {
        if (winPanel == null)
        {
            // Create win panel background
            GameObject winPanelGO = new GameObject("WinPanel");
            winPanelGO.transform.SetParent(mainCanvas.transform, false);
            winPanel = winPanelGO;
            
            Image panelImage = winPanelGO.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.8f);
            
            RectTransform panelRect = winPanelGO.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            
            // Create win text
            GameObject winTextGO = new GameObject("WinText");
            winTextGO.transform.SetParent(winPanel.transform, false);
            
            TextMeshProUGUI winText = winTextGO.AddComponent<TextMeshProUGUI>();
            winText.text = "HERZLICHEN GLÜCKWUNSCH!\n\nDu hast alle Kristalle gesammelt!\nDu bist ein wahrer Quantum-Puzzle-Meister!";
            winText.fontSize = 32;
            winText.color = winColor;
            winText.alignment = TextAlignmentOptions.Center;
            
            RectTransform winTextRect = winTextGO.GetComponent<RectTransform>();
            winTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            winTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            winTextRect.anchoredPosition = new Vector2(0, 50);
            winTextRect.sizeDelta = new Vector2(800, 200);
            
            // Create restart button
            CreateButton("RestartButton", "Neustart", new Vector2(0, -50), () => {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            });
            
            // Create quit button
            CreateButton("QuitButton", "Beenden", new Vector2(0, -120), () => {
                Application.Quit();
            });
            
            winPanel.SetActive(false);
        }
    }
    
    private void CreateButton(string name, string text, Vector2 position, System.Action onClick)
    {
        GameObject buttonGO = new GameObject(name);
        buttonGO.transform.SetParent(winPanel.transform, false);
        
        Button button = buttonGO.AddComponent<Button>();
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        
        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = new Vector2(200, 50);
        
        // Create button text
        GameObject buttonTextGO = new GameObject("Text");
        buttonTextGO.transform.SetParent(buttonGO.transform, false);
        
        TextMeshProUGUI buttonText = buttonTextGO.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 18;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        
        RectTransform buttonTextRect = buttonTextGO.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;
        
        button.onClick.AddListener(() => onClick());
    }
    
    public void UpdateCrystalCount(int current, int total)
    {
        if (crystalCountText != null)
        {
            crystalCountText.text = $"Kristalle: {current}/{total}";
        }
    }
    
    public void ShowWinPanel()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
    }
    
    
    public void ShowInstructions()
    {
        if (instructionsText == null)
        {
            GameObject instructionsGO = new GameObject("Instructions");
            instructionsGO.transform.SetParent(mainCanvas.transform, false);
            
            instructionsText = instructionsGO.AddComponent<TextMeshProUGUI>();
            instructionsText.text = "Steuerung:\nWASD - Bewegung\nMaus - Kamera\nLeertaste - Springen\nE - Interagieren\nESC - Cursor freigeben\n\nZiel: Sammle alle Kristalle!";
            instructionsText.fontSize = 16;
            instructionsText.color = instructionColor;
            instructionsText.alignment = TextAlignmentOptions.Center;
            
            RectTransform rectTransform = instructionsText.rectTransform;
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(600, 400);
        }
        instructionsText.gameObject.SetActive(true);
    }
    
    public void HideInstructions()
    {
        if (instructionsText != null)
        {
            instructionsText.gameObject.SetActive(false);
        }
    }
    
    public void HideAllUI()
    {
        // Hide instructions but keep crystal counter
        HideInstructions();
    }
    
    public void ShowMinimalUI()
    {
        // Show only essential UI elements
        HideInstructions();
        // Crystal counter stays visible
    }
}
