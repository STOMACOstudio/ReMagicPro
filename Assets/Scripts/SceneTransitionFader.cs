using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionFader : MonoBehaviour
{
    private CanvasGroup overlay;
    private float fadeInDuration;
    private bool fadeInScheduled;

    public static SceneTransitionFader Create(Color color)
    {
        GameObject overlayRoot = new GameObject("SceneTransitionFade");
        DontDestroyOnLoad(overlayRoot);

        Canvas canvas = overlayRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10000;
        overlayRoot.AddComponent<GraphicRaycaster>();

        CanvasGroup canvasGroup = overlayRoot.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        GameObject imageRoot = new GameObject("FadeImage");
        imageRoot.transform.SetParent(overlayRoot.transform, false);

        RectTransform imageRect = imageRoot.AddComponent<RectTransform>();
        imageRect.anchorMin = Vector2.zero;
        imageRect.anchorMax = Vector2.one;
        imageRect.offsetMin = Vector2.zero;
        imageRect.offsetMax = Vector2.zero;

        Image fadeImage = imageRoot.AddComponent<Image>();
        fadeImage.color = color;

        SceneTransitionFader fader = overlayRoot.AddComponent<SceneTransitionFader>();
        fader.overlay = canvasGroup;

        return fader;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public IEnumerator Fade(float from, float to, float duration)
    {
        if (overlay == null)
            yield break;

        if (duration <= 0f)
        {
            overlay.alpha = to;
            yield break;
        }

        float elapsed = 0f;
        overlay.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            overlay.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        overlay.alpha = to;
    }

    public void ScheduleFadeInAfterSceneLoad(float duration)
    {
        fadeInDuration = duration;
        fadeInScheduled = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!fadeInScheduled)
            return;

        fadeInScheduled = false;
        StartCoroutine(FadeInAndDestroy());
    }

    private IEnumerator FadeInAndDestroy()
    {
        yield return null;
        yield return Fade(1f, 0f, fadeInDuration);
        Destroy(gameObject);
    }
}
