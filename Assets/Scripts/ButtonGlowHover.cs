using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonGlowHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image overlayImage;
    public TMP_Text targetText;
    public Color normalTextColor = Color.white;
    public Color hoverTextColor = Color.yellow;
    public float hoverDelay = 0.08f;
    public float fadeSpeed = 8f;
    public AudioClip hoverSound;

    private float targetAlpha = 0f;
    private float textBlendTarget = 0f;
    private float textBlend = 0f;
    private bool isHovering;
    private float hoverTimer;

    void Start()
    {
        if (targetText == null)
            targetText = GetComponentInChildren<TMP_Text>(true);

        if (targetText != null)
        {
            normalTextColor = targetText.color;
            targetText.color = normalTextColor;
        }
    }

    void Update()
    {
        if (isHovering)
            hoverTimer += Time.unscaledDeltaTime;

        textBlendTarget = isHovering && hoverTimer >= hoverDelay ? 1f : 0f;

        if (overlayImage != null)
        {
            Color c = overlayImage.color;
            c.a = Mathf.Lerp(c.a, targetAlpha, Time.unscaledDeltaTime * fadeSpeed);
            overlayImage.color = c;
        }

        if (targetText != null)
        {
            textBlend = Mathf.Lerp(textBlend, textBlendTarget, Time.unscaledDeltaTime * fadeSpeed);
            targetText.color = Color.Lerp(normalTextColor, hoverTextColor, textBlend);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        hoverTimer = 0f;
        targetAlpha = 1f;

        AudioClip clip = hoverSound;
        if (clip == null && SoundManager.Instance != null)
            clip = SoundManager.Instance.buttonClick;

        if (clip != null && SoundManager.Instance != null)
            SoundManager.Instance.PlaySound(clip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        hoverTimer = 0f;
        targetAlpha = 0f;
    }
}
