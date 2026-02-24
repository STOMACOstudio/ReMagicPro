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
    public float startY = 10f;       // Height where the player starts
    public float targetY = 0.6f;     // Height where movement unlocks
    public float descentSpeed = 2f;  // How fast the player floats down
    private bool isIntroFinished = false;

    [Header("Audio")]
    public AudioSource footstepAudio; 

    [Header("Mouse")]
    public float mouseSensitivity = 1f;

    [Header("Boundary Settings")]
    public float platformHalfSize = 12.5f;

    private float xRotation = 0f;
    private float fixedY;
    private Vector3 currentVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        // Set the initial starting position for the intro
        transform.position = new Vector3(transform.position.x, startY, transform.position.z);
        
        // We set fixedY to targetY so once the intro is done, 
        // the player stays at the correct height.
        fixedY = targetY;
    }

    void Update()
    {
        // Looking is always allowed
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
        // Move the player downwards
        float newY = Mathf.MoveTowards(transform.position.y, targetY, descentSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Check if we've reached the target height
        if (Mathf.Abs(transform.position.y - targetY) < 0.001f)
        {
            isIntroFinished = true;
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

        float padding = controller.radius;
        nextPos.x = Mathf.Clamp(nextPos.x, -platformHalfSize + padding, platformHalfSize - padding);
        nextPos.z = Mathf.Clamp(nextPos.z, -platformHalfSize + padding, platformHalfSize - padding);
        nextPos.y = fixedY;

        Vector3 finalMove = nextPos - transform.position;
        controller.Move(finalMove);
    }

    void HandleFootsteps()
    {
        if (currentVelocity.magnitude > 0.1f)
        {
            if (!footstepAudio.isPlaying)
            {
                footstepAudio.Play();
            }
        }
        else
        {
            if (footstepAudio.isPlaying)
            {
                footstepAudio.Pause();
            }
        }
    }
}