using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BattleData
{
    private const string DefaultReturnSceneName = "MapScene";

    public static string CurrentZoneId = null;
    public static string LastCompletedZoneId = null;

    //public static MapZone CurrentZone = null;
    public static bool ZoneJustCompleted = false;

    public static string CurrentDeckKey = null;
    public static string ReturnSceneName = null;
    public static bool IsBattleOpenedAdditively = false;
    public static PlatformTrigger TriggeringPlatform = null;

    public static readonly List<GameObject> PausedSceneRoots = new List<GameObject>();

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
    }

    public static string GetReturnScene(string fallbackSceneName = DefaultReturnSceneName)
    {
        return string.IsNullOrEmpty(ReturnSceneName) ? fallbackSceneName : ReturnSceneName;
    }
}
