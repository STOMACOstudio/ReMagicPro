using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MulliganManager : MonoBehaviour
{
    public GameObject mulliganPanel;
    public Button mulliganButton;
    public Button keepButton;

    void Start()
    {
        if (TurnSystem.Instance != null)
            TurnSystem.Instance.autoStart = false;

        if (mulliganPanel != null)
            mulliganPanel.SetActive(true);

        if (mulliganButton != null)
        {
            mulliganButton.onClick.AddListener(OnMulligan);
            if (CoinsManager.Coins < 10)
                mulliganButton.interactable = false;
        }
        if (keepButton != null)
            keepButton.onClick.AddListener(OnKeep);
    }

    private void OnMulligan()
    {
        if (!CoinsManager.SpendCoins(10))
            return;

        var player = GameManager.Instance.humanPlayer;
        foreach (var card in player.Hand.ToList())
        {
            player.Deck.Add(card);
            var visual = GameManager.Instance.FindCardVisual(card);
            if (visual != null)
            {
                GameManager.Instance.activeCardVisuals.Remove(visual);
                Destroy(visual.gameObject);
            }
        }
        player.Hand.Clear();

        GameManager.Instance.ShuffleDeck(player);
        GameManager.Instance.DrawCards(player, 7);
        GameManager.Instance.UpdateUI();
    }

    private void OnKeep()
    {
        if (mulliganPanel != null)
            mulliganPanel.SetActive(false);

        TurnSystem.Instance.StartGame();
    }
}
