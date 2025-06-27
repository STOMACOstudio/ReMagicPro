using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class MapZone : MonoBehaviour
{
    public enum ZoneType { Shack, Beginner, Advanced, Boss }

    [Header("Zone Settings")]
    public ZoneType zoneType;

    [Header("Zone Info")]
    public string zoneId;

    [Header("Battle")]
    public string battleSceneName = "GameScene";

    [Header("References")]
    public Image image;
    public TMP_Text areaLabel;
    public Image colorIcon;

    [Header("Sprites")]
    public Sprite shackSprite;
    public Sprite bossSprite;
    public Sprite[] beginnerSprites;
    public Sprite[] advancedSprites;

    [Header("Enemy Settings")]
    public Sprite enemyPortrait;

    [Header("Color Icons")]
    public Sprite redIcon;
    public Sprite blueIcon;
    public Sprite blackIcon;
    public Sprite greenIcon;
    public Sprite whiteIcon;
    public Sprite artifactIcon;

    private static readonly Dictionary<string, string> DeckKeyMap = new Dictionary<string, string>
    {
        {"shore", "Deck_Shore"},
        {"camp", "Deck_Camp"},
        {"graveyard", "Deck_Graveyard"},
        {"village", "Deck_Village"},
        {"thicket", "Deck_Thicket"},
        {"ruins", "Deck_Ruins"},
        {"church", "Deck_Church"},
        {"tower", "Deck_Tower"},
        {"hut", "Deck_Hut"},
        {"nest", "Deck_Nest"},
        {"woods", "Deck_Woods"},
        {"shack", "Deck_Starter"},
        {"castle", "Deck_Boss"}
    };

    private readonly Dictionary<string, Sprite> colorIconMap = new Dictionary<string, Sprite>();

    [Header("Unlock Logic")]
    public List<MapZone> prerequisites;
    public List<MapZone> nextZones;
    public bool isUnlocked = false;

    [Header("Deck Settings")]
    public string deckKey;

    [TextArea(2, 3)]
    public string enemyDescription;

    public bool isCompleted = false;
    private Button button;
    private Sprite assignedSprite = null;

    public GameObject linePrefab;
    public Transform lineContainer;

    private string baseSpriteNameForPortrait = "";

    private static List<MapZone> allZones = new List<MapZone>();

    public void AssignSprite()
    {
        if (!allZones.Contains(this))
            allZones.Add(this);

        if (image == null) return;

        if (button == null)
            button = GetComponent<Button>();

        if (button != null && button.onClick.GetPersistentEventCount() == 0)
            button.onClick.AddListener(OnClick);

        if (assignedSprite == null)
        {
            Sprite chosen = null;

            switch (zoneType)
            {
                case ZoneType.Shack:
                    chosen = shackSprite;
                    break;
                case ZoneType.Boss:
                    chosen = bossSprite;
                    break;
                case ZoneType.Beginner:
                    if (beginnerSprites.Length > 0)
                    {
                        int index = MapZoneManager.Instance.GetSpriteIndexForZone(this);
                        chosen = beginnerSprites[index % beginnerSprites.Length];
                    }
                    break;
                case ZoneType.Advanced:
                    if (advancedSprites.Length > 0)
                    {
                        int index = MapZoneManager.Instance.GetSpriteIndexForZone(this);
                        chosen = advancedSprites[index % advancedSprites.Length];
                    }
                    break;
            }

            if (chosen != null)
            {
                assignedSprite = chosen;
                baseSpriteNameForPortrait = chosen.name;

                enemyPortrait = GetPortraitForFoe(baseSpriteNameForPortrait);
                enemyDescription = GetDescriptionForFoe(baseSpriteNameForPortrait);

                // Assign deckKey based on sprite name using dictionary lookup
                string lowerName = chosen.name.ToLower();
                deckKey = DeckKeyMap.FirstOrDefault(kvp => lowerName.Contains(kvp.Key)).Value;
                if (string.IsNullOrEmpty(deckKey))
                    deckKey = "Deck_Starter";
            }
        }

        if (assignedSprite != null)
        {
            image.sprite = assignedSprite;

            if (areaLabel != null)
            {
                string[] parts = assignedSprite.name.Split('_');
                areaLabel.text = parts[0].Replace("_", " ");
            }

            if (colorIcon != null)
            {
                colorIcon.gameObject.SetActive(isUnlocked);

                if (isUnlocked)
                {
                    if (colorIconMap.Count == 0)
                    {
                        colorIconMap["red"] = redIcon;
                        colorIconMap["blue"] = blueIcon;
                        colorIconMap["black"] = blackIcon;
                        colorIconMap["green"] = greenIcon;
                        colorIconMap["white"] = whiteIcon;
                        colorIconMap["artifact"] = artifactIcon;
                    }

                    string lowerName = assignedSprite.name.ToLower();
                    Sprite found = colorIconMap.FirstOrDefault(kvp => lowerName.Contains(kvp.Key)).Value;
                    if (found != null)
                        colorIcon.sprite = found;
                }
            }
        }

        if (button != null)
        {
            button.interactable = isUnlocked;
            Color visual = isUnlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
            image.color = visual;
            if (colorIcon != null) colorIcon.color = visual;
        }
    }

    public void OnClick()
    {
        if (!isUnlocked || isCompleted) return;

        MapZoneUIManager.Instance.ShowZoneDetails(this);
    }

    public void CompleteZone()
    {
        if (isCompleted) return;

        isCompleted = true;
        Debug.Log($"[MapZone] Zone marked as completed: {name}");

        if (button != null)
            button.interactable = false;

        foreach (var zone in nextZones)
        {
            zone.TryUnlock();

            if (linePrefab != null && lineContainer != null)
            {
                GameObject lineObj = Instantiate(linePrefab, lineContainer);
                RectTransform line = lineObj.GetComponent<RectTransform>();

                Vector3 startWorld = transform.position;
                Vector3 endWorld = zone.transform.position;

                Vector2 start = lineContainer.InverseTransformPoint(startWorld);
                Vector2 end = lineContainer.InverseTransformPoint(endWorld);

                Vector2 direction = end - start;
                float distance = direction.magnitude;

                line.anchoredPosition = start;
                line.sizeDelta = new Vector2(distance, line.sizeDelta.y);
                line.pivot = new Vector2(0, 0.5f);
                line.rotation = Quaternion.FromToRotation(Vector3.right, direction);
            }
        }

        foreach (var zone in allZones)
        {
            if (zone != this && !zone.isCompleted && zone.isUnlocked && !nextZones.Contains(zone))
            {
                zone.isUnlocked = false;
                if (zone.button != null)
                    zone.button.interactable = false;
                zone.AssignSprite();
            }
        }
    }

    public void TryUnlock()
    {
        if (isUnlocked) return;

        bool anyComplete = false;
        foreach (var prereq in prerequisites)
        {
            if (prereq.isCompleted)
            {
                anyComplete = true;
                break;
            }
        }

        if (!anyComplete) return;

        isUnlocked = true;
        AssignSprite();
    }

    private Sprite GetPortraitForFoe(string spriteName)
    {
        spriteName = spriteName.ToLower();

        string[] parts = spriteName.Split('_');
        if (parts.Length < 1)
            return null;

        string location = parts[0];

        return Resources.Load<Sprite>($"Portraits/{location}_Portrait");
    }

    private string GetDescriptionForFoe(string spriteName)
    {
        spriteName = spriteName.ToLower();

        if (spriteName == "shack")
            return "Your mother awaits... with judgment.";

        string[] parts = spriteName.Split('_');
        if (parts.Length < 1) return "Something strange is here.";

        string location = parts[0];

        return location switch
        {
            "shack" => "All young wizards must leave home to prove their valor. Before departing on your pilgrimage, your mother wants to test your strength.",
            "village" => "You approach a small village. Unfortunately, intruders like you aren't welcome here—they seem afraid of you.",
            "shore" => "You find a secret shore. It seems calm, and you take a nap. When you wake up, the sea creatures have risen... drawn by something they recognize.",
            "graveyard" => "The calm of the graveyard seems like a good path to take, but slowly the dead rise from their tombs and encircle you, stirred by a presence they somehow know.",
            "camp" => "The remains of a campfire: someone left not long ago. The goblins are just around the corner, and their beasts seem uneasy, as if reacting to something...",
            "thicket" => "Intrigued by the woods, you step inside a thicket. Some curious monkeys approach you, sniff you—and suddenly become violent!",
            "ruins" => "Silent ruins that seem very old. The ancient machines are rusty and stiff, but as you pass by, they slowly reboot.",
            "church" => "A solitary church in the middle of a field. As you approach, the protectors leap without warning—no words, only fear and anger in their eyes.",
            "tower" => "A fellow wizard invites you into his tower for tea... but grows pale as you draw near. Then he attacks, as though compelled to strike first.",
            "hut" => "Unfortunately, you stumble into the hideout of some hideous witches, starving for raw human flesh.",
            "nest" => "You spot a huge egg. Starving, you hope to cook it—but the mother is coming, and you're now on the menu.",
            "woods" => "You wander the forest for hours before finding a clearing, made by wild beasts fighting. The scent you carry seems to disturb the natural order.",
            "castle" => "Your beloved mother reveals herself to be a Lich Queen. She devours her children—if they survive the pilgrimage—to renew her powers.",
            _ => "An unknown threat lies ahead."
        };
    }

    public void MarkAsCompleted()
    {
        if (isCompleted) return;

        CompleteZone();
    }
}