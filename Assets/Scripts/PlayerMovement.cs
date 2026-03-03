using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    private const string DeckEditorSceneName = "DeckEditorScene";

    [Header("Components")]
    public CharacterController controller;
    public Transform playerCamera;
    public AudioSource footstepAudio;

    [Header("Movement Settings")]
    public float maxSpeed = 5f;
    public float acceleration = 12f;
    public float deceleration = 10f;
    public float mouseSensitivity = 1f;

    [Header("Boundary Settings")]
    public bool constrainToBoundary = false;
    public float platformRadius = 12.5f;
    public Vector3 boundaryCenter = Vector3.zero;

    private float xRotation = 0f;
    private float fixedY;
    private Vector3 currentVelocity;
    private bool movementEnabled = true;
    private float footstepDelayTimer = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        fixedY = transform.position.y;

        if (controller == null)
            controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Look();

        if (movementEnabled)
        {
            Move();
            HandleFootsteps();
            HandleDeckEditorShortcut();
        }
        else if (footstepAudio != null && footstepAudio.isPlaying)
        {
            footstepAudio.Pause();
        }
    }

    void HandleDeckEditorShortcut()
    {
        if (Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame)
            return;

        if (!DeckHolder.IsStarterDeckRewardCollected || DeckHolder.SelectedDeck == null || DeckHolder.SelectedDeck.Count == 0)
            return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(DeckEditorSceneName);
    }

    void Look()
    {
        if (Mouse.current == null || playerCamera == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity * 0.1f;

        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseDelta.x);
    }

    void Move()
    {
        if (controller == null || Keyboard.current == null)
            return;

        float x = (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0);
        float z = (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0);

        Vector3 inputDir = (transform.right * x + transform.forward * z).normalized;
        Vector3 targetVelocity = inputDir * maxSpeed;

        float lerpSpeed = (inputDir.magnitude > 0) ? acceleration : deceleration;
        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, lerpSpeed * Time.deltaTime);

        Vector3 nextPos = transform.position + (currentVelocity * Time.deltaTime);

        if (constrainToBoundary)
        {
            Vector2 offset = new Vector2(nextPos.x - boundaryCenter.x, nextPos.z - boundaryCenter.z);
            float maxAllowedRadius = Mathf.Max(0f, platformRadius - controller.radius);

            if (offset.magnitude > maxAllowedRadius)
            {
                offset = offset.normalized * maxAllowedRadius;
                nextPos.x = boundaryCenter.x + offset.x;
                nextPos.z = boundaryCenter.z + offset.y;
            }
        }

        nextPos.y = fixedY;

        Vector3 finalMove = nextPos - transform.position;
        controller.Move(finalMove);
    }

    void HandleFootsteps()
    {
        if (footstepAudio == null)
            return;

        if (footstepDelayTimer > 0)
        {
            footstepDelayTimer -= Time.deltaTime;
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

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
        if (!enabled)
            currentVelocity = Vector3.zero;
    }

    public void SetFixedY(float y)
    {
        fixedY = y;
    }

    public void SetLookPitch(float pitch)
    {
        xRotation = Mathf.Clamp(pitch, -90f, 90f);
        if (playerCamera != null)
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    public void SetBoundary(bool enabled, float radius, Vector3 center)
    {
        constrainToBoundary = enabled;
        platformRadius = radius;
        boundaryCenter = center;
    }

    public void SetFootstepDelay(float delaySeconds)
    {
        footstepDelayTimer = Mathf.Max(0f, delaySeconds);
    }
}
