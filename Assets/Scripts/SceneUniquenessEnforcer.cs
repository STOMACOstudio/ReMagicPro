using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneUniquenessEnforcer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;

        EnforceForActiveScene();
    }

    private static void OnSceneLoaded(Scene _, LoadSceneMode __)
    {
        EnforceForActiveScene();
    }

    private static void OnActiveSceneChanged(Scene _, Scene activeScene)
    {
        EventSystemUtility.EnsureSingleEventSystem(activeScene);
        EventSystemUtility.EnsureSingleAudioListener(activeScene);
    }

    private static void EnforceForActiveScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        EventSystemUtility.EnsureSingleEventSystem(activeScene);
        EventSystemUtility.EnsureSingleAudioListener(activeScene);
    }
}
