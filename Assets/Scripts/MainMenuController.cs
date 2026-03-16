using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private AudioClip startGameSound;
    [Header("Transition Fade")]
    [SerializeField] private float fadeToBlackDuration = 2.5f;
    [SerializeField] private float tutorialFadeInDuration = 1.75f;

    private bool isTransitioning;

    void Start()
    {
        ClearAllSaves();
    }

    public void PlayGame()
    {
        if (isTransitioning)
            return;

        isTransitioning = true;

        AudioClip clip = startGameSound;
        if (clip == null && SoundManager.Instance != null)
            clip = SoundManager.Instance.buttonClick;

        if (clip != null && SoundManager.Instance != null)
            SoundManager.Instance.PlaySound(clip);

        StartCoroutine(PlayGameWithFade());
    }

    private IEnumerator PlayGameWithFade()
    {
        CanvasGroup overlay = CreateFadeOverlay();

        yield return FadeOverlay(overlay, 0f, 1f, fadeToBlackDuration);

        AsyncOperation loadTutorial = SceneManager.LoadSceneAsync("TutorialScene");
        while (!loadTutorial.isDone)
            yield return null;

        yield return null;
        yield return FadeOverlay(overlay, 1f, 0f, tutorialFadeInDuration);

        if (overlay != null)
            Destroy(overlay.gameObject);
    }

    private CanvasGroup CreateFadeOverlay()
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
        fadeImage.color = Color.black;

        return canvasGroup;
    }

    private IEnumerator FadeOverlay(CanvasGroup overlay, float from, float to, float duration)
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

    public void OpenOptions()
    {
        Debug.Log("Options clicked");
    }

    public void OpenCredits()
    {
        Debug.Log("Credits clicked");
    }

    void ClearAllSaves()
    {
        Debug.LogWarning("[DEV] Clearing all PlayerPrefs!");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Reset any generated deck and previously selected colors
        DeckHolder.SelectedDeck = null;
        DeckHolder.IsStarterDeckRewardCollected = false;
        ColorButtonBehavior.ResetSelections();
    }
}
