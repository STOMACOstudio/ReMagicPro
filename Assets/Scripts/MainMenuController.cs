using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneTransitionFader fader = SceneTransitionFader.Create(Color.black);

        yield return fader.Fade(0f, 1f, fadeToBlackDuration);

        fader.ScheduleFadeInAfterSceneLoad(tutorialFadeInDuration);
        SceneManager.LoadScene("TutorialScene");
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
