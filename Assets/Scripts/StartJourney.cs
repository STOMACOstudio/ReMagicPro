using UnityEngine;
using UnityEngine.SceneManagement;

public class StartJourney : MonoBehaviour
{
    public void BeginJourney()
    {
        // Optional: check if color was selected
        string chosenColor = PlayerPrefs.GetString("PlayerColor", "");
        if (!string.IsNullOrEmpty(chosenColor))
        {
            SceneManager.LoadScene("DeckBuilderScene");
        }
        else
        {
            Debug.LogWarning("No color selected!");
        }
    }
}