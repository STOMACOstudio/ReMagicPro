using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    public void GoToGameScene()
    {
        SceneManager.LoadScene("GameScene"); // Replace with your actual scene name
    }
}