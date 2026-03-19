using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class SubtitleManager : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;
    private CanvasGroup canvasGroup;
    private bool isSequencePlaying;
    private bool isInitialized;

    [Header("Skip Controls")]
    [SerializeField] private KeyCode skipKey = KeyCode.Space;

    [System.Serializable]
    public class SubtitleLine {
        public string text;
        public float displayDuration;
    }

    void Awake()
    {
        if (subtitleText == null)
        {
            Debug.LogError($"[{nameof(SubtitleManager)}] Missing subtitleText reference on {gameObject.name}.", this);
            enabled = false;
            return;
        }

        // Ensure we have a CanvasGroup for fading
        canvasGroup = subtitleText.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = subtitleText.gameObject.AddComponent<CanvasGroup>();
        
        subtitleText.text = "";
        canvasGroup.alpha = 0;
        isInitialized = true;
    }

    public void DisplaySequence(List<SubtitleLine> lines)
    {
        if (!isInitialized)
            return;

        StopSequence();

        if (lines == null || lines.Count == 0)
            return;

        StartCoroutine(SequenceRoutine(lines));
    }

    public IEnumerator DisplaySequenceAndWait(List<SubtitleLine> lines)
    {
        DisplaySequence(lines);

        while (isSequencePlaying)
        {
            yield return null;
        }
    }

    IEnumerator SequenceRoutine(List<SubtitleLine> lines)
    {
        isSequencePlaying = true;

        foreach (SubtitleLine line in lines)
        {
            subtitleText.text = line.text;
            
            // Fade In
            yield return StartCoroutine(Fade(0, 1, 0.5f));

            bool skipRequested = false;
            float elapsed = 0f;
            while (elapsed < line.displayDuration)
            {
                if (SkipRequested())
                {
                    skipRequested = true;
                    break;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Fade Out
            if (skipRequested)
            {
                canvasGroup.alpha = 0f;
            }
            else
            {
                yield return StartCoroutine(Fade(1, 0, 0.5f));
            }

            subtitleText.text = ""; 
        }

        isSequencePlaying = false;
    }


    private bool SkipRequested()
    {
        bool skipRequested = false;

#if ENABLE_LEGACY_INPUT_MANAGER
        skipRequested |= Input.GetKeyDown(skipKey) || Input.GetMouseButtonDown(0);
#endif

#if ENABLE_INPUT_SYSTEM
        skipRequested |= IsInputSystemSkipPressed();
#endif

        return skipRequested;
    }

#if ENABLE_INPUT_SYSTEM
    private bool IsInputSystemSkipPressed()
    {
        bool keyboardSkip = false;
        if (Keyboard.current != null)
        {
            Key mappedKey = MapLegacyKeyCode(skipKey);
            keyboardSkip = mappedKey != Key.None
                ? Keyboard.current[mappedKey].wasPressedThisFrame
                : Keyboard.current.spaceKey.wasPressedThisFrame;
        }

        bool mouseSkip = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        return keyboardSkip || mouseSkip;
    }

    private static Key MapLegacyKeyCode(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.Space: return Key.Space;
            case KeyCode.Return: return Key.Enter;
            case KeyCode.KeypadEnter: return Key.NumpadEnter;
            case KeyCode.Escape: return Key.Escape;
            case KeyCode.Tab: return Key.Tab;
            default: return Key.None;
        }
    }
#endif

    IEnumerator Fade(float start, float end, float duration)
    {
        float timer = 0;
        while(timer < duration) {
            canvasGroup.alpha = Mathf.Lerp(start, end, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = end;
    }

    public void StopSequence()
    {
        StopAllCoroutines();
        isSequencePlaying = false;
        if (subtitleText != null)
            subtitleText.text = "";
        if (canvasGroup != null) canvasGroup.alpha = 0;
    }
}
