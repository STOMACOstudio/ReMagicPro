using System.Collections;
using System.Collections.Generic;
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

    void Awake()
        {
            Instance = this;
            panel.SetActive(false);
        }

    public void ShowZoneDetails(MapZone zone)
        {
            selectedZone = zone;

            enemyPortrait.sprite = null;

            if (zone.enemyPortrait != null)
                enemyPortrait.sprite = zone.enemyPortrait;

            descriptionText.text = zone.enemyDescription;

            panel.SetActive(true);
            engageButton.interactable = zone.isUnlocked && !zone.isCompleted;
        }

    public void OnEngageClicked()
        {
            if (selectedZone == null || !selectedZone.isUnlocked) return;

            selectedZone.CompleteZone();

            panel.SetActive(false);

            Debug.Log($"[TEST] Simulated win for zone: {selectedZone.name}");
        }

    /*public void OnEngageClicked()
        {
            if (selectedZone == null || !selectedZone.isUnlocked) return;

            BattleData.CurrentZone = selectedZone;
            SceneManager.LoadScene("GameScene"); // Replace with your actual game scene
        }*/
}