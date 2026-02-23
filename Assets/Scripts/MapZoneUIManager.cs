using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MapZoneUIManager : MonoBehaviour
{
    public static MapZoneUIManager Instance;

    [Header("UI Elements")]
    public GameObject panel;
    public Image enemyPortrait;
    public TMP_Text descriptionText;
    public Button engageButton;

    private MapZone selectedZone;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate MapZoneUIManager found. Destroying the new instance.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (panel != null)
        {
            panel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("MapZoneUIManager panel reference is missing.");
        }
    }

    public void ShowZoneDetails(MapZone zone)
    {
        if (zone == null)
        {
            Debug.LogWarning("ShowZoneDetails was called with a null zone.");
            return;
        }

        selectedZone = zone;

        if (enemyPortrait != null)
        {
            enemyPortrait.sprite = zone.enemyPortrait;
        }

        if (descriptionText != null)
        {
            descriptionText.text = zone.enemyDescription;
        }

        if (panel != null)
        {
            panel.SetActive(true);
        }

        if (engageButton != null)
        {
            engageButton.interactable = zone.isUnlocked && !zone.isCompleted;
        }
    }

    /*public void OnEngageClicked()
        {
            if (selectedZone == null || !selectedZone.isUnlocked) return;

            selectedZone.CompleteZone();

            panel.SetActive(false);

            Debug.Log($"[TEST] Simulated win for zone: {selectedZone.name}");
        }*/

    public void OnEngageClicked()
    {
        if (selectedZone == null || !selectedZone.isUnlocked)
        {
            return;
        }

        Debug.Log("Engaging zone: " + selectedZone.zoneId);
        //BattleData.CurrentZone = selectedZone;
        BattleData.CurrentZoneId = selectedZone.zoneId;
        BattleData.CurrentDeckKey = selectedZone.deckKey;

        SceneManager.LoadScene("GameScene");
    }
}
