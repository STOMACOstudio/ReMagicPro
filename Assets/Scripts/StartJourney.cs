using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartJourney : MonoBehaviour
{
    public GameObject loadingOverlay; // assign in Inspector

    public void BeginJourney()
    {
        string chosenColors = PlayerPrefs.GetString("PlayerColors", "");
        if (!string.IsNullOrEmpty(chosenColors))
        {
            StartCoroutine(ShowLoadingAndLoad());
        }
        else
        {
            Debug.LogWarning("No color selected!");
        }
    }

    IEnumerator ShowLoadingAndLoad()
    {
        loadingOverlay.SetActive(true); // show dark screen + text + animation
        yield return new WaitForSeconds(0.5f); // optional, so player sees it
        SceneManager.LoadScene("DeckBuilderScene");
    }
}