using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private AudioClip startGameSound;
    [Header("Transition Fade")]
    [SerializeField, Min(0f)] private float fadeToBlackDuration = 2.5f;
    [SerializeField, Min(0f)] private float tutorialFadeInDuration = 1.75f;

    private const float DefaultFadeToBlackDuration = 2.5f;
    private const float DefaultTutorialFadeInDuration = 1.75f;

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
        SceneTransitionFader fader = SceneTransitionFader.Create(Color.black);

        float safeFadeOutDuration = ResolveDurationOrDefault(fadeToBlackDuration, DefaultFadeToBlackDuration);
        float safeFadeInDuration = ResolveDurationOrDefault(tutorialFadeInDuration, DefaultTutorialFadeInDuration);

        yield return fader.Fade(0f, 1f, safeFadeOutDuration);

        fader.ScheduleFadeInAfterSceneLoad(safeFadeInDuration);
        SceneManager.LoadScene("TutorialScene");
    }

    private float ResolveDurationOrDefault(float configuredDuration, float fallbackDuration)
    {
        if (configuredDuration > 0f)
            return configuredDuration;

        return fallbackDuration;
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
