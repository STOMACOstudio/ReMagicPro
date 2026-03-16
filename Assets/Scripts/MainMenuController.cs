using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private AudioClip startGameSound;

    void Start()
    {
        ClearAllSaves();
    }

    public void PlayGame()
    {
        AudioClip clip = startGameSound;
        if (clip == null && SoundManager.Instance != null)
            clip = SoundManager.Instance.buttonClick;

        if (clip != null && SoundManager.Instance != null)
            SoundManager.Instance.PlaySound(clip);

        ScreenFadeTransition.Instance.FadeToScene("TutorialScene");
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
