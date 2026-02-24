using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform playerCamera;

    [Header("Movement")]
    public float maxSpeed = 5f;
    public float acceleration = 12f;
    public float deceleration = 10f;

    [Header("Intro Settings")]
    public float startY = 10f;
    public float targetY = 0.6f;
    public float descentSpeed = 2f;
    public float initialLookDownAngle = -5f; 
    private bool isIntroFinished = false;
    private float landingTimer = 0f;

    [Header("Audio")]
    public AudioSource footstepAudio; 

    [Header("Mouse")]
    public float mouseSensitivity = 1f;

    [Header("Boundary Settings")]
    public float platformRadius = 12.5f; // Changed from HalfSize to Radius

    private float xRotation = 0f;
    private float fixedY;
    private Vector3 currentVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        xRotation = initialLookDownAngle;
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.position = new Vector3(transform.position.x, startY, transform.position.z);
        fixedY = targetY;
    }

    void Update()
    {
        Look();

        if (!isIntroFinished)
        {
            HandleIntro();
        }
        else
        {
            Move();
            HandleFootsteps();
        }
    }

    void HandleIntro()
    {
        float newY = Mathf.MoveTowards(transform.position.y, targetY, descentSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if (Mathf.Abs(transform.position.y - targetY) < 0.001f)
        {
            isIntroFinished = true;
            if (footstepAudio != null)
            {
                footstepAudio.Play();
                landingTimer = 0.2f;
            }
        }
    }

    void Look()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity * 0.1f;
        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseDelta.x);
    }

    void Move()
    {
        float x = (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0);
        float z = (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0);

        Vector3 inputDir = (transform.right * x + transform.forward * z).normalized;
        Vector3 targetVelocity = inputDir * maxSpeed;

        if (inputDir.magnitude > 0)
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
        else
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);

        Vector3 desiredMove = currentVelocity * Time.deltaTime;
        Vector3 nextPos = transform.position + desiredMove;

        // --- NEW CIRCULAR BOUNDARY LOGIC ---
        // Calculate horizontal distance from center (0,0)
        Vector2 flatPos = new Vector2(nextPos.x, nextPos.z);
        float maxAllowedRadius = platformRadius - controller.radius;

        if (flatPos.magnitude > maxAllowedRadius)
        {
            // Clamp the position to the edge of the circle
            flatPos = flatPos.normalized * maxAllowedRadius;
            nextPos.x = flatPos.x;
            nextPos.z = flatPos.y;
        }
        // ------------------------------------

        nextPos.y = fixedY;

        Vector3 finalMove = nextPos - transform.position;
        controller.Move(finalMove);
    }

    void HandleFootsteps()
    {
        if (landingTimer > 0)
        {
            landingTimer -= Time.deltaTime;
            return;
        }

        if (currentVelocity.magnitude > 0.1f)
        {
            if (!footstepAudio.isPlaying) footstepAudio.Play();
        }
        else
        {
            if (footstepAudio.isPlaying) footstepAudio.Pause();
        }
    }
}