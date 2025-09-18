using UnityEngine;

public class PostProcessingManager : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    
    [Header("Color Correction Settings")]
    public bool enableColorCorrection = true;
    [Range(-100f, 100f)]
    public float brightness = 0f;
    [Range(-100f, 100f)]
    public float contrast = 0f;
    [Range(-100f, 100f)]
    public float saturation = 0f;
    
    [Header("Camera Effects")]
    public bool enableVignette = false;
    [Range(0f, 1f)]
    public float vignetteIntensity = 0.3f;
    [Range(0f, 1f)]
    public float vignetteSmoothness = 0.2f;
    
    [Header("Motion Blur")]
    public bool enableMotionBlur = false;
    [Range(0f, 1f)]
    public float motionBlurIntensity = 0.5f;
    
    private Material colorCorrectionMaterial;
    private Material vignetteMaterial;
    private Material motionBlurMaterial;
    
    void Start()
    {
        SetupPostProcessing();
    }
    
    void SetupPostProcessing()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                playerCamera = FindObjectOfType<Camera>();
            }
        }
        
        CreateMaterials();
    }
    
    void CreateMaterials()
    {
        // Create color correction shader
        Shader colorCorrectionShader = Shader.Find("Hidden/ColorCorrection");
        if (colorCorrectionShader != null)
        {
            colorCorrectionMaterial = new Material(colorCorrectionShader);
        }
        
        // Create vignette shader
        Shader vignetteShader = Shader.Find("Hidden/Vignette");
        if (vignetteShader != null)
        {
            vignetteMaterial = new Material(vignetteShader);
        }
        
        // Create motion blur shader
        Shader motionBlurShader = Shader.Find("Hidden/MotionBlur");
        if (motionBlurShader != null)
        {
            motionBlurMaterial = new Material(motionBlurShader);
        }
    }
    
    void Update()
    {
        UpdateColorCorrection();
        UpdateVignette();
        UpdateMotionBlur();
    }
    
    void UpdateColorCorrection()
    {
        if (colorCorrectionMaterial != null && enableColorCorrection)
        {
            colorCorrectionMaterial.SetFloat("_Brightness", brightness / 100f);
            colorCorrectionMaterial.SetFloat("_Contrast", contrast / 100f + 1f);
            colorCorrectionMaterial.SetFloat("_Saturation", saturation / 100f + 1f);
        }
    }
    
    void UpdateVignette()
    {
        if (vignetteMaterial != null && enableVignette)
        {
            vignetteMaterial.SetFloat("_Intensity", vignetteIntensity);
            vignetteMaterial.SetFloat("_Smoothness", vignetteSmoothness);
        }
    }
    
    void UpdateMotionBlur()
    {
        if (motionBlurMaterial != null && enableMotionBlur)
        {
            motionBlurMaterial.SetFloat("_Intensity", motionBlurIntensity);
        }
    }
    
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (playerCamera == null) return;
        
        RenderTexture temp = source;
        
        // Apply color correction
        if (enableColorCorrection && colorCorrectionMaterial != null)
        {
            RenderTexture temp2 = RenderTexture.GetTemporary(source.width, source.height);
            Graphics.Blit(temp, temp2, colorCorrectionMaterial);
            temp = temp2;
        }
        
        // Apply vignette
        if (enableVignette && vignetteMaterial != null)
        {
            RenderTexture temp3 = RenderTexture.GetTemporary(source.width, source.height);
            Graphics.Blit(temp, temp3, vignetteMaterial);
            if (temp != source) RenderTexture.ReleaseTemporary(temp);
            temp = temp3;
        }
        
        // Apply motion blur
        if (enableMotionBlur && motionBlurMaterial != null)
        {
            RenderTexture temp4 = RenderTexture.GetTemporary(source.width, source.height);
            Graphics.Blit(temp, temp4, motionBlurMaterial);
            if (temp != source) RenderTexture.ReleaseTemporary(temp);
            temp = temp4;
        }
        
        // Final blit
        Graphics.Blit(temp, destination);
        
        // Clean up temporary textures
        if (temp != source) RenderTexture.ReleaseTemporary(temp);
    }
    
    // Public methods for runtime changes
    public void SetColorCorrection(bool enabled, float bright = 0f, float contrastVal = 0f, float saturationVal = 0f)
    {
        enableColorCorrection = enabled;
        brightness = bright;
        contrast = contrastVal;
        saturation = saturationVal;
    }
    
    public void SetVignette(bool enabled, float intensity = 0.3f, float smoothness = 0.2f)
    {
        enableVignette = enabled;
        vignetteIntensity = intensity;
        vignetteSmoothness = smoothness;
    }
    
    public void SetMotionBlur(bool enabled, float intensity = 0.5f)
    {
        enableMotionBlur = enabled;
        motionBlurIntensity = intensity;
    }
}
