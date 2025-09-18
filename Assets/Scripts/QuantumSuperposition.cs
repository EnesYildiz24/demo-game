using UnityEngine;

public class QuantumSuperposition : MonoBehaviour
{
    [Header("Quantum Superposition Settings")]
    public Transform[] quantumPositions;
    public float switchInterval = 2f;
    public Color quantumColor = Color.blue;
    
    private int currentPositionIndex = 0;
    private float lastSwitchTime;
    private Vector3 originalPosition;
    private bool isActive = false;
    
    void Start()
    {
        originalPosition = transform.position;
        lastSwitchTime = Time.time;
    }
    
    void Update()
    {
        if (isActive && quantumPositions.Length > 0)
        {
            if (Time.time - lastSwitchTime >= switchInterval)
            {
                SwitchPosition();
                lastSwitchTime = Time.time;
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActive = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isActive = false;
            transform.position = originalPosition;
        }
    }
    
    private void SwitchPosition()
    {
        if (quantumPositions.Length > 0)
        {
            currentPositionIndex = (currentPositionIndex + 1) % quantumPositions.Length;
            transform.position = quantumPositions[currentPositionIndex].position;
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = quantumColor;
        Gizmos.DrawWireSphere(transform.position, 1f);
        
        if (quantumPositions != null)
        {
            for (int i = 0; i < quantumPositions.Length; i++)
            {
                if (quantumPositions[i] != null)
                {
                    Gizmos.color = i == currentPositionIndex ? Color.white : quantumColor;
                    Gizmos.DrawWireSphere(quantumPositions[i].position, 0.8f);
                    
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(transform.position, quantumPositions[i].position);
                }
            }
        }
    }
}
