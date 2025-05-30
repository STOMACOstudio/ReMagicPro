using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenUI : MonoBehaviour
{
    public GameObject winPanel; // Assign in Inspector
    public Button winImageButton; // Assign in Inspector
    private CanvasGroup canvasGroup;

    private GameManager gameManager;

    void Start()
    {
        winPanel.SetActive(false);
        canvasGroup = winPanel.GetComponent<CanvasGroup>();

        gameManager = FindObjectOfType<GameManager>();

        winImageButton.onClick.AddListener(() =>
        {
            gameManager.WinBattle();
        });
    }

    public void ShowWinScreen()
    {
        StartCoroutine(DissolveIn());
    }

    private IEnumerator DissolveIn()
    {
        winPanel.SetActive(true);
        canvasGroup.alpha = 0f;

        float duration = 1f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(timer / duration);
            yield return null;
        }

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}