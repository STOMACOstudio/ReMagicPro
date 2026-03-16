using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenFadeTransition : MonoBehaviour
{
    private static ScreenFadeTransition instance;

    [SerializeField] private float defaultFadeDuration = 0.65f;

    private Canvas overlayCanvas;
    private Image overlayImage;
    private Coroutine activeFadeCoroutine;
    private string pendingFadeInScene;

    public static ScreenFadeTransition Instance
    {
        get
        {
            if (instance == null)
                CreateInstance();

            return instance;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (instance == null)
            CreateInstance();
    }

    private static void CreateInstance()
    {
        GameObject transitionObject = new GameObject(nameof(ScreenFadeTransition));
        instance = transitionObject.AddComponent<ScreenFadeTransition>();
        DontDestroyOnLoad(transitionObject);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureOverlay();
    }

    private void OnDestroy()
    {
        if (instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void FadeToScene(string sceneName, float fadeDuration = -1f)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return;

        float duration = fadeDuration > 0f ? fadeDuration : defaultFadeDuration;
        pendingFadeInScene = sceneName;

        if (activeFadeCoroutine != null)
            StopCoroutine(activeFadeCoroutine);

        activeFadeCoroutine = StartCoroutine(FadeOutAndLoadScene(sceneName, duration));
    }

    private IEnumerator FadeOutAndLoadScene(string sceneName, float duration)
    {
        EnsureOverlay();

        overlayImage.raycastTarget = true;
        yield return Fade(overlayImage.color.a, 1f, duration);
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene loadedScene, LoadSceneMode mode)
    {
        bool shouldFadeIn = loadedScene.name == pendingFadeInScene || loadedScene.name == "TutorialScene";
        pendingFadeInScene = null;

        if (!shouldFadeIn)
            return;

        if (activeFadeCoroutine != null)
            StopCoroutine(activeFadeCoroutine);

        EnsureOverlay();
        overlayImage.color = new Color(0f, 0f, 0f, 1f);
        activeFadeCoroutine = StartCoroutine(FadeFromBlack(defaultFadeDuration));
    }

    private IEnumerator FadeFromBlack(float duration)
    {
        yield return Fade(1f, 0f, duration);
        overlayImage.raycastTarget = false;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        EnsureOverlay();

        if (duration <= 0f)
        {
            SetOverlayAlpha(to);
            activeFadeCoroutine = null;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            SetOverlayAlpha(Mathf.Lerp(from, to, t));
            yield return null;
        }

        SetOverlayAlpha(to);
        activeFadeCoroutine = null;
    }

    private void SetOverlayAlpha(float alpha)
    {
        Color color = overlayImage.color;
        color.a = alpha;
        overlayImage.color = color;
    }

    private void EnsureOverlay()
    {
        if (overlayCanvas != null && overlayImage != null)
            return;

        GameObject canvasObject = new GameObject("ScreenFadeCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);

        overlayCanvas = canvasObject.GetComponent<Canvas>();
        overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        overlayCanvas.sortingOrder = short.MaxValue;

        CanvasScaler canvasScaler = canvasObject.GetComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);

        GameObject imageObject = new GameObject("FadeOverlay", typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(canvasObject.transform, false);

        RectTransform imageRect = imageObject.GetComponent<RectTransform>();
        imageRect.anchorMin = Vector2.zero;
        imageRect.anchorMax = Vector2.one;
        imageRect.offsetMin = Vector2.zero;
        imageRect.offsetMax = Vector2.zero;

        overlayImage = imageObject.GetComponent<Image>();
        overlayImage.color = new Color(0f, 0f, 0f, 0f);
        overlayImage.raycastTarget = false;
    }
}
