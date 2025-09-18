using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambientSource;
    
    [Header("Step Sounds")]
    public AudioClip[] stepSounds;
    public float stepInterval = 0.5f;
    public float sprintStepInterval = 0.3f;
    
    [Header("Jump Sounds")]
    public AudioClip jumpSound;
    public AudioClip landSound;
    
    [Header("Interaction Sounds")]
    public AudioClip grabSound;
    public AudioClip dropSound;
    public AudioClip switchSound;
    public AudioClip doorSound;
    
    [Header("Ambient Audio")]
    public AudioClip ambientLoop;
    [Range(0f, 1f)]
    public float ambientVolume = 0.3f;
    
    [Header("Music")]
    public AudioClip backgroundMusic;
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    
    private PlayerController playerController;
    private float lastStepTime;
    private bool isPlayingAmbient = false;
    
    void Start()
    {
        SetupAudioSources();
        InitializeAudioClips();
        PlayBackgroundMusic();
        PlayAmbientAudio();
        // PlayerController will be found in Update() when it's created
    }
    
    void InitializeAudioClips()
    {
        // Initialize stepSounds array if it's null
        if (stepSounds == null)
        {
            stepSounds = new AudioClip[0];
            Debug.Log("AudioManager: Initialized empty stepSounds array");
        }
    }
    
    void SetupAudioSources()
    {
        // Music source
        if (musicSource == null)
        {
            GameObject musicGO = new GameObject("MusicSource");
            musicGO.transform.SetParent(transform);
            musicSource = musicGO.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.playOnAwake = false;
        }
        
        // SFX source
        if (sfxSource == null)
        {
            GameObject sfxGO = new GameObject("SFXSource");
            sfxGO.transform.SetParent(transform);
            sfxSource = sfxGO.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.volume = 1f;
            sfxSource.playOnAwake = false;
        }
        
        // Ambient source
        if (ambientSource == null)
        {
            GameObject ambientGO = new GameObject("AmbientSource");
            ambientGO.transform.SetParent(transform);
            ambientSource = ambientGO.AddComponent<AudioSource>();
            ambientSource.loop = true;
            ambientSource.volume = ambientVolume;
            ambientSource.playOnAwake = false;
        }
    }
    
    void Update()
    {
        HandleStepSounds();
    }
    
    void HandleStepSounds()
    {
        // Early return if no step sounds available
        if (stepSounds == null || stepSounds.Length == 0) 
        {
            // Debug: Log when step sounds are not available
            if (stepSounds == null)
            {
                Debug.LogWarning("AudioManager: stepSounds is null");
            }
            return;
        }
        
        // Try to find player controller if not found yet
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
            if (playerController == null) return;
        }
        
        // Additional safety check - make sure player is fully initialized
        CharacterController controller = playerController.GetComponent<CharacterController>();
        if (controller == null) return;
        
        // Check if player is moving and grounded
        bool isMoving = controller.velocity.magnitude > 0.1f;
        bool isGrounded = controller.isGrounded;
        
        if (isMoving && isGrounded)
        {
            float currentTime = Time.time;
            float stepIntervalToUse = (playerController != null && playerController.isSprinting) ? sprintStepInterval : stepInterval;
            
            if (currentTime - lastStepTime >= stepIntervalToUse)
            {
                PlayRandomStepSound();
                lastStepTime = currentTime;
            }
        }
    }
    
    void PlayRandomStepSound()
    {
        if (stepSounds != null && stepSounds.Length > 0 && sfxSource != null)
        {
            AudioClip randomStep = stepSounds[Random.Range(0, stepSounds.Length)];
            if (randomStep != null)
            {
                sfxSource.PlayOneShot(randomStep, 0.7f);
            }
        }
    }
    
    public void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            sfxSource.PlayOneShot(jumpSound, 0.8f);
        }
    }
    
    public void PlayLandSound()
    {
        if (landSound != null)
        {
            sfxSource.PlayOneShot(landSound, 0.6f);
        }
    }
    
    public void PlayGrabSound()
    {
        if (grabSound != null)
        {
            sfxSource.PlayOneShot(grabSound, 0.5f);
        }
    }
    
    public void PlayDropSound()
    {
        if (dropSound != null)
        {
            sfxSource.PlayOneShot(dropSound, 0.5f);
        }
    }
    
    public void PlaySwitchSound()
    {
        if (switchSound != null)
        {
            sfxSource.PlayOneShot(switchSound, 0.6f);
        }
    }
    
    public void PlayDoorSound()
    {
        if (doorSound != null)
        {
            sfxSource.PlayOneShot(doorSound, 0.7f);
        }
    }
    
    void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }
    
    void PlayAmbientAudio()
    {
        if (ambientLoop != null && ambientSource != null && !isPlayingAmbient)
        {
            ambientSource.clip = ambientLoop;
            ambientSource.Play();
            isPlayingAmbient = true;
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }
    
    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
    }
    
    public void SetAmbientVolume(float volume)
    {
        ambientVolume = Mathf.Clamp01(volume);
        if (ambientSource != null)
        {
            ambientSource.volume = ambientVolume;
        }
    }
}
