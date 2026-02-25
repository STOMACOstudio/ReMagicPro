using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(AudioSource))]
public class PlatformTrigger : MonoBehaviour
{
    [Header("Setup")]
    public string playerTag = "Player";
    public Material activeMaterial;

    private Material originalMaterial;
    private Renderer rend;
    private AudioSource audioSource;

    private int playerCount = 0; // handles multiple colliders safely

    void Awake()
    {
        rend = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();

        originalMaterial = rend.material;

        // Ensure collider is trigger
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerCount++;

        if (playerCount == 1)
        {
            if (activeMaterial != null)
                rend.material = activeMaterial;

            if (audioSource != null)
                audioSource.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerCount--;

        if (playerCount <= 0)
        {
            playerCount = 0;
            rend.material = originalMaterial;
        }
    }
}