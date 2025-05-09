using UnityEngine;
using UnityEngine.UI;

public class MapZone : MonoBehaviour
{
    public enum ZoneType { Shack, Beginner, Advanced, Boss }

    [Header("Zone Settings")]
    public ZoneType zoneType;

    [Header("References")]
    public Image image; // drag your own Image component here

    [Header("Sprites")]
    public Sprite shackSprite;
    public Sprite bossSprite;
    public Sprite[] beginnerSprites;
    public Sprite[] advancedSprites;

    // This is the important line — must be INSIDE the class
    public void AssignSprite()
    {
        if (image == null) return;

        switch (zoneType)
        {
            case ZoneType.Shack:
                image.sprite = shackSprite;
                break;
            case ZoneType.Boss:
                image.sprite = bossSprite;
                break;
            case ZoneType.Beginner:
                if (beginnerSprites.Length > 0)
                    image.sprite = beginnerSprites[Random.Range(0, beginnerSprites.Length)];
                break;
            case ZoneType.Advanced:
                if (advancedSprites.Length > 0)
                    image.sprite = advancedSprites[Random.Range(0, advancedSprites.Length)];
                break;
        }
    }
}