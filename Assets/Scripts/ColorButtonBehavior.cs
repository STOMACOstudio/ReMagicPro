using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ColorButtonBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Glow")]
    public Image hoverGlow;
    public Image selectionGlow;

    [Header("Text Outputs")]
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI colorNameText;

    [Header("Button Data")]
    [TextArea] public string description;
    public string colorName;
    public string colorLabel;
    public Color displayColor;

    [Header("Background")]
    public Image backgroundPanel;
    public Color backgroundColor;

    private static ColorButtonBehavior currentlySelected;
    private static Image backgroundPanelStatic;
    private static Color targetBGColor;
    private static Color startingBGColor;
    private static float bgFadeSpeed = 2f;

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
            startingBGColor = backgroundPanel.color; // save original
        }
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
            descriptionText.text = "";
            colorNameText.text = "";
            targetBGColor = startingBGColor; // Revert background
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

        descriptionText.text = description;
        colorNameText.text = colorLabel;
        colorNameText.color = displayColor;

        PlayerPrefs.SetString("PlayerColor", colorName);
    }

    private void SetAlpha(Image img, float a)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = a;
        img.color = c;
    }
}