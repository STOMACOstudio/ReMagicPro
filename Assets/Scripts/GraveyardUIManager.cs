using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraveyardUIManager : MonoBehaviour
{
    public static GraveyardUIManager Instance { get; private set; }

    public Transform cardContainer;
    public Button closeButton;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
    }

    public void Open(List<Card> cards)
    {
        gameObject.SetActive(true);
        GameManager.Instance.graveyardViewActive = true;

        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        GameObject prefab = CardHoverPreview.Instance != null
            ? CardHoverPreview.Instance.CardVisualPrefab
            : Resources.Load<GameObject>("Prefab/CardPrefab");

        foreach (var card in cards)
        {
            if (card.isToken) continue;
            GameObject go = Instantiate(prefab, cardContainer);
            CardVisual visual = go.GetComponent<CardVisual>();
            visual.Setup(card, GameManager.Instance);
            visual.transform.localPosition = Vector3.zero;
            visual.isInGraveyard = true;
            visual.UpdateGraveyardVisual();
        }
    }

    public void Close()
    {
        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);
        gameObject.SetActive(false);
        GameManager.Instance.graveyardViewActive = false;
    }
}
