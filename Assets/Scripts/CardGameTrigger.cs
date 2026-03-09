using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CardGameTrigger : MonoBehaviour
{
    private const string BattleSceneName = "GameScene";

    [Header("Trigger")]
    public string playerTag = "Player";

    [Header("Battle")]
    [Tooltip("Deck key from GameManager.LoadDeckByKey (for example: Deck_Shore, Deck_Camp, Deck_Boss).")]
    public string enemyDeckKey = "Deck_Starter";

    [Tooltip("Cards the player can choose from after winning this match. Only one card can be claimed.")]
    public List<string> cardRewardOptions = new List<string>();

    [Header("Subtitles")]
    public SubtitleManager subtitleManager;
    public List<SubtitleManager.SubtitleLine> preBattleLines;

    private bool hasTriggered;

    void Awake()
    {
        if (subtitleManager == null)
            subtitleManager = Object.FindFirstObjectByType<SubtitleManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag(playerTag))
            return;

        hasTriggered = true;
        StartCoroutine(StartBattleSequence(other));
    }

    private IEnumerator StartBattleSequence(Collider playerCollider)
    {
        LockPlayerMovement(playerCollider);

        if (subtitleManager != null && preBattleLines != null && preBattleLines.Count > 0)
            yield return subtitleManager.DisplaySequenceAndWait(preBattleLines);

        StartBattle();
    }

    private void LockPlayerMovement(Collider playerCollider)
    {
        if (playerCollider == null)
            return;

        PlayerMovement movement = playerCollider.GetComponent<PlayerMovement>();
        BattleData.PausePlayerMovement(movement);
    }

    private void StartBattle()
    {
        string trimmedDeckKey = string.IsNullOrWhiteSpace(enemyDeckKey) ? "Deck_Starter" : enemyDeckKey.Trim();

        BattleData.CurrentZoneId = null;
        BattleData.CurrentDeckKey = trimmedDeckKey;
        BattleData.SetRewardCards(cardRewardOptions);
        BattleData.ReturnSceneName = SceneManager.GetActiveScene().name;
        BattleData.IsBattleOpenedAdditively = true;
        BattleData.TriggeringPlatform = null;
        BattleData.PauseReturnScene();

        SceneManager.LoadScene(BattleSceneName, LoadSceneMode.Additive);
        Scene battleScene = SceneManager.GetSceneByName(BattleSceneName);
        if (battleScene.IsValid() && battleScene.isLoaded)
        {
            EventSystemUtility.EnableOnlyForScene(battleScene);
            EventSystemUtility.EnsureSingleAudioListener(battleScene);
            SceneManager.SetActiveScene(battleScene);
        }
    }
}
