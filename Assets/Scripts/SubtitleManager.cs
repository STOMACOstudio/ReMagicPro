using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SubtitleManager : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;
    private CanvasGroup canvasGroup;

    [System.Serializable]
    public class SubtitleLine {
        public string text;
        public float displayDuration;
    }

    void Awake()
    {
        // Ensure we have a CanvasGroup for fading
        canvasGroup = subtitleText.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = subtitleText.gameObject.AddComponent<CanvasGroup>();
        
        subtitleText.text = "";
        canvasGroup.alpha = 0;
    }

    public void DisplaySequence(List<SubtitleLine> lines)
    {
        StopAllCoroutines();
        StartCoroutine(SequenceRoutine(lines));
    }

    IEnumerator SequenceRoutine(List<SubtitleLine> lines)
    {
        foreach (SubtitleLine line in lines)
        {
            subtitleText.text = line.text;
            
            // Fade In
            yield return StartCoroutine(Fade(0, 1, 0.5f));

            yield return new WaitForSeconds(line.displayDuration);

            // Fade Out
            yield return StartCoroutine(Fade(1, 0, 0.5f));
            
            subtitleText.text = ""; 
        }
    }

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
        subtitleText.text = "";
        if (canvasGroup != null) canvasGroup.alpha = 0;
    }
}