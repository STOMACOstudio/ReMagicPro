using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToGameScene : MonoBehaviour
{
    public void GoToGame()
    {
        SceneManager.LoadScene("GameScene"); // Replace with your actual scene name
    }
}