using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckEditorNavigation : MonoBehaviour
{
    public void GoToDeckEditor()
    {
        SceneManager.LoadScene("DeckEditorScene");
    }

    public void ConfirmDeck()
    {
        SceneManager.LoadScene("MapScene");
    }
}
