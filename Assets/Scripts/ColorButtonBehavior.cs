using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class ColorButtonBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Glow")]
    public Image hoverGlow;
    public Image selectionGlow;

    [Header("Text Outputs")]
    public TextMeshProUGUI descriptionTextTMP;
    public TextMeshProUGUI colorNameTextTMP;
    public CanvasGroup descriptionGroup;
    public CanvasGroup colorNameGroup;

    [Header("Button Data")]
    [TextArea] public string description;
    public string colorName;
    public string colorLabel;
    public Color displayColor;

    [Header("Background")]
    public Image backgroundPanel;
    public Color backgroundColor;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioClip unclickSound;

    private static ColorButtonBehavior currentlySelected;
    private static Image backgroundPanelStatic;
    private static Color targetBGColor;
    private static Color startingBGColor;
    private static float bgFadeSpeed = 2f;

    public GameObject startButton;

    private bool isSelected = false;
    private float hoverAlphaTarget = 0f;
    public float fadeSpeed = 5f;

    void Start()
    {
        SetAlpha(hoverGlow, 0f);
        SetAlpha(selectionGlow, 0f);

        if (backgroundPanelStatic == null && backgroundPanel != null)
        {
            backgroundPanelStatic = backgroundPanel;
            targetBGColor = backgroundPanel.color;
            startingBGColor = backgroundPanel.color;
        }

        if (descriptionGroup != null)
            descriptionGroup.alpha = 0f;

        if (colorNameGroup != null)
            colorNameGroup.alpha = 0f;
    }

    void Update()
    {
        // Smooth hover glow
        if (!isSelected && hoverGlow != null)
        {
            float currentAlpha = hoverGlow.color.a;
            float newAlpha = Mathf.Lerp(currentAlpha, hoverAlphaTarget, Time.deltaTime * fadeSpeed);
            SetAlpha(hoverGlow, newAlpha);
        }

        // Smooth background fade
        if (backgroundPanelStatic != null)
        {
            Color currentColor = backgroundPanelStatic.color;
            backgroundPanelStatic.color = Color.Lerp(currentColor, targetBGColor, Time.deltaTime * bgFadeSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
            hoverAlphaTarget = 1f;

        if (audioSource != null && hoverSound != null)
            audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
            hoverAlphaTarget = 0f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentlySelected == this)
        {
            isSelected = false;
            currentlySelected = null;
            SetAlpha(selectionGlow, 0f);
            hoverAlphaTarget = 0f;
            StartCoroutine(FadeCanvasGroup(descriptionGroup, 0f));
            StartCoroutine(FadeCanvasGroup(colorNameGroup, 0f));
            targetBGColor = startingBGColor;

            // Play unclick sound
            if (audioSource != null && unclickSound != null)
                audioSource.PlayOneShot(unclickSound);

            // Hide the Start Journey button when deselecting
            if (startButton != null)
                startButton.SetActive(false);

            return;
        }

        if (currentlySelected != null)
        {
            currentlySelected.isSelected = false;
            currentlySelected.hoverAlphaTarget = 0f;
            currentlySelected.SetAlpha(currentlySelected.selectionGlow, 0f);
        }

        currentlySelected = this;
        isSelected = true;

        targetBGColor = backgroundColor;

        hoverAlphaTarget = 1f;
        SetAlpha(hoverGlow, 1f);
        SetAlpha(selectionGlow, 1f);

        // Set text content and fade in
        descriptionTextTMP.text = description;
        colorNameTextTMP.text = colorLabel;
        colorNameTextTMP.color = displayColor;

        StartCoroutine(FadeCanvasGroup(descriptionGroup, 1f));
        StartCoroutine(FadeCanvasGroup(colorNameGroup, 1f));

        PlayerPrefs.SetString("PlayerColor", colorName);

        // Play click sound
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);

        // Show the Start Journey button when a color is selected
        if (startButton != null)
            startButton.SetActive(true);
    }

    private void SetAlpha(Image img, float a)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = a;
        img.color = c;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float targetAlpha, float speed = 5f)
    {
        while (!Mathf.Approximately(group.alpha, targetAlpha))
        {
            group.alpha = Mathf.MoveTowards(group.alpha, targetAlpha, Time.deltaTime * speed);
            yield return null;
        }
    }
}