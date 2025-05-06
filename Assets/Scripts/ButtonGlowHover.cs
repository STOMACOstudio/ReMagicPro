using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonGlowHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image overlayImage;
    public float fadeSpeed = 5f;
    private float targetAlpha = 0f;

    void Update()
    {
        if (overlayImage != null)
        {
            Color c = overlayImage.color;
            c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
            overlayImage.color = c;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetAlpha = 1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetAlpha = 0f;
    }
}