using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grabbable : MonoBehaviour
{
    [Header("Grab Settings")]
    public bool allowRotationWhileHeld = false;
    public float maxHoldDistance = 12f;
    public float minHoldDistance = 1.2f;

    private Rigidbody cachedRigidbody;

    public Rigidbody GetRigidbody()
    {
        if (cachedRigidbody == null)
        {
            cachedRigidbody = GetComponent<Rigidbody>();
        }
        return cachedRigidbody;
    }
}


