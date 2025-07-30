using UnityEngine;
using UnityEngine.SceneManagement;
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
            prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/CardPrefab.prefab");
#else
            prefab = Resources.Load<GameObject>("Prefab/CardPrefab");
#endif

        foreach (var data in DeckHolder.SelectedDeck)
        {
            Card card = CardFactory.Create(data.cardName);
            GameObject go = Instantiate(prefab, container);
            go.transform.localScale = Vector3.one * 1.5f;
            CardVisual visual = go.GetComponent<CardVisual>();
            CardData sourceData = CardDatabase.GetCardData(card.cardName);
            visual.Setup(card, null, sourceData);
        }
    }
}
