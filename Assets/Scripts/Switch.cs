using UnityEngine;

public class Switch : Interactable
{
    [Header("Switch Settings")]
    public bool isOn = false;
    public float switchTime = 0.5f;
    public Vector3 onRotation = new Vector3(0, 0, 45f);
    public Vector3 offRotation = new Vector3(0, 0, -45f);
    
    [Header("Events")]
    public UnityEngine.Events.UnityEvent OnSwitchOn;
    public UnityEngine.Events.UnityEvent OnSwitchOff;
    
    private bool isSwitching = false;
    private float switchTimer = 0f;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    
    void Start()
    {
        startRotation = transform.rotation;
        UpdateTargetRotation();
    }
    
    void Update()
    {
        if (isSwitching)
        {
            switchTimer += Time.deltaTime;
            float progress = switchTimer / switchTime;
            
            if (progress >= 1f)
            {
                progress = 1f;
                isSwitching = false;
            }
            
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, progress);
        }
    }
    
    public override void Interact()
    {
        if (isSwitching) return;
        
        isOn = !isOn;
        StartSwitch();
        
        if (isOn)
        {
            OnSwitchOn?.Invoke();
        }
        else
        {
            OnSwitchOff?.Invoke();
        }
    }
    
    private void StartSwitch()
    {
        isSwitching = true;
        switchTimer = 0f;
        startRotation = transform.rotation;
        UpdateTargetRotation();
    }
    
    private void UpdateTargetRotation()
    {
        Vector3 targetEuler = isOn ? onRotation : offRotation;
        targetRotation = Quaternion.Euler(targetEuler);
    }
    
    public override string GetPromptText()
    {
        return isOn ? "Schalter ausschalten" : "Schalter einschalten";
    }
}
