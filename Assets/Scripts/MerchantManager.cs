using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MerchantManager : MonoBehaviour
{
    [System.Serializable]
    public class MerchantSlot
    {
        public Transform slotRoot;
        public TMP_Text priceText;
        [HideInInspector] public CardData cardData;
        [HideInInspector] public int price;
        [HideInInspector] public Button button;
    }

    public MerchantSlot basicLandSlot;
    public MerchantSlot commonSlot;
    public MerchantSlot uncommonSlot;
    public MerchantSlot rareSlot;
    public MerchantSlot specialOfferSlot;

    public AudioClip purchaseSound;

    private GameObject cardPrefab;

    void Start()
    {
        cardPrefab = CardHoverPreview.Instance != null
            ? CardHoverPreview.Instance.CardVisualPrefab
            : Resources.Load<GameObject>("Prefab/CardPrefab");

        SetupSlots();
    }

    private void SetupSlots()
    {
        basicLandSlot.cardData = GetRandomBasicLand();
        SetupSlot(basicLandSlot, 2);

        commonSlot.cardData = GetRandomCardByRarity("Common");
        SetupSlot(commonSlot, 6);

        uncommonSlot.cardData = GetRandomCardByRarity("Uncommon");
        SetupSlot(uncommonSlot, 10);

        rareSlot.cardData = GetRandomCardByRarity("Rare");
        SetupSlot(rareSlot, 20);

        // special offer from one of the above with 20% discount
        var options = new List<MerchantSlot> { basicLandSlot, commonSlot, uncommonSlot, rareSlot };
        int index = Random.Range(0, options.Count);
        var chosen = options[index];
        int discounted = Mathf.CeilToInt(chosen.price * 0.8f);
        specialOfferSlot.cardData = chosen.cardData;
        SetupSlot(specialOfferSlot, discounted);
    }

    private void SetupSlot(MerchantSlot slot, int price)
    {
        if (slot.slotRoot == null || cardPrefab == null || slot.cardData == null)
            return;

        slot.price = price;
        if (slot.priceText != null)
            slot.priceText.text = price.ToString();

        Transform buttonTransform = slot.slotRoot.childCount > 0 ? slot.slotRoot.GetChild(0) : slot.slotRoot;
        foreach (Transform child in buttonTransform)
            Destroy(child.gameObject);

        Card card = CardFactory.Create(slot.cardData.cardName);
        GameObject go = Instantiate(cardPrefab, buttonTransform);
        go.transform.localScale = Vector3.one;
        var visual = go.GetComponent<CardVisual>();
        visual.Setup(card, null, slot.cardData);
        visual.disableHoverEffects = true;

        slot.button = buttonTransform.GetComponent<Button>();
        if (slot.button != null)
        {
            slot.button.onClick.RemoveAllListeners();
            slot.button.onClick.AddListener(() => Purchase(slot));
        }
    }

    private CardData GetRandomBasicLand()
    {
        string[] lands = { "Plains", "Island", "Swamp", "Mountain", "Forest" };
        string name = lands[Random.Range(0, lands.Length)];
        return CardDatabase.GetCardData(name);
    }

    private CardData GetRandomCardByRarity(string rarity)
    {
        var pool = CardDatabase.GetAllCards()
            .Where(c => c.rarity == rarity && !c.isToken)
            .ToList();
        if (pool.Count == 0)
            return null;
        return pool[Random.Range(0, pool.Count)];
    }

    public void Purchase(MerchantSlot slot)
    {
        if (slot.cardData == null)
            return;

        if (!CoinsManager.SpendCoins(slot.price))
            return;

        PlayerCollection.OwnedCards.Add(slot.cardData);
        if (purchaseSound != null && SoundManager.Instance != null)
            SoundManager.Instance.PlaySound(purchaseSound);
    }
}
