using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int totalCrystals = 6; // 5 normale + 1 hinter der Tür
    
    private int collectedCrystals = 0;
    private bool gameWon = false;
    private UIManager uiManager;
    
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            GameObject uiManagerGO = new GameObject("UIManager");
            uiManager = uiManagerGO.AddComponent<UIManager>();
        }
        
        UpdateUI();
        ShowInstructions();
    }
    
    void Update()
    {
        // Check for escape key to unlock cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        
        // Check for click to lock cursor again
        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    
    public void CollectCrystal(int value)
    {
        collectedCrystals += value;
        UpdateUI();
        
        if (collectedCrystals >= totalCrystals && !gameWon)
        {
            WinGame();
        }
    }
    
    private void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateCrystalCount(collectedCrystals, totalCrystals);
        }
    }
    
    private void ShowInstructions()
    {
        // Instructions are now handled by UIManager
        // This method can be used for additional instruction logic if needed
    }
    
    private void WinGame()
    {
        gameWon = true;

        if (uiManager != null)
        {
            uiManager.ShowWinPanel();
        }

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
    }

    // Neue Methode für sofortigen Sieg (z.B. wenn Tür sich öffnet)
    public void ForceWin()
    {
        if (!gameWon)
        {
            Debug.Log("Spieler hat gewonnen! Tür wurde geöffnet.");
            WinGame();
        }
    }
    
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
