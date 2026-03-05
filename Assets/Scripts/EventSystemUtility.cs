using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class EventSystemUtility
{
    public static void EnableOnlyForScene(Scene targetScene)
    {
        EventSystem[] eventSystems = Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        EventSystem fallback = null;

        foreach (EventSystem eventSystem in eventSystems)
        {
            if (eventSystem == null)
                continue;

            bool shouldEnable = eventSystem.gameObject.scene == targetScene;
            eventSystem.enabled = shouldEnable;

            if (fallback == null)
                fallback = eventSystem;
        }

        if (!HasEnabledEventSystem(eventSystems) && fallback != null)
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
}
