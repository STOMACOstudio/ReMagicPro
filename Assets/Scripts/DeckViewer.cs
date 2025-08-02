using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DeckViewer : MonoBehaviour
{
    private const string EditorSceneName = "DeckEditorScene";

    void Start()
    {
        if (SceneManager.GetActiveScene().name != EditorSceneName)
            return;

        ShowDeck();
    }

    public static void ShowDeck()
    {
        if (SceneManager.GetActiveScene().name != EditorSceneName)
            return;

        Transform container = GameObject.Find("CardContainer")?.transform;
        if (container == null)
            return;

        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        if (DeckHolder.SelectedDeck == null)
            return;

        GameObject prefab = CardHoverPreview.Instance != null ?
            CardHoverPreview.Instance.CardVisualPrefab : null;

        if (prefab == null)
#if UNITY_EDITOR
            prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefab/CardPrefab.prefab");
#else
            prefab = Resources.Load<GameObject>("Prefab/CardPrefab");
#endif

        var groupedBasics = new Dictionary<string, (CardData data, int count)>();

        foreach (var data in DeckHolder.SelectedDeck)
        {
            if (CardData.IsBasicLand(data))
            {
                if (groupedBasics.TryGetValue(data.cardName, out var entry))
                    groupedBasics[data.cardName] = (entry.data, entry.count + 1);
                else
                    groupedBasics[data.cardName] = (data, 1);
            }
            else
            {
                Spawn(prefab, container, data, 1);
            }
        }

        foreach (var kvp in groupedBasics.Values)
            Spawn(prefab, container, kvp.data, kvp.count);
    }

    private static void Spawn(GameObject prefab, Transform container, CardData data, int count)
    {
        Card card = CardFactory.Create(data.cardName);
        GameObject go = Object.Instantiate(prefab, container);
        go.transform.localScale = Vector3.one * 1.5f;
        CardVisual visual = go.GetComponent<CardVisual>();
        CardData sourceData = CardDatabase.GetCardData(card.cardName);
        visual.Setup(card, null, sourceData);

        var handler = go.AddComponent<DeckEditorCardButton>();
        handler.Initialize(data, null, count);
    }
}
