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

    [Header("Audio")] // <--- Nuova sezione
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
        fixedY = transform.position.y;
    }

    void Update()
    {
        Look();
        Move();
        HandleFootsteps(); // <--- Chiamata al nuovo metodo
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

    // <--- Nuovo Metodo per i passi
    void HandleFootsteps()
    {
        // Controlliamo se il giocatore si sta muovendo (velocità > soglia minima)
        // e se è a terra (usando la logica del controller)
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