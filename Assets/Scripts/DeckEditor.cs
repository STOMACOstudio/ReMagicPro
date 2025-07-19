using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckEditor : MonoBehaviour
{
    public Transform cardContainer;
    public GameObject cardVisualPrefab;

    private List<CardData> currentDeck = new List<CardData>();
    private List<CardData> collection = new List<CardData>();

    void Start()
    {
        LoadCollection();
        LoadDeck();
        ShowCards();
    }

    void LoadCollection()
    {
        // Placeholder collection logic - load all cards in database
        collection = new List<CardData>(CardDatabase.GetAllCards());
    }

    void LoadDeck()
    {
        if (DeckHolder.SelectedDeck != null)
            currentDeck = new List<CardData>(DeckHolder.SelectedDeck);
    }

    public void CardClicked(CardVisual visual)
    {
        // determine if card is already in deck by comparing parent container
        if (visual.transform.parent == cardContainer)
        {
            RemoveCardAt(visual.transform.GetSiblingIndex());
        }
        else
        {
            AddCard(visual.LinkedData);
        }
        ShowCards();
    }

    void AddCard(CardData data)
    {
        currentDeck.Add(data);
    }

    void RemoveCardAt(int index)
    {
        if (index >= 0 && index < currentDeck.Count)
            currentDeck.RemoveAt(index);
    }

    public void SaveDeck()
    {
        EnsureMinimumSize();
        DeckHolder.SelectedDeck = new List<CardData>(currentDeck);
        // optionally persist using PlayerPrefs
        PlayerPrefs.SetString("SavedDeck", string.Join(",", currentDeck.ConvertAll(c => c.cardName)));
        PlayerPrefs.Save();
        SceneManager.LoadScene("MapScene");
    }

    void EnsureMinimumSize()
    {
        while (currentDeck.Count < 40)
        {
            currentDeck.Add(CardDatabase.GetCardData(BasicLandNameForColor()));
        }
    }

    string BasicLandNameForColor()
    {
        string colorPref = PlayerPrefs.GetString("PlayerColor", "White");
        string color = colorPref.Split(',')[0];
        return color switch
        {
            "White" => "Plains",
            "Blue" => "Island",
            "Black" => "Swamp",
            "Red" => "Mountain",
            "Green" => "Forest",
            _ => "Plains"
        };
    }

    void ShowCards()
    {
        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        foreach (var cardData in currentDeck)
        {
            Card card = CardFactory.Create(cardData.cardName);
            GameObject go = Instantiate(cardVisualPrefab, cardContainer);
            go.transform.localScale = Vector3.one * 1.5f;
            CardVisual visual = go.GetComponent<CardVisual>();
            CardData sourceData = CardDatabase.GetCardData(card.cardName);
            visual.Setup(card, null, sourceData);
        }
    }
}
