using UnityEngine;

public class GrabController : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;

    [Header("Grab Settings")]
    public float grabRange = 5f;
    public float holdDistance = 3f;
    public float moveSmoothing = 25f;
    public float throwForce = 8f;
    public LayerMask grabbableLayers = ~0;
    public KeyCode grabKey = KeyCode.E;
    public KeyCode placeKey = KeyCode.Mouse1; // Right click to place/scale

    private Grabbable held;
    private float targetDistance;
    private float scaleRefDistance;
    private Vector3 scaleRefLocalScale;

    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }
        targetDistance = holdDistance;
    }

    void Update()
    {
        HandleGrabInput();
        HandleHoldMove();
        HandleDistanceScroll();
        HandlePlaceAndScale();
    }

    void HandleGrabInput()
    {
        if (Input.GetKeyDown(grabKey))
        {
            if (held == null)
            {
                TryGrab();
            }
            else
            {
                Drop(false);
            }
        }

        if (Input.GetMouseButtonDown(0) && held != null)
        {
            Drop(true);
        }
    }

    void TryGrab()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, grabRange, grabbableLayers, QueryTriggerInteraction.Ignore))
        {
            Grabbable grabbable = hit.collider.GetComponentInParent<Grabbable>();
            if (grabbable != null)
            {
                held = grabbable;
                Rigidbody rb = held.GetRigidbody();
                rb.isKinematic = true;
                rb.interpolation = RigidbodyInterpolation.None;
                targetDistance = Mathf.Clamp(Vector3.Distance(playerCamera.transform.position, hit.point), held.minHoldDistance, held.maxHoldDistance);
                // capture scaling reference to preserve screen size when placing
                scaleRefDistance = targetDistance;
                scaleRefLocalScale = held.transform.localScale;
            }
        }
    }

    void Drop(bool throwObject)
    {
        if (held == null) return;
        Rigidbody rb = held.GetRigidbody();
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        if (throwObject)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(playerCamera.transform.forward * throwForce, ForceMode.VelocityChange);
        }
        held = null;
    }

    void HandleHoldMove()
    {
        if (held == null) return;
        Rigidbody rb = held.GetRigidbody();

        Vector3 targetPos = playerCamera.transform.position + playerCamera.transform.forward * targetDistance;
        
        // Use faster, more direct movement
        float smoothing = moveSmoothing * Time.deltaTime;
        Vector3 newPos = Vector3.Lerp(held.transform.position, targetPos, smoothing);
        rb.MovePosition(newPos);

        if (!held.allowRotationWhileHeld)
        {
            Quaternion lookRot = Quaternion.LookRotation(-playerCamera.transform.forward, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(held.transform.rotation, lookRot, smoothing));
        }
    }

    void HandleDistanceScroll()
    {
        if (held == null) return;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            targetDistance = Mathf.Clamp(targetDistance + scroll * 2f, held.minHoldDistance, held.maxHoldDistance);
        }
    }

    void HandlePlaceAndScale()
    {
        if (held == null) return;
        if (!Input.GetKeyDown(placeKey)) return;

        // Raycast to a placement surface
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Min(held.maxHoldDistance, 50f), ~0, QueryTriggerInteraction.Ignore))
        {
            float newDistance = Mathf.Clamp(hit.distance, held.minHoldDistance, held.maxHoldDistance);

            // scale to preserve angular/screen size: scale ~ distance
            if (scaleRefDistance > 0.01f)
            {
                float factor = newDistance / scaleRefDistance;
                held.transform.localScale = scaleRefLocalScale * factor;
            }

            targetDistance = newDistance;

            // place flush on the surface
            Rigidbody rb = held.GetRigidbody();
            Collider col = held.GetComponent<Collider>();
            float pushOut = 0.01f;
            float extent = 0.0f;
            if (col != null)
            {
                // approximate extent along normal using bounds extents
                // this is not exact but good enough for prototype
                extent = Vector3.Scale(col.bounds.extents, Vector3.one).magnitude * 0.5f;
            }
            Vector3 placePos = hit.point + hit.normal * (extent + pushOut);
            rb.MovePosition(placePos);

            // align object to face camera (optional). Alternatively, align to surface normal
            Quaternion align = Quaternion.FromToRotation(held.transform.forward, -playerCamera.transform.forward) * held.transform.rotation;
            rb.MoveRotation(align);
        }
    }
}


