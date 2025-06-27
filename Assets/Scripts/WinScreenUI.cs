using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinScreenUI : MonoBehaviour
{
    public GameObject winPanel; // Assign in Inspector
    public Button winImageButton; // Assign in Inspector
    public Image winIconImage; // Optional: assign to dynamically change sprite
    public Sprite winSprite;
    public Sprite loseSprite;

    private CanvasGroup canvasGroup;
    private GameManager gameManager;

    private bool isWin;

    void Start()
    {
        winPanel.SetActive(false);
        canvasGroup = winPanel.GetComponent<CanvasGroup>();
        gameManager = FindObjectOfType<GameManager>();

        winImageButton.onClick.AddListener(OnWinLoseClick);
    }

    public void ShowWinScreen()
    {
        isWin = true;
        if (winIconImage != null && winSprite != null)
            winIconImage.sprite = winSprite;

        SoundManager.Instance.PlaySound(SoundManager.Instance.victory);

        StartCoroutine(FadeIn());
    }

    public void ShowLoseScreen()
    {
        isWin = false;
        if (winIconImage != null && loseSprite != null)
            winIconImage.sprite = loseSprite;

        SoundManager.Instance.PlaySound(SoundManager.Instance.defeat);

        StartCoroutine(FadeIn());
    }

    private void OnWinLoseClick()
    {
        if (isWin)
        {
            gameManager.WinBattle(); // Go to map scene
        }
        else
        {
            SceneManager.LoadScene("MainMenu"); // Restart game
        }
    }

    private IEnumerator FadeIn()
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