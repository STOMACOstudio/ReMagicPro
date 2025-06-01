using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeAndDestroy : MonoBehaviour
{
    public float duration = 1.5f;

    private SpriteRenderer spriteRenderer;
    private Image uiImage;
    private float timer = 0f;
    private Color originalColor;

    void Start()
    {
        // Try to get both (won't crash if one is missing)
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        uiImage = GetComponentInChildren<Image>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        else if (uiImage != null)
        {
            originalColor = uiImage.color;
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer or Image found on " + gameObject.name);
            Destroy(gameObject);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(originalColor.a, 0f, timer / duration);
        Color faded = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        if (spriteRenderer != null)
            spriteRenderer.color = faded;
        else if (uiImage != null)
            uiImage.color = faded;

        if (timer >= duration)
            Destroy(gameObject);
    }
}