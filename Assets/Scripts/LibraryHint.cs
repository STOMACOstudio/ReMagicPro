using System.Collections;
using TMPro;
using UnityEngine;

public class LibraryHint : MonoBehaviour
{
    public float yOffset = 2f;
    public float fadeSpeed = 5f;

    private TextMeshPro hintText;
    private Coroutine fadeRoutine;

    void Start()
    {
        // Create the hint text at runtime as a child of this object
        GameObject textObj = new GameObject("LibraryHintText");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = new Vector3(0f, yOffset, 0f);
        textObj.transform.localRotation = Quaternion.identity;

        hintText = textObj.AddComponent<TextMeshPro>();
        hintText.text = "Library";
        hintText.font = TMP_Settings.defaultFontAsset;
        hintText.alignment = TextAlignmentOptions.Center;
        hintText.fontSize = 3f;

        SetAlpha(0f);
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
        while (hintText != null && !Mathf.Approximately(hintText.color.a, target))
        {
            Color c = hintText.color;
            c.a = Mathf.MoveTowards(c.a, target, Time.deltaTime * fadeSpeed);
            hintText.color = c;
            yield return null;
        }
    }

    void SetAlpha(float a)
    {
        if (hintText != null)
        {
            Color c = hintText.color;
            c.a = a;
            hintText.color = c;
        }
    }
}
