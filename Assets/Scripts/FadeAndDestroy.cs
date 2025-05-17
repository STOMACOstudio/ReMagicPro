using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAndDestroy : MonoBehaviour
{
    public float duration = 1.5f;

    private SpriteRenderer spriteRenderer;
    private float timer = 0f;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(originalColor.a, 0, timer / duration);
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        if (timer >= duration)
            Destroy(gameObject);
    }
}