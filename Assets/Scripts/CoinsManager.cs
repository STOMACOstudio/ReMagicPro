using UnityEngine;
using TMPro;

public class CoinsManager : MonoBehaviour
{
    public static int Coins = 0;
    public static event System.Action OnCoinsChanged;

    [SerializeField] private TMP_Text coinsText;

    private void OnEnable()
    {
        OnCoinsChanged += UpdateDisplay;
        UpdateDisplay();
    }

    private void OnDisable()
    {
        OnCoinsChanged -= UpdateDisplay;
    }

    public static void AddCoins(int amount)
    {
        Coins += amount;
        OnCoinsChanged?.Invoke();
    }

    public static bool SpendCoins(int amount)
    {
        if (Coins < amount)
            return false;
        Coins -= amount;
        OnCoinsChanged?.Invoke();
        return true;
    }

    private void UpdateDisplay()
    {
        if (coinsText != null)
            coinsText.text = Coins.ToString();
    }
}
