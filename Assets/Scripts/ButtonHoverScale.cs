using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scaleAmount = 1.1f;
    public float speed = 10f;
    private Vector3 originalScale;
    private Vector3 targetScale;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * scaleAmount;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }
}