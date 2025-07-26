using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LibraryHint : MonoBehaviour
{
    public float yOffset = 2f;
    public float fadeSpeed = 5f;

    private Canvas hintCanvas;
    private CanvasGroup canvasGroup;
    private TextMeshProUGUI hintText;
    private Coroutine fadeRoutine;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        if (cam == null)
            cam = FindObjectOfType<Camera>();

        // Canvas to hold the text in world space
        GameObject canvasObj = new GameObject("LibraryHintCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = new Vector3(0f, yOffset, 0f);
        canvasObj.transform.localRotation = Quaternion.identity;

        hintCanvas = canvasObj.AddComponent<Canvas>();
        hintCanvas.renderMode = RenderMode.WorldSpace;
        hintCanvas.worldCamera = cam;

        canvasObj.AddComponent<CanvasScaler>();
        canvasGroup = canvasObj.AddComponent<CanvasGroup>();

        RectTransform canvasRect = hintCanvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1f, 0.3f);

        GameObject textObj = new GameObject("LibraryHintText");
        textObj.transform.SetParent(canvasObj.transform, false);
        hintText = textObj.AddComponent<TextMeshProUGUI>();
        hintText.text = "Library";
        hintText.alignment = TextAlignmentOptions.Center;
        hintText.fontSize = 36f;
        hintText.rectTransform.sizeDelta = new Vector2(200f, 60f);

        SetAlpha(0f);
    }

    void Update()
    {
        if (hintCanvas != null && cam != null)
        {
            hintCanvas.transform.forward = cam.transform.forward;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade(1f);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade(0f);
        }
    }

    void StartFade(float target)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeRoutine(target));
    }

    IEnumerator FadeRoutine(float target)
    {
        while (canvasGroup != null && !Mathf.Approximately(canvasGroup.alpha, target))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime * fadeSpeed);
            yield return null;
        }
    }

    void SetAlpha(float a)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = a;
        }
    }
}
