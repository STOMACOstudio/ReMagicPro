using System.Collections;
using UnityEngine;

public class TurnBanner : MonoBehaviour
{
    public CanvasGroup group;
    public float fadeDuration = 0.5f;
    public float displayTime = 1f;

    private void OnEnable()
    {
        StartCoroutine(PlayEffect());
    }

    private IEnumerator PlayEffect()
    {
        yield return Fade(0f, 1f);
        yield return new WaitForSeconds(displayTime);
        yield return Fade(1f, 0f);
        gameObject.SetActive(false);
    }

    IEnumerator Fade(float start, float end)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, end, t / fadeDuration);
            yield return null;
        }
        group.alpha = end;
    }
}
