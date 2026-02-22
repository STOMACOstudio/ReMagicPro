using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ExitOnEscape : MonoBehaviour
{
    private void Update()
    {
        if (!IsEscapePressedThisFrame())
        {
            return;
        }

        Application.Quit();
        Debug.Log("Escape pressed — quitting.");
    }

    private static bool IsEscapePressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }
}
