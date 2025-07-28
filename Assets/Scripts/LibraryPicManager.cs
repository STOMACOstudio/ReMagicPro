using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LibraryPicManager : MonoBehaviour
{
    [Header("Target Image")]
    public Image targetImage;

    [Header("Single Colors")]
    public Sprite whiteArt;
    public Sprite blueArt;
    public Sprite blackArt;
    public Sprite redArt;
    public Sprite greenArt;

    [Header("Two-Color Combinations")]
    public Sprite whiteBlueArt;
    public Sprite whiteBlackArt;
    public Sprite whiteRedArt;
    public Sprite whiteGreenArt;
    public Sprite blueBlackArt;
    public Sprite blueRedArt;
    public Sprite blueGreenArt;
    public Sprite blackRedArt;
    public Sprite blackGreenArt;
    public Sprite redGreenArt;

    private string lastKey = "";

    void Start()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
        UpdateArt();
    }

    void Update()
    {
        UpdateArt();
    }

    private void UpdateArt()
    {
        string stored = PlayerPrefs.GetString("PlayerColors", "");
        if (stored == lastKey) return;
        lastKey = stored;
        Sprite art = GetSpriteForColors(stored);
        if (targetImage != null)
        {
            if (art != null)
            {
                targetImage.enabled = true;
                targetImage.sprite = art;
            }
            else
            {
                targetImage.sprite = null;
                targetImage.enabled = false;
            }
        }
    }

    private Sprite GetSpriteForColors(string colors)
    {
        if (string.IsNullOrEmpty(colors))
            return null;

        var parts = colors.Split(',');
        List<string> trimmed = new List<string>();
        foreach (var p in parts)
        {
            var t = p.Trim();
            if (!string.IsNullOrEmpty(t))
                trimmed.Add(t);
        }
        trimmed.Sort();
        string key = string.Join(",", trimmed);

        switch (key)
        {
            case "Black": return blackArt;
            case "Blue": return blueArt;
            case "Green": return greenArt;
            case "Red": return redArt;
            case "White": return whiteArt;
            case "Black,Blue": return blueBlackArt;
            case "Black,Green": return blackGreenArt;
            case "Black,Red": return blackRedArt;
            case "Black,White": return whiteBlackArt;
            case "Blue,Green": return blueGreenArt;
            case "Blue,Red": return blueRedArt;
            case "Blue,White": return whiteBlueArt;
            case "Green,Red": return redGreenArt;
            case "Green,White": return whiteGreenArt;
            case "Red,White": return whiteRedArt;
            default: return null;
        }
    }
}
