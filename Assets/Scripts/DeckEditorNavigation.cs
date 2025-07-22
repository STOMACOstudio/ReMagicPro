using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckEditorNavigation : MonoBehaviour
{
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "DeckEditorScene")
        {
            DeckViewer.ShowDeck();
        }
    }
    public void GoToDeckEditor()
    {
        SceneManager.LoadScene("DeckEditorScene");
    }

    public void ConfirmDeck()
    {
        FindObjectOfType<DeckEditorManager>()?.ConfirmDeck();
        SceneManager.LoadScene("MapScene");
    }
}
