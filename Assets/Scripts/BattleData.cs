using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BattleData
{
    private const string DefaultReturnSceneName = "FarmScene";

    public enum BattleTransitionState
    {
        Idle,
        EnteringBattle,
        InBattle,
        ReturningToWorld
    }

    public static string CurrentZoneId = null;
    public static string LastCompletedZoneId = null;

    //public static MapZone CurrentZone = null;
    public static bool ZoneJustCompleted = false;

    public static string CurrentDeckKey = null;
    public static readonly List<string> CurrentRewardCardNames = new List<string>();
    public static string ReturnSceneName = null;
    public static bool IsBattleOpenedAdditively = false;
    public static CardGameTrigger TriggeringCardGameTrigger = null;
    public static PlatformTrigger TriggeringPlatform = null;
    public static BattleTransitionState TransitionState { get; private set; } = BattleTransitionState.Idle;

    public static readonly List<GameObject> PausedSceneRoots = new List<GameObject>();
    private static readonly List<PlayerMovement> PausedPlayerMovements = new List<PlayerMovement>();

    public static void SetRewardCards(IEnumerable<string> cardNames)
    {
        CurrentRewardCardNames.Clear();

        if (cardNames == null)
            return;

        foreach (string cardName in cardNames)
        {
            if (string.IsNullOrWhiteSpace(cardName))
                continue;

            string trimmedName = cardName.Trim();
            if (!CurrentRewardCardNames.Contains(trimmedName))
                CurrentRewardCardNames.Add(trimmedName);
        }
    }

    public static void ClearRewardCards()
    {
        CurrentRewardCardNames.Clear();
    }

    public static void PauseReturnScene()
    {
        PausedSceneRoots.Clear();

        Scene returnScene = SceneManager.GetSceneByName(GetReturnScene());
        if (!returnScene.IsValid() || !returnScene.isLoaded)
            return;

        GameObject[] roots = returnScene.GetRootGameObjects();
        for (int i = 0; i < roots.Length; i++)
        {
            GameObject root = roots[i];
            if (root == null || !root.activeSelf)
                continue;

            PausedSceneRoots.Add(root);
            root.SetActive(false);
        }
    }

    public static void ResumeReturnScene()
    {
        for (int i = 0; i < PausedSceneRoots.Count; i++)
        {
            GameObject root = PausedSceneRoots[i];
            if (root != null)
                root.SetActive(true);
        }

        PausedSceneRoots.Clear();
        ResumePlayerMovement();
    }

    public static void PausePlayerMovement(PlayerMovement movement)
    {
        if (movement == null)
            return;

        if (!PausedPlayerMovements.Contains(movement))
            PausedPlayerMovements.Add(movement);

        movement.enabled = false;

        if (movement.footstepAudio != null && movement.footstepAudio.isPlaying)
            movement.footstepAudio.Stop();
    }

    public static void ResumePlayerMovement()
    {
        for (int i = 0; i < PausedPlayerMovements.Count; i++)
        {
            PlayerMovement movement = PausedPlayerMovements[i];
            if (movement != null)
                movement.enabled = true;
        }

        PausedPlayerMovements.Clear();
    }

    public static string GetReturnScene(string fallbackSceneName = DefaultReturnSceneName)
    {
        return string.IsNullOrEmpty(ReturnSceneName) ? fallbackSceneName : ReturnSceneName;
    }

    public static bool TryBeginBattleTransition(
        string deckKey,
        string zoneId,
        string returnSceneName,
        IEnumerable<string> rewardCards = null,
        CardGameTrigger triggeringCardGameTrigger = null,
        PlatformTrigger triggeringPlatform = null,
        bool pauseReturnScene = true)
    {
        if (TransitionState != BattleTransitionState.Idle)
        {
            Debug.LogWarning($"[BattleData] Cannot start a new battle transition while state is {TransitionState}.");
            return false;
        }

        CurrentZoneId = zoneId;
        CurrentDeckKey = string.IsNullOrWhiteSpace(deckKey) ? "Deck_Starter" : deckKey.Trim();
        TriggeringCardGameTrigger = triggeringCardGameTrigger;
        TriggeringPlatform = triggeringPlatform;
        ReturnSceneName = string.IsNullOrWhiteSpace(returnSceneName) ? DefaultReturnSceneName : returnSceneName;
        IsBattleOpenedAdditively = true;
        SetRewardCards(rewardCards);

        TransitionState = BattleTransitionState.EnteringBattle;

        if (pauseReturnScene)
            PauseReturnScene();

        return true;
    }

    public static void MarkBattleSceneLoaded()
    {
        if (TransitionState == BattleTransitionState.EnteringBattle ||
            TransitionState == BattleTransitionState.InBattle)
        {
            TransitionState = BattleTransitionState.InBattle;
            return;
        }

        Debug.LogWarning($"[BattleData] MarkBattleSceneLoaded called while state is {TransitionState}.");
    }

    public static bool TryBeginReturnToWorld()
    {
        if (!IsBattleOpenedAdditively)
            return false;

        if (TransitionState == BattleTransitionState.ReturningToWorld)
            return false;

        TransitionState = BattleTransitionState.ReturningToWorld;
        return true;
    }

    public static void CompleteReturnToWorld()
    {
        TriggeringCardGameTrigger = null;
        TriggeringPlatform = null;
        IsBattleOpenedAdditively = false;
        ReturnSceneName = null;
        CurrentDeckKey = null;
        CurrentZoneId = null;
        ClearRewardCards();
        TransitionState = BattleTransitionState.Idle;
    }
}
