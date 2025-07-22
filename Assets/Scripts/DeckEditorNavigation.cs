using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckEditorNavigation : MonoBehaviour
{
    void Start()
    {
        // Deck display is handled by DeckEditorManager.
        // Calling DeckViewer.ShowDeck here would rebuild the deck
        // without click handlers, so we omit that call.
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
