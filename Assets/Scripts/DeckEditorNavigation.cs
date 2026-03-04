using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DeckEditorNavigation : MonoBehaviour
{
    void Start()
    {
        // Deck display is handled by DeckEditorManager.
        // Calling DeckViewer.ShowDeck here would rebuild the deck
        // without click handlers, so we omit that call.
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            ReturnToPreviousScene();
    }

    public void GoToDeckEditor()
    {
        DeckHolder.DeckEditorReturnSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("DeckEditorScene");
    }

    public void ConfirmDeck()
    {
        var manager = UnityEngine.Object.FindFirstObjectByType<DeckEditorManager>();
        if (manager != null && manager.IsDeckComplete)
        {
            manager.ConfirmDeck();
            ReturnToPreviousScene();
        }
    }

    private void ReturnToPreviousScene()
    {
        SceneManager.LoadScene(DeckHolder.GetDeckEditorReturnScene());
    }
}
