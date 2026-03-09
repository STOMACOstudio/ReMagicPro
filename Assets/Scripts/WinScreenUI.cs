using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class WinScreenUI : MonoBehaviour
{
    public GameObject winPanel;
    public Button winImageButton;
    public Image winIconImage;
    public Sprite winSprite;
    public Sprite loseSprite;
    public Image wonCardImage;
    public TMP_Text coinsWonText;
    public Transform wonCardContainer;
    public GameObject cardVisualPrefab;

    private readonly List<GameObject> spawnedRewardVisuals = new List<GameObject>();
    private readonly List<CardData> availableRewardCards = new List<CardData>();
    private CanvasGroup canvasGroup;
    private GameManager gameManager;
    private Coroutine fadeCoroutine;
    private bool isWin;
    private bool hasRewardChoices;
    private CardData selectedRewardCard;

    private const int WinCoinAmount = 25;
    private const int LoseCoinPenalty = 25;

    void Start()
    {
        winPanel.SetActive(false);
        canvasGroup = winPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = winPanel.AddComponent<CanvasGroup>();

        gameManager = UnityEngine.Object.FindFirstObjectByType<GameManager>();

        if (wonCardImage != null)
            wonCardImage.sprite = null;

        ClearRewardVisuals();

        if (coinsWonText != null)
        {
            coinsWonText.text = string.Empty;
        }

        winImageButton.onClick.AddListener(OnWinLoseClick);
    }

    public void ShowWinScreen(CardData wonCard, int coinsAward = WinCoinAmount)
    {
        isWin = true;

        if (winIconImage != null && winSprite != null)
            winIconImage.sprite = winSprite;

        SetupRewardChoices(wonCard);

        if (coinsWonText != null)
        {
            coinsWonText.text = "+" + coinsAward;
            coinsWonText.color = Color.green;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.gameOver = true;

        SoundManager.Instance.PlaySound(SoundManager.Instance.victory);
        CoinsManager.AddCoins(coinsAward);

        StartFadeIn();
    }

    public void ShowLoseScreen()
    {
        isWin = false;

        if (winIconImage != null && loseSprite != null)
            winIconImage.sprite = loseSprite;

        if (wonCardImage != null)
            wonCardImage.sprite = null;

        ClearRewardVisuals();
        selectedRewardCard = null;
        availableRewardCards.Clear();
        hasRewardChoices = false;

        if (coinsWonText != null)
        {
            coinsWonText.text = "-" + LoseCoinPenalty;
            coinsWonText.color = Color.red;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.gameOver = true;

        SoundManager.Instance.PlaySound(SoundManager.Instance.defeat);
        CoinsManager.AddCoins(-LoseCoinPenalty);
        StartFadeIn();
    }

    private void StartFadeIn()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeIn());
    }

    private void SetupRewardChoices(CardData fallbackCard)
    {
        ClearRewardVisuals();
        selectedRewardCard = null;

        List<CardData> rewardCards = ResolveRewardCards();
        availableRewardCards.Clear();
        availableRewardCards.AddRange(rewardCards);
        hasRewardChoices = rewardCards.Count > 0;

        if (!hasRewardChoices)
        {
            winImageButton.interactable = true;
            SpawnCardDisplay(fallbackCard, false);
            return;
        }

        if (rewardCards.Count == 1)
        {
            selectedRewardCard = rewardCards[0];
            winImageButton.interactable = true;
            SpawnCardDisplay(rewardCards[0], false);
            return;
        }

        winImageButton.interactable = true;
        for (int i = 0; i < rewardCards.Count; i++)
        {
            CardData rewardCard = rewardCards[i];
            GameObject rewardVisual = SpawnCardDisplay(rewardCard, true);
            if (rewardVisual != null)
            {
                rewardVisual.transform.localScale = Vector3.one * 1.4f;
            }
        }
    }

    private List<CardData> ResolveRewardCards()
    {
        List<CardData> rewards = new List<CardData>();

        for (int i = 0; i < BattleData.CurrentRewardCardNames.Count; i++)
        {
            string rewardName = BattleData.CurrentRewardCardNames[i];
            CardData data = CardDatabase.GetCardData(rewardName);
            if (data == null)
            {
                Debug.LogWarning($"[{nameof(WinScreenUI)}] Reward card '{rewardName}' does not exist in CardDatabase.");
                continue;
            }

            rewards.Add(data);
        }

        return rewards;
    }

    private GameObject SpawnCardDisplay(CardData cardData, bool clickable)
    {
        if (wonCardContainer == null || cardVisualPrefab == null || cardData == null)
            return null;

        GameObject visualObject = Instantiate(cardVisualPrefab, wonCardContainer);
        Card cardObj = CardFactory.Create(cardData.cardName);
        CardVisual visual = visualObject.GetComponent<CardVisual>();
        if (visual != null)
        {
            visual.Setup(cardObj, null, cardData);
            visual.disableHoverEffects = true;
        }

        if (clickable)
        {
            Button button = visualObject.GetComponent<Button>();
            if (button == null)
                button = visualObject.AddComponent<Button>();

            Image image = visualObject.GetComponent<Image>();
            if (image == null)
            {
                image = visualObject.AddComponent<Image>();
                image.color = new Color(1f, 1f, 1f, 0.001f);
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnRewardCardSelected(cardData));
        }

        spawnedRewardVisuals.Add(visualObject);
        return visualObject;
    }

    private void OnRewardCardSelected(CardData cardData)
    {
        selectedRewardCard = cardData;
        winImageButton.interactable = true;
    }

    private void ClaimSelectedRewardIfNeeded()
    {
        if (!isWin || !hasRewardChoices)
            return;

        CardData rewardToClaim = selectedRewardCard;
        if (rewardToClaim == null && availableRewardCards.Count > 0)
            rewardToClaim = availableRewardCards[0];

        if (rewardToClaim == null)
            return;

        PlayerCollection.OwnedCards.Add(rewardToClaim);
        Debug.Log($"[{nameof(WinScreenUI)}] Added reward card '{rewardToClaim.cardName}' to player collection.");
    }

    private void OnWinLoseClick()
    {
        if (isWin)
        {
            ClaimSelectedRewardIfNeeded();
            BattleData.ClearRewardCards();
            gameManager.WinBattle();
            return;
        }

        BattleData.ClearRewardCards();
        gameManager.ReturnToPreviousScene(applyWinEffects: false);
    }

    private void ClearRewardVisuals()
    {
        for (int i = 0; i < spawnedRewardVisuals.Count; i++)
        {
            if (spawnedRewardVisuals[i] != null)
                Destroy(spawnedRewardVisuals[i]);
        }

        spawnedRewardVisuals.Clear();

        if (wonCardContainer == null)
            return;

        foreach (Transform child in wonCardContainer)
            Destroy(child.gameObject);
    }

    private IEnumerator FadeIn()
    {
        winPanel.SetActive(true);
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float duration = 1f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(timer / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        fadeCoroutine = null;
    }
}
