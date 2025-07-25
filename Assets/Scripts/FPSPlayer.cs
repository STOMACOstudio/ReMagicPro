using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class FPSPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    private Rigidbody rb;
    private float verticalLookRotation = 0f;
    private Vector3 movementInput;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // prevent physics from rotating the player

        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cameraTransform = cam.transform;
            }
            else
            {
                Debug.LogWarning("FPSPlayer cameraTransform is not assigned and no child camera found.");
            }
        }
    }

    void Update()
    {
        // Input
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        movementInput = (transform.right * x + transform.forward * z).normalized * moveSpeed;

        // Mouse Look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
        if (cameraTransform != null)
        {
            cameraTransform.localEulerAngles = Vector3.right * verticalLookRotation;
        }
    }

    void FixedUpdate()
    {
        // Apply movement via Rigidbody
        Vector3 newPosition = rb.position + movementInput * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }
}