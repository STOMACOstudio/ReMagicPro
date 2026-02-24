using UnityEngine;

public sealed class FloatingObject : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 50, 0);

    [Header("Floating Settings")]
    [SerializeField] private float amplitude = 0.5f; // How high it moves
    [SerializeField] private float frequency = 1f;   // How fast it bobs

    private Vector3 startPosition;

    void Start()
    {
        // Store the initial position so we bob around it
        startPosition = transform.position;
    }

    void Update()
    {
        // 1. Handle Rotation
        transform.Rotate(rotationSpeed * Time.deltaTime);

        // 2. Handle Floating (Sine Wave)
        Vector3 tempPos = startPosition;
        tempPos.y += Mathf.Sin(Time.time * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
    }
}