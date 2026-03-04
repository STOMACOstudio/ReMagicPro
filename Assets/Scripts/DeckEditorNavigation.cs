using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DeckEditorNavigation : MonoBehaviour
{
    private const string DeckEditorSceneName = "DeckEditorScene";
    private bool isReturningToPreviousScene;

    void Start()
    {
        // Deck display is handled by DeckEditorManager.
        // Calling DeckViewer.ShowDeck here would rebuild the deck
        // without click handlers, so we omit that call.
        SceneManager.SetActiveScene(gameObject.scene);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            ConfirmDeck();
    }

    public void GoToDeckEditor()
    {
        DeckHolder.DeckEditorReturnSceneName = SceneManager.GetActiveScene().name;
        DeckHolder.IsDeckEditorOpenedAdditively = false;
        DeckHolder.RestoreGameplayCursorOnDeckEditorClose = false;
        SceneManager.LoadScene(DeckEditorSceneName);
    }

    public void ConfirmDeck()
    {
        if (isReturningToPreviousScene)
            return;

        var manager = UnityEngine.Object.FindFirstObjectByType<DeckEditorManager>();
        if (manager != null && manager.IsDeckComplete)
        {
            manager.ConfirmDeck();
            ReturnToPreviousScene();
        }
    }

    private void ReturnToPreviousScene()
    {
        if (isReturningToPreviousScene)
            return;

        isReturningToPreviousScene = true;
        string returnSceneName = DeckHolder.GetDeckEditorReturnScene();

        if (DeckHolder.IsDeckEditorOpenedAdditively)
        {
            Scene returnScene = SceneManager.GetSceneByName(returnSceneName);
            if (returnScene.IsValid() && returnScene.isLoaded)
            {
                SceneManager.SetActiveScene(returnScene);
                SceneManager.UnloadSceneAsync(DeckEditorSceneName);

                DeckHolder.IsDeckEditorOpenedAdditively = false;
                if (DeckHolder.RestoreGameplayCursorOnDeckEditorClose)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }

                DeckHolder.RestoreGameplayCursorOnDeckEditorClose = false;
                return;
            }
        }

        SceneManager.LoadScene(returnSceneName);
    }
}
