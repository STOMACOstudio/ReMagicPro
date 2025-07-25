using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToColorSelection : Interactable
{
    public override void Interact()
    {
        // Unlock and show the cursor before leaving the first person scene
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("ColorSelectScene");
    }
}
