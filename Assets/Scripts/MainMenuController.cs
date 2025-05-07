using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("ColorSelectScene"); // Use your actual scene name here
    }

    public void OpenOptions()
    {
        Debug.Log("Options clicked");
    }

    public void OpenCredits()
    {
        Debug.Log("Credits clicked");
    }
}