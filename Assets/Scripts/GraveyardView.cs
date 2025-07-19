using System.Collections.Generic;
using UnityEngine;

public class GraveyardView : MonoBehaviour
{
    public static GraveyardView Instance;

    public GameObject graveyardPanel;
    public Transform content;
    public GameObject cardPrefab;

    private readonly List<GameObject> spawned = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    public bool IsOpen => graveyardPanel != null && graveyardPanel.activeSelf;

    public void Open(Player owner)
    {
        if (graveyardPanel == null || content == null || cardPrefab == null || owner == null)
            return;

        foreach (var go in spawned)
            Destroy(go);
        spawned.Clear();

        foreach (var card in owner.Graveyard)
        {
            GameObject obj = Instantiate(cardPrefab, content);
            var visual = obj.GetComponent<CardVisual>();
            CardData data = CardDatabase.GetCardData(card.cardName);
            visual.Setup(card, GameManager.Instance, data);
            visual.PrepareForGraveyard();
            spawned.Add(obj);
        }

        graveyardPanel.SetActive(true);
        GameManager.Instance.isGraveyardOpen = true;
    }

    public void Close()
    {
        graveyardPanel.SetActive(false);
        foreach (var go in spawned)
            Destroy(go);
        spawned.Clear();
        GameManager.Instance.isGraveyardOpen = false;
    }
}
