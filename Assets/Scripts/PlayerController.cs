using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("General Settings")]
    private CharacterController controller;
    private Camera playerCamera;

    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float sprintSpeed = 10f;
    public float airControlFactor = 0.5f; // 0 = keine Luftkontrolle, 1 = volle Kontrolle

    [Header("Jump & Gravity Settings")]
    public float jumpHeight = 1.5f;
    public float gravityStrength = 20f; // Schwerkraft als POSITIVER Wert
    public float coyoteTime = 0.1f; // Zeitfenster nach Verlassen des Bodens
    public float jumpBufferTime = 0.1f; // Zeitfenster vor Bodenkontakt

    [Header("Look Settings")]
    public float mouseSensitivity = 2f;
    public float fov = 60f;
    public float sprintFov = 70f;
    public float jumpFov = 65f;
    public float fovTransitionSpeed = 5f;
    private float xRotation = 0f;

    // Private Variablen für die interne Logik
    private Vector3 playerVelocity;
    private bool isGrounded;
    private float currentFov;
    private float headBobTimer;
    private Vector3 originalCameraPosition;
    public bool isSprinting;
    private float lastTimeGrounded;
    private float lastTimeJumpPressed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        
        if (playerCamera != null)
        {
            currentFov = fov;
            playerCamera.fieldOfView = currentFov;
            originalCameraPosition = playerCamera.transform.localPosition;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        ProcessGroundCheck();
        ProcessMovement();
        ProcessMouseLook();
        ProcessHeadBob();
        ProcessFovEffects();
    }

    private void ProcessGroundCheck()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        if (isGrounded)
        {
            lastTimeGrounded = Time.time;
        }
    }

    private void ProcessMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * horizontalInput + transform.forward * verticalInput;
        
        isSprinting = Input.GetKey(KeyCode.LeftShift) && (horizontalInput != 0 || verticalInput != 0);
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        // Weniger Kontrolle in der Luft
        float control = isGrounded ? 1f : airControlFactor;
        controller.Move(moveDirection.normalized * (currentSpeed * control) * Time.deltaTime);

        // Sprunglogik
        if (Input.GetButtonDown("Jump"))
        {
            lastTimeJumpPressed = Time.time;
        }

        // Schwerkraft anwenden (jetzt mit Subtraktion)
        playerVelocity.y -= gravityStrength * Time.deltaTime;

        controller.Move(playerVelocity * Time.deltaTime);

        // Jump Buffer + Coyote Time anwenden
        bool withinCoyote = Time.time - lastTimeGrounded <= coyoteTime;
        bool bufferedJump = Time.time - lastTimeJumpPressed <= jumpBufferTime;
        if (bufferedJump && (isGrounded || withinCoyote))
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * 2f * gravityStrength);
            lastTimeJumpPressed = -999f; // Buffer verbrauchen
        }
    }

    private void ProcessMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void ProcessHeadBob()
    {
        if (playerCamera == null) return;

        float speed = controller.velocity.magnitude;
        bool isMoving = speed > 0.1f && isGrounded;

        if (isMoving)
        {
            float bobSpeed = isSprinting ? 1.2f : 0.8f;
            float bobAmount = isSprinting ? 0.08f : 0.05f;
            
            headBobTimer += Time.deltaTime * bobSpeed;
            
            float headBobY = Mathf.Sin(headBobTimer) * bobAmount;
            float headBobX = Mathf.Sin(headBobTimer * 0.5f) * bobAmount * 0.3f;
            
            Vector3 headBobOffset = new Vector3(headBobX, headBobY, 0f);
            playerCamera.transform.localPosition = originalCameraPosition + headBobOffset;
        }
        else
        {
            headBobTimer = 0f;
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, originalCameraPosition, Time.deltaTime * 8f);
        }
    }

    private void ProcessFovEffects()
    {
        if (playerCamera == null) return;

        float targetFov = fov;
        
        if (isSprinting)
        {
            targetFov = sprintFov;
        }
        else if (!isGrounded && playerVelocity.y > 0)
        {
            targetFov = jumpFov;
        }

        currentFov = Mathf.Lerp(currentFov, targetFov, Time.deltaTime * fovTransitionSpeed);
        playerCamera.fieldOfView = currentFov;
    }
}