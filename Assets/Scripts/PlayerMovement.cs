using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

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

    [Header("Intro Settings")]
    public float startY = 10f;
    public float targetY = 0.6f;
    public float descentSpeed = 2f;
    public float initialLookDownAngle = -5f;
    
    [Header("Intro Narrative")]
    public SubtitleManager subtitleManager;
    public List<SubtitleManager.SubtitleLine> introLines;

    [Header("Boundary Settings")]
    public float platformRadius = 12.5f;

    private float xRotation = 0f;
    private float fixedY;
    private Vector3 currentVelocity;
    private bool isIntroFinished = false;
    private bool subtitlesStarted = false;
    private float landingTimer = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;

        if (controller == null || playerCamera == null)
        {
            Debug.LogError($"[{nameof(PlayerMovement)}] Missing required references on {gameObject.name}.", this);
            enabled = false;
            return;
        }
        
        // Setup initial camera and position
        xRotation = initialLookDownAngle;
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.position = new Vector3(transform.position.x, startY, transform.position.z);
        
        fixedY = targetY;

        // Auto-hook SubtitleManager if not assigned
        if (subtitleManager == null)
            subtitleManager = Object.FindFirstObjectByType<SubtitleManager>();
    }

    void Update()
    {
        if (DeckHolder.IsDeckEditorOpenedAdditively)
        {
            StopFootsteps();
            return;
        }

        Look();

        if (!isIntroFinished)
        {
            HandleIntro();
        }
        else
        {
            Move();
            HandleFootsteps();
            HandleDeckEditorShortcut();
        }
    }

    void HandleDeckEditorShortcut()
    {
        if (Keyboard.current == null || !Keyboard.current.eKey.wasPressedThisFrame)
            return;

        if (!DeckHolder.IsStarterDeckRewardCollected || DeckHolder.SelectedDeck == null || DeckHolder.SelectedDeck.Count == 0)
            return;

        DeckHolder.DeckEditorReturnSceneName = SceneManager.GetActiveScene().name;
        DeckHolder.IsDeckEditorOpenedAdditively = true;
        DeckHolder.RestoreGameplayCursorOnDeckEditorClose = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(OpenDeckEditorScene());
    }

    IEnumerator OpenDeckEditorScene()
    {
        EventSystem currentEventSystem = EventSystem.current;
        if (currentEventSystem != null)
            currentEventSystem.enabled = false;

        Scene deckEditorScene = SceneManager.GetSceneByName(DeckEditorSceneName);

        if (!deckEditorScene.IsValid() || !deckEditorScene.isLoaded)
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(DeckEditorSceneName, LoadSceneMode.Additive);
            if (loadOperation != null)
            {
                yield return loadOperation;
            }

            deckEditorScene = SceneManager.GetSceneByName(DeckEditorSceneName);
        }

        if (deckEditorScene.IsValid() && deckEditorScene.isLoaded)
        {
            EventSystemUtility.EnableOnlyForScene(deckEditorScene);
            EventSystemUtility.EnsureSingleAudioListener(deckEditorScene);
            SceneManager.SetActiveScene(deckEditorScene);
        }
    }

    void HandleIntro()
    {
        // Trigger multi-line subtitle sequence through the manager
        if (!subtitlesStarted && subtitleManager != null)
        {
            subtitlesStarted = true;
            subtitleManager.DisplaySequence(introLines);
        }

        // Descend to floor
        float newY = Mathf.MoveTowards(transform.position.y, targetY, descentSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if (Mathf.Abs(transform.position.y - targetY) < 0.001f)
        {
            isIntroFinished = true;
            if (footstepAudio != null)
            {
                footstepAudio.Play();
                landingTimer = 0.2f; // Slight delay for first movement sounds
            }
        }
    }

    void Look()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity * 0.1f;
        
        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseDelta.x);
    }

    void Move()
    {
        if (Keyboard.current == null)
            return;

        // Calculate input
        float x = (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0);
        float z = (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0);

        Vector3 inputDir = (transform.right * x + transform.forward * z).normalized;
        Vector3 targetVelocity = inputDir * maxSpeed;

        // Apply acceleration/deceleration
        float lerpSpeed = (inputDir.magnitude > 0) ? acceleration : deceleration;
        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, lerpSpeed * Time.deltaTime);

        // Circular Boundary Constraint
        Vector3 nextPos = transform.position + (currentVelocity * Time.deltaTime);
        Vector2 flatPos = new Vector2(nextPos.x, nextPos.z);
        float maxAllowedRadius = platformRadius - controller.radius;

        if (flatPos.magnitude > maxAllowedRadius)
        {
            flatPos = flatPos.normalized * maxAllowedRadius;
            nextPos.x = flatPos.x;
            nextPos.z = flatPos.y;
        }

        nextPos.y = fixedY;

        // Move the controller
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

    void StopFootsteps()
    {
        if (footstepAudio != null && footstepAudio.isPlaying)
            footstepAudio.Stop();
    }
}
