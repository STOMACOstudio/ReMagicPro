using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class EventSystemUtility
{
    public static void EnableOnlyForScene(Scene targetScene)
    {
        EnsureSingleEventSystem(targetScene);
    }

    public static void EnsureSingleEventSystem(Scene targetScene)
    {
        EventSystem[] eventSystems = Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        EventSystem preferred = null;
        EventSystem fallback = null;

        foreach (EventSystem eventSystem in eventSystems)
        {
            if (eventSystem == null)
                continue;

            if (preferred == null && eventSystem.gameObject.scene == targetScene)
                preferred = eventSystem;

            if (fallback == null)
                fallback = eventSystem;

            bool shouldEnable = preferred != null && eventSystem == preferred;
            eventSystem.enabled = shouldEnable;
        }

        if (!HasEnabledEventSystem(eventSystems) && fallback != null)
            fallback.enabled = true;
    }

    public static void EnsureSingleAudioListener(Scene targetScene)
    {
        AudioListener[] listeners = Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        AudioListener preferred = null;
        AudioListener fallback = null;

        foreach (AudioListener listener in listeners)
        {
            if (listener == null)
                continue;

            if (preferred == null && listener.gameObject.scene == targetScene)
                preferred = listener;

            if (fallback == null)
                fallback = listener;

            bool shouldEnable = preferred != null && listener == preferred;
            listener.enabled = shouldEnable;
        }

        if (!HasEnabledAudioListener(listeners) && fallback != null)
            fallback.enabled = true;
    }

    private static bool HasEnabledEventSystem(EventSystem[] eventSystems)
    {
        foreach (EventSystem eventSystem in eventSystems)
        {
            if (eventSystem != null && eventSystem.enabled)
                return true;
        }

        return false;
    }

    private static bool HasEnabledAudioListener(AudioListener[] listeners)
    {
        foreach (AudioListener listener in listeners)
        {
            if (listener != null && listener.enabled)
                return true;
        }

        return false;
    }
}
