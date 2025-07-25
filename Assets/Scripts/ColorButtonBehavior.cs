using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

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

    // Only one color can be selected at a time
    private static ColorButtonBehavior selectedColor = null;
    private static Image backgroundPanelStatic;
    private static Color targetBGColor;
    private static Color startingBGColor;
    private static float bgFadeSpeed = 2f;

    public static void ResetSelections()
    {
        selectedColor = null;
        backgroundPanelStatic = null;
    }

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
            // Deselect if this button is already selected
            if (selectedColor == this)
            {
                selectedColor = null;
                isSelected = false;
                SetAlpha(selectionGlow, 0f);
                hoverAlphaTarget = 0f;

                if (audioSource != null && unclickSound != null)
                    audioSource.PlayOneShot(unclickSound);

                if (startButton != null)
                    startButton.SetActive(false);

                UpdateColorInfoDisplay();
                SaveSelectedColor();
                return;
            }

            // If another color is selected, clear it
            if (selectedColor != null)
            {
                selectedColor.isSelected = false;
                selectedColor.SetAlpha(selectedColor.selectionGlow, 0f);
                selectedColor.hoverAlphaTarget = 0f;
            }

            selectedColor = this;
            isSelected = true;

            targetBGColor = backgroundColor;
            hoverAlphaTarget = 1f;
            SetAlpha(hoverGlow, 1f);
            SetAlpha(selectionGlow, 1f);

            if (audioSource != null && clickSound != null)
                audioSource.PlayOneShot(clickSound);

            if (startButton != null)
                startButton.SetActive(true);

            UpdateColorInfoDisplay();
            SaveSelectedColor();
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
    
    private void SaveSelectedColor()
        {
            if (selectedColor == null)
            {
                PlayerPrefs.DeleteKey("PlayerColor");
            }
            else
            {
                PlayerPrefs.SetString("PlayerColor", selectedColor.colorName);
            }
        }
    
    private void UpdateColorInfoDisplay()
        {
            if (selectedColor == null)
            {
                colorNameTextTMP.text = "";
                descriptionTextTMP.text = "";
                colorNameGroup.alpha = 0f;
                descriptionGroup.alpha = 0f;
                return;
            }

            // Display the selected color's name and description
            colorNameTextTMP.text = $"<color=#{ColorUtility.ToHtmlStringRGB(selectedColor.displayColor)}>{selectedColor.colorLabel}</color>";
            descriptionTextTMP.text = selectedColor.description;
            colorNameGroup.alpha = 1f;
            descriptionGroup.alpha = 1f;
        }
}