using UnityEngine;
using UnityEngine.UI;

public class TriggerPulseVFX : MonoBehaviour
{
    [Header("Pulse Settings")]
    public float scaleSpeed = 2f;        // How fast the scaling happens
    public float scaleAmount = 0.1f;     // Max scale change from original
    public float alphaSpeed = 2f;        // How fast the alpha changes
    public float alphaAmount = 0.5f;     // Amount of alpha change (0–1 range)

    private Image image;
    private Vector3 originalScale;
    private float originalAlpha;

    void Start()
    {
        image = GetComponentInChildren<Image>();
        if (image == null)
        {
            Debug.LogError("PulsatingImage: No Image component found on this GameObject!");
            enabled = false;
            return;
        }

        originalScale = transform.localScale;
        originalAlpha = image.color.a;
    }

    void Update()
    {
        // Pulsate scale
        float scaleOffset = Mathf.Sin(Time.time * scaleSpeed) * scaleAmount;
        transform.localScale = originalScale * (1f + scaleOffset);

        // Pulsate alpha
        Color c = image.color;
        c.a = originalAlpha + Mathf.Sin(Time.time * alphaSpeed) * alphaAmount;
        image.color = c;
    }
}
