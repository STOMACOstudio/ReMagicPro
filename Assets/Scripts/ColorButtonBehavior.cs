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

    private static List<ColorButtonBehavior> selectedColors = new List<ColorButtonBehavior>();
    private const int MaxSelectedColors = 2;
    private static Image backgroundPanelStatic;
    private static Color targetBGColor;
    private static Color startingBGColor;
    private static float bgFadeSpeed = 2f;

    public static void ResetSelections()
    {
        selectedColors.Clear();
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
            if (selectedColors.Contains(this))
            {
                selectedColors.Remove(this);
                isSelected = false;
                SetAlpha(selectionGlow, 0f);
                hoverAlphaTarget = 0f;

                if (audioSource != null && unclickSound != null)
                    audioSource.PlayOneShot(unclickSound);

                if (selectedColors.Count == 0 && startButton != null)
                    startButton.SetActive(false);

                UpdateColorInfoDisplay();
                SaveSelectedColors();
                return;
            }

            if (selectedColors.Count >= MaxSelectedColors)
            {
                // Optional: give feedback that you can't select more than two
                return;
            }

            selectedColors.Add(this);
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
            SaveSelectedColors();
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
    
    private void SaveSelectedColors()
        {
            var colors = selectedColors.ConvertAll(b => b.colorName);
            string joined = string.Join(",", colors);
            PlayerPrefs.SetString("PlayerColor", joined);
        }
    
    private void UpdateColorInfoDisplay()
        {
            if (selectedColors.Count == 0)
            {
                colorNameTextTMP.text = "";
                descriptionTextTMP.text = "";
                colorNameGroup.alpha = 0f;
                descriptionGroup.alpha = 0f;
                return;
            }

            // Build combined name and description
            string nameText = "";
            string descText = "";
            foreach (var color in selectedColors)
            {
                nameText += $"<color=#{ColorUtility.ToHtmlStringRGB(color.displayColor)}>{color.colorLabel}</color>\n";
                descText += $"{color.description}\n";
            }

            colorNameTextTMP.text = nameText.Trim();
            descriptionTextTMP.text = descText.Trim();
            colorNameGroup.alpha = 1f;
            descriptionGroup.alpha = 1f;
        }
}