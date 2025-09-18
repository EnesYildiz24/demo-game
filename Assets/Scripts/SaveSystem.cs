using UnityEngine;
using System.IO;

[System.Serializable]
public class GameSettings
{
    public float mouseSensitivity = 2f;
    public float masterVolume = 1f;
    public float musicVolume = 0.5f;
    public float sfxVolume = 1f;
    public float ambientVolume = 0.3f;
    public bool fullscreen = true;
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;
    public int qualityLevel = 2;
}

[System.Serializable]
public class GameProgress
{
    public int currentLevel = 0;
    public float[] levelTimes = new float[10]; // Best times for 10 levels
    public bool[] crystalsCollected = new bool[50]; // 50 crystals total
    public int totalCrystals = 0;
    public bool[] levelsUnlocked = new bool[10];
    public string playerName = "Player";
}

public class SaveSystem : MonoBehaviour
{
    [Header("Save Settings")]
    public string settingsFileName = "settings.json";
    public string progressFileName = "progress.json";
    
    private string settingsPath;
    private string progressPath;
    
    public GameSettings gameSettings;
    public GameProgress gameProgress;
    
    public static SaveSystem Instance { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePaths();
            LoadSettings();
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializePaths()
    {
        string dataPath = Application.persistentDataPath;
        settingsPath = Path.Combine(dataPath, settingsFileName);
        progressPath = Path.Combine(dataPath, progressFileName);
    }
    
    public void SaveSettings()
    {
        try
        {
            string json = JsonUtility.ToJson(gameSettings, true);
            File.WriteAllText(settingsPath, json);
            Debug.Log("Settings saved to: " + settingsPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save settings: " + e.Message);
        }
    }
    
    public void LoadSettings()
    {
        try
        {
            if (File.Exists(settingsPath))
            {
                string json = File.ReadAllText(settingsPath);
                gameSettings = JsonUtility.FromJson<GameSettings>(json);
                ApplySettings();
                Debug.Log("Settings loaded from: " + settingsPath);
            }
            else
            {
                gameSettings = new GameSettings();
                SaveSettings();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load settings: " + e.Message);
            gameSettings = new GameSettings();
        }
    }
    
    public void SaveProgress()
    {
        try
        {
            string json = JsonUtility.ToJson(gameProgress, true);
            File.WriteAllText(progressPath, json);
            Debug.Log("Progress saved to: " + progressPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save progress: " + e.Message);
        }
    }
    
    public void LoadProgress()
    {
        try
        {
            if (File.Exists(progressPath))
            {
                string json = File.ReadAllText(progressPath);
                gameProgress = JsonUtility.FromJson<GameProgress>(json);
                Debug.Log("Progress loaded from: " + progressPath);
            }
            else
            {
                gameProgress = new GameProgress();
                // Unlock first level by default
                gameProgress.levelsUnlocked[0] = true;
                SaveProgress();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load progress: " + e.Message);
            gameProgress = new GameProgress();
            gameProgress.levelsUnlocked[0] = true;
        }
    }
    
    void ApplySettings()
    {
        // Apply audio settings
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            audioManager.SetMusicVolume(gameSettings.musicVolume);
            audioManager.SetSFXVolume(gameSettings.sfxVolume);
            audioManager.SetAmbientVolume(gameSettings.ambientVolume);
        }
        
        // Apply graphics settings
        QualitySettings.SetQualityLevel(gameSettings.qualityLevel);
        Screen.SetResolution(gameSettings.resolutionWidth, gameSettings.resolutionHeight, gameSettings.fullscreen);
        
        // Apply mouse sensitivity
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.mouseSensitivity = gameSettings.mouseSensitivity;
        }
    }
    
    // Progress management methods
    public void CompleteLevel(int levelIndex, float time)
    {
        if (levelIndex >= 0 && levelIndex < gameProgress.levelTimes.Length)
        {
            if (gameProgress.levelTimes[levelIndex] == 0f || time < gameProgress.levelTimes[levelIndex])
            {
                gameProgress.levelTimes[levelIndex] = time;
            }
            
            // Unlock next level
            if (levelIndex + 1 < gameProgress.levelsUnlocked.Length)
            {
                gameProgress.levelsUnlocked[levelIndex + 1] = true;
            }
        }
        SaveProgress();
    }
    
    public void CollectCrystal(int crystalIndex)
    {
        if (crystalIndex >= 0 && crystalIndex < gameProgress.crystalsCollected.Length)
        {
            if (!gameProgress.crystalsCollected[crystalIndex])
            {
                gameProgress.crystalsCollected[crystalIndex] = true;
                gameProgress.totalCrystals++;
            }
        }
        SaveProgress();
    }
    
    public bool IsLevelUnlocked(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < gameProgress.levelsUnlocked.Length)
        {
            return gameProgress.levelsUnlocked[levelIndex];
        }
        return false;
    }
    
    public float GetBestTime(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < gameProgress.levelTimes.Length)
        {
            return gameProgress.levelTimes[levelIndex];
        }
        return 0f;
    }
    
    public int GetTotalCrystals()
    {
        return gameProgress.totalCrystals;
    }
    
    // Settings management methods
    public void UpdateMouseSensitivity(float sensitivity)
    {
        gameSettings.mouseSensitivity = sensitivity;
        SaveSettings();
    }
    
    public void UpdateVolume(float master, float music, float sfx, float ambient)
    {
        gameSettings.masterVolume = master;
        gameSettings.musicVolume = music;
        gameSettings.sfxVolume = sfx;
        gameSettings.ambientVolume = ambient;
        SaveSettings();
        ApplySettings();
    }
    
    public void UpdateGraphics(int quality, int width, int height, bool fullscreen)
    {
        gameSettings.qualityLevel = quality;
        gameSettings.resolutionWidth = width;
        gameSettings.resolutionHeight = height;
        gameSettings.fullscreen = fullscreen;
        SaveSettings();
        ApplySettings();
    }
}
