using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    public string[] levelScenes = { "Level1", "Level2", "Level3" };
    public string hubScene = "Hub";
    public string mainMenuScene = "MainMenu";
    
    [Header("Level Loading")]
    public float fadeTime = 1f;
    public CanvasGroup loadingScreen;
    
    private int currentLevelIndex = 0;
    private bool isLoading = false;
    
    public static LevelManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (loadingScreen != null)
        {
            loadingScreen.alpha = 0f;
            loadingScreen.gameObject.SetActive(false);
        }
    }
    
    public void LoadLevel(int levelIndex)
    {
        if (isLoading || levelIndex < 0 || levelIndex >= levelScenes.Length) return;
        
        if (SaveSystem.Instance != null && !SaveSystem.Instance.IsLevelUnlocked(levelIndex))
        {
            Debug.Log("Level " + levelIndex + " is not unlocked yet!");
            return;
        }
        
        currentLevelIndex = levelIndex;
        StartCoroutine(LoadLevelCoroutine(levelScenes[levelIndex]));
    }
    
    public void LoadNextLevel()
    {
        int nextLevel = currentLevelIndex + 1;
        if (nextLevel < levelScenes.Length)
        {
            LoadLevel(nextLevel);
        }
        else
        {
            LoadHub();
        }
    }
    
    public void ReloadCurrentLevel()
    {
        if (currentLevelIndex >= 0 && currentLevelIndex < levelScenes.Length)
        {
            StartCoroutine(LoadLevelCoroutine(levelScenes[currentLevelIndex]));
        }
    }
    
    public void LoadHub()
    {
        StartCoroutine(LoadLevelCoroutine(hubScene));
    }
    
    public void LoadMainMenu()
    {
        StartCoroutine(LoadLevelCoroutine(mainMenuScene));
    }
    
    IEnumerator LoadLevelCoroutine(string sceneName)
    {
        isLoading = true;
        
        // Show loading screen
        if (loadingScreen != null)
        {
            loadingScreen.gameObject.SetActive(true);
            yield return StartCoroutine(FadeCanvasGroup(loadingScreen, 0f, 1f, fadeTime * 0.5f));
        }
        
        // Load scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        
        while (!asyncLoad.isDone)
        {
            // Optional: Update loading progress bar here
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
        
        // Wait a frame for scene to fully load
        yield return null;
        
        // Hide loading screen
        if (loadingScreen != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(loadingScreen, 1f, 0f, fadeTime * 0.5f));
            loadingScreen.gameObject.SetActive(false);
        }
        
        isLoading = false;
    }
    
    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            canvasGroup.alpha = alpha;
            yield return null;
        }
        
        canvasGroup.alpha = endAlpha;
    }
    
    public void CompleteLevel(float completionTime)
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.CompleteLevel(currentLevelIndex, completionTime);
        }
        
        // Show completion UI or automatically load next level
        Debug.Log("Level " + currentLevelIndex + " completed in " + completionTime.ToString("F2") + " seconds!");
        
        // Auto-load next level after a delay
        StartCoroutine(LoadNextLevelAfterDelay(3f));
    }
    
    IEnumerator LoadNextLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadNextLevel();
    }
    
    public void CollectCrystal(int crystalIndex)
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.CollectCrystal(crystalIndex);
        }
    }
    
    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
    
    public string GetCurrentLevelName()
    {
        if (currentLevelIndex >= 0 && currentLevelIndex < levelScenes.Length)
        {
            return levelScenes[currentLevelIndex];
        }
        return "Unknown";
    }
    
    public bool IsLevelUnlocked(int levelIndex)
    {
        if (SaveSystem.Instance != null)
        {
            return SaveSystem.Instance.IsLevelUnlocked(levelIndex);
        }
        return levelIndex == 0; // First level always unlocked
    }
    
    public float GetBestTime(int levelIndex)
    {
        if (SaveSystem.Instance != null)
        {
            return SaveSystem.Instance.GetBestTime(levelIndex);
        }
        return 0f;
    }
    
    public int GetTotalCrystals()
    {
        if (SaveSystem.Instance != null)
        {
            return SaveSystem.Instance.GetTotalCrystals();
        }
        return 0;
    }
}
