using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MerchantManager : MonoBehaviour
{
    [System.Serializable]
    public class MerchantSlot
    {
        // Assign the Transform that contains all UI for this slot.
        // The actual buy Button should be a child of this transform so it can
        // be positioned independently from the card preview.
        public Transform slotRoot;
        public TMP_Text priceText;

        [HideInInspector] public CardData cardData;
        [HideInInspector] public int price;
        [HideInInspector] public Button button;
        [HideInInspector] public CanvasGroup group;
        [HideInInspector] public bool sold;
    }

    public MerchantSlot basicLandSlot;
    public MerchantSlot commonSlot;
    public MerchantSlot uncommonSlot;
    public MerchantSlot rareSlot;
    public MerchantSlot specialOfferSlot;

    public AudioClip purchaseSound;

    private GameObject cardPrefab;

    private void SaveSoldState()
    {
        PlayerPrefs.SetInt("Merchant_BasicLandSold", basicLandSlot.sold ? 1 : 0);
        PlayerPrefs.SetInt("Merchant_CommonSold", commonSlot.sold ? 1 : 0);
        PlayerPrefs.SetInt("Merchant_UncommonSold", uncommonSlot.sold ? 1 : 0);
        PlayerPrefs.SetInt("Merchant_RareSold", rareSlot.sold ? 1 : 0);
        PlayerPrefs.SetInt("Merchant_OfferSold", specialOfferSlot.sold ? 1 : 0);
    }

    private void MarkSlotAsSold(MerchantSlot slot)
    {
        if (slot == null)
            return;

        if (slot.button != null)
            slot.button.interactable = false;
        if (slot.group != null)
        {
            slot.group.alpha = 0.5f;
            slot.group.blocksRaycasts = false;
            slot.group.interactable = false;
        }
    }

    void Start()
    {
        int refresh = PlayerPrefs.GetInt("RefreshMerchant", 1);
        cardPrefab = CardHoverPreview.Instance != null
            ? CardHoverPreview.Instance.CardVisualPrefab
#if UNITY_EDITOR
            : UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefab/CardPrefab.prefab");
#else
            : Resources.Load<GameObject>("Prefab/CardPrefab");
#endif

        if (refresh == 1 || !HasSavedInventory())
        {
            SetupSlots();
            SaveInventory();
            PlayerPrefs.SetInt("RefreshMerchant", 0);
            PlayerPrefs.Save();
        }
        else
        {
            LoadInventory();
        }
    }

    private void SetupSlots()
    {
        basicLandSlot.cardData = GetRandomBasicLand();
        basicLandSlot.sold = false;
        SetupSlot(basicLandSlot, 2);

        commonSlot.cardData = GetRandomCardByRarity("Common");
        commonSlot.sold = false;
        SetupSlot(commonSlot, 6);

        uncommonSlot.cardData = GetRandomCardByRarity("Uncommon");
        uncommonSlot.sold = false;
        SetupSlot(uncommonSlot, 10);

        rareSlot.cardData = GetRandomCardByRarity("Rare");
        rareSlot.sold = false;
        SetupSlot(rareSlot, 20);

        // special offer from one of the above with 50% discount
        var options = new List<MerchantSlot> { basicLandSlot, commonSlot, uncommonSlot, rareSlot };
        int index = Random.Range(0, options.Count);
        var chosen = options[index];
        int discounted = Mathf.CeilToInt(chosen.price * 0.5f);
        specialOfferSlot.cardData = chosen.cardData;
        specialOfferSlot.sold = false;
        SetupSlot(specialOfferSlot, discounted);
    }

    private void SetupSlot(MerchantSlot slot, int price)
    {
        if (slot.slotRoot == null || cardPrefab == null || slot.cardData == null)
            return;

        slot.price = price;
        if (slot.priceText != null)
        {
            slot.priceText.text = price.ToString();
            // Ensure the price label does not block raycasts so the buy button
            // receives clicks across its entire area
            slot.priceText.raycastTarget = false;
        }

        // Create or reuse a child object to host the card visual
        Transform cardParent = slot.slotRoot.Find("CardContainer");
        if (cardParent == null)
        {
            GameObject cp = new GameObject("CardContainer");
            cp.transform.SetParent(slot.slotRoot, false);
            cardParent = cp.transform;
        }

        // Find the buy button within the slot root, ignoring buttons that belong to the card prefab
        slot.button = slot.slotRoot.GetComponentsInChildren<Button>(true)
            .FirstOrDefault(b => !b.transform.IsChildOf(cardParent));

        foreach (Transform child in cardParent)
            Destroy(child.gameObject);

        Card card = CardFactory.Create(slot.cardData.cardName);
        GameObject go = Instantiate(cardPrefab, cardParent);
        go.transform.localScale = Vector3.one * 3f;
        // Place the card slightly higher in the merchant room so it is
        // easier to see. 65 units on the Y axis gives a better visual
        // alignment for the prefab in each slot.
        go.transform.localPosition = new Vector3(0f, 0f, 0f);
        var visual = go.GetComponent<CardVisual>();
        visual.Setup(card, null, slot.cardData);
        visual.disableHoverEffects = true;

        // Disable interactions on the card so it doesn't block the buy button
        Button cardButton = go.GetComponent<Button>();
        if (cardButton != null)
            cardButton.enabled = false;
        CanvasGroup cg = cardParent.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = cardParent.gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;

        slot.group = slot.slotRoot.GetComponent<CanvasGroup>();
        if (slot.group == null)
            slot.group = slot.slotRoot.gameObject.AddComponent<CanvasGroup>();

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
        string[] basicLands = { "Plains", "Island", "Swamp", "Mountain", "Forest" };
        var pool = CardDatabase.GetAllCards()
            .Where(c => c.rarity == rarity && !c.isToken && !basicLands.Contains(c.cardName))
            .ToList();
        if (pool.Count == 0)
            return null;
        return pool[Random.Range(0, pool.Count)];
    }

    private bool HasSavedInventory()
    {
        return PlayerPrefs.HasKey("Merchant_BasicLand");
    }

    private void SaveInventory()
    {
        PlayerPrefs.SetString("Merchant_BasicLand", basicLandSlot.cardData.cardName);
        PlayerPrefs.SetString("Merchant_Common", commonSlot.cardData.cardName);
        PlayerPrefs.SetString("Merchant_Uncommon", uncommonSlot.cardData.cardName);
        PlayerPrefs.SetString("Merchant_Rare", rareSlot.cardData.cardName);
        PlayerPrefs.SetString("Merchant_Offer", specialOfferSlot.cardData.cardName);
        PlayerPrefs.SetInt("Merchant_BasicLandPrice", basicLandSlot.price);
        PlayerPrefs.SetInt("Merchant_CommonPrice", commonSlot.price);
        PlayerPrefs.SetInt("Merchant_UncommonPrice", uncommonSlot.price);
        PlayerPrefs.SetInt("Merchant_RarePrice", rareSlot.price);
        PlayerPrefs.SetInt("Merchant_OfferPrice", specialOfferSlot.price);
        SaveSoldState();
        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        basicLandSlot.cardData = CardDatabase.GetCardData(PlayerPrefs.GetString("Merchant_BasicLand"));
        commonSlot.cardData = CardDatabase.GetCardData(PlayerPrefs.GetString("Merchant_Common"));
        uncommonSlot.cardData = CardDatabase.GetCardData(PlayerPrefs.GetString("Merchant_Uncommon"));
        rareSlot.cardData = CardDatabase.GetCardData(PlayerPrefs.GetString("Merchant_Rare"));
        specialOfferSlot.cardData = CardDatabase.GetCardData(PlayerPrefs.GetString("Merchant_Offer"));

        basicLandSlot.sold = PlayerPrefs.GetInt("Merchant_BasicLandSold", 0) == 1;
        commonSlot.sold = PlayerPrefs.GetInt("Merchant_CommonSold", 0) == 1;
        uncommonSlot.sold = PlayerPrefs.GetInt("Merchant_UncommonSold", 0) == 1;
        rareSlot.sold = PlayerPrefs.GetInt("Merchant_RareSold", 0) == 1;
        specialOfferSlot.sold = PlayerPrefs.GetInt("Merchant_OfferSold", 0) == 1;

        SetupSlot(basicLandSlot, PlayerPrefs.GetInt("Merchant_BasicLandPrice", 2));
        SetupSlot(commonSlot, PlayerPrefs.GetInt("Merchant_CommonPrice", 6));
        SetupSlot(uncommonSlot, PlayerPrefs.GetInt("Merchant_UncommonPrice", 10));
        SetupSlot(rareSlot, PlayerPrefs.GetInt("Merchant_RarePrice", 20));
        SetupSlot(specialOfferSlot, PlayerPrefs.GetInt("Merchant_OfferPrice", 1));

        if (basicLandSlot.sold) MarkSlotAsSold(basicLandSlot);
        if (commonSlot.sold) MarkSlotAsSold(commonSlot);
        if (uncommonSlot.sold) MarkSlotAsSold(uncommonSlot);
        if (rareSlot.sold) MarkSlotAsSold(rareSlot);
        if (specialOfferSlot.sold) MarkSlotAsSold(specialOfferSlot);
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
        slot.sold = true;
        MarkSlotAsSold(slot);
        SaveSoldState();
        PlayerPrefs.Save();
    }
}
