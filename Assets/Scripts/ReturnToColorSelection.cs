using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToColorSelection : Interactable
{
    public override void Interact()
    {
        SceneManager.LoadScene("ColorSelectScene");
    }
}
