using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        SceneManager.LoadScene("TutorialScene"); // Use your actual scene name here
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
