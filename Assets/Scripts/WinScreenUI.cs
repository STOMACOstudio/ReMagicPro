using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// For spawning card visuals
using System.Collections.Generic;

public class WinScreenUI : MonoBehaviour
{
    public GameObject winPanel; // Assign in Inspector
    public Button winImageButton; // Assign in Inspector
    public Image winIconImage; // Optional: assign to dynamically change sprite
    public Sprite winSprite;
    public Sprite loseSprite;
    public Image wonCardImage; // Legacy image slot (unused)
    public TMP_Text coinsWonText; // Displays coins earned
    public Transform wonCardContainer; // Where to spawn the won card prefab
    public GameObject cardVisualPrefab; // Card visual prefab

    private GameObject spawnedCardVisual;

    private CanvasGroup canvasGroup;
    private GameManager gameManager;

    private bool isWin;

    void Start()
    {
        winPanel.SetActive(false);
        canvasGroup = winPanel.GetComponent<CanvasGroup>();
        gameManager = FindObjectOfType<GameManager>();

        if (wonCardImage != null)
            wonCardImage.sprite = null;

        if (spawnedCardVisual != null)
        {
            Destroy(spawnedCardVisual);
            spawnedCardVisual = null;
        }

        if (spawnedCardVisual != null)
        {
            Destroy(spawnedCardVisual);
            spawnedCardVisual = null;
        }

        }

        if (wonCardContainer != null)
        {
            foreach (Transform child in wonCardContainer)
                Destroy(child.gameObject);
        }

        if (coinsWonText != null)
            coinsWonText.text = string.Empty;

        winImageButton.onClick.AddListener(OnWinLoseClick);
    }

    public void ShowWinScreen(CardData wonCard, int coinsAward = 25)
    {
        isWin = true;
        if (winIconImage != null && winSprite != null)
            winIconImage.sprite = winSprite;

        if (wonCardContainer != null && cardVisualPrefab != null && wonCard != null)
        {
            if (spawnedCardVisual != null)
                Destroy(spawnedCardVisual);

            spawnedCardVisual = Instantiate(cardVisualPrefab, wonCardContainer);
            Card cardObj = CardFactory.Create(wonCard.cardName);
            var visual = spawnedCardVisual.GetComponent<CardVisual>();
            visual.Setup(cardObj, null, wonCard);
            spawnedCardVisual.transform.localScale = Vector3.one * 2f;

            if (wonCardImage != null)
                wonCardImage.sprite = null;
        }

        if (coinsWonText != null)
            coinsWonText.text = "+" + coinsAward.ToString();

        if (GameManager.Instance != null)
            GameManager.Instance.gameOver = true;

        SoundManager.Instance.PlaySound(SoundManager.Instance.victory);

        CoinsManager.AddCoins(coinsAward);

        StartCoroutine(FadeIn());
    }

    public void ShowLoseScreen()
    {
        isWin = false;
        if (winIconImage != null && loseSprite != null)
            winIconImage.sprite = loseSprite;

        if (wonCardImage != null)
            wonCardImage.sprite = null;
        if (spawnedCardVisual != null)
        {
            Destroy(spawnedCardVisual);
            spawnedCardVisual = null;
        }

        if (coinsWonText != null)
            coinsWonText.text = string.Empty;

        if (GameManager.Instance != null)
            GameManager.Instance.gameOver = true;

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