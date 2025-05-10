using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MapZone : MonoBehaviour
{
    public enum ZoneType { Shack, Beginner, Advanced, Boss }

    [Header("Zone Settings")]
    public ZoneType zoneType;

    [Header("References")]
    public Image image;
    public TMP_Text areaLabel;
    public Image colorIcon;

    [Header("Sprites")]
    public Sprite shackSprite;
    public Sprite bossSprite;
    public Sprite[] beginnerSprites;
    public Sprite[] advancedSprites;

    [Header("Color Icons")]
    public Sprite redIcon;
    public Sprite blueIcon;
    public Sprite blackIcon;
    public Sprite greenIcon;
    public Sprite whiteIcon;
    public Sprite artifactIcon;

    [Header("Unlock Logic")]
    public List<MapZone> prerequisites; // new: zones that must be completed to unlock this one
    public List<MapZone> nextZones;
    public bool isUnlocked = false;

    [Header("Battle")]
    //public string aiDeckName;
    public string battleSceneName = "GameScene"; // default scene name

    private bool isCompleted = false;
    private Button button;
    private Sprite assignedSprite = null;

    public GameObject linePrefab;      // Drag the line prefab here
    public Transform lineContainer;    // Drag an empty "Lines" object here for organization

    private static List<MapZone> allZones = new List<MapZone>();

    public void AssignSprite()
    {
        if (!allZones.Contains(this))
        allZones.Add(this);

        if (image == null) return;

        // Get button component once
        if (button == null)
            button = GetComponent<Button>();

        // Hook up click only once
        if (button != null && button.onClick.GetPersistentEventCount() == 0)
            button.onClick.AddListener(OnClick);

        // Pick the sprite only once
        if (assignedSprite == null)
        {
            switch (zoneType)
            {
                case ZoneType.Shack:
                    assignedSprite = shackSprite;
                    break;
                case ZoneType.Boss:
                    assignedSprite = bossSprite;
                    break;
                case ZoneType.Beginner:
                    if (beginnerSprites.Length > 0)
                        assignedSprite = beginnerSprites[Random.Range(0, beginnerSprites.Length)];
                    break;
                case ZoneType.Advanced:
                    if (advancedSprites.Length > 0)
                        assignedSprite = advancedSprites[Random.Range(0, advancedSprites.Length)];
                    break;
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
                // Show icon only if zone is unlocked
                colorIcon.gameObject.SetActive(isUnlocked);

                if (isUnlocked)
                {
                    string lowerName = assignedSprite.name.ToLower();

                    if (lowerName.Contains("red")) colorIcon.sprite = redIcon;
                    else if (lowerName.Contains("blue")) colorIcon.sprite = blueIcon;
                    else if (lowerName.Contains("black")) colorIcon.sprite = blackIcon;
                    else if (lowerName.Contains("green")) colorIcon.sprite = greenIcon;
                    else if (lowerName.Contains("white")) colorIcon.sprite = whiteIcon;
                    else if (lowerName.Contains("artifact")) colorIcon.sprite = artifactIcon;
                }
            }

        }

        // Visuals: locked or unlocked
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

        // Set static reference so GameManager can know what deck to use
        BattleData.SelectedMapZone = this;

        UnityEngine.SceneManagement.SceneManager.LoadScene(battleSceneName);
    }

    /*public void OnClick()
    {
        if (!isUnlocked || isCompleted) return;

        Debug.Log($"{gameObject.name} clicked → marked as completed");
        isCompleted = true;

        // Disable this button (completed)
        if (button != null)
            button.interactable = false;

        // Unlock next zones
        foreach (var zone in nextZones)
        {
            zone.TryUnlock();

            // UI Line: draw from this to next
            if (linePrefab != null && lineContainer != null)
            {
                GameObject lineObj = Instantiate(linePrefab, lineContainer);
                RectTransform line = lineObj.GetComponent<RectTransform>();

                Vector3 startWorld = transform.position;
                Vector3 endWorld = zone.transform.position;

                // Convert world to local position relative to LineContainer
                Vector2 start = lineContainer.InverseTransformPoint(startWorld);
                Vector2 end = lineContainer.InverseTransformPoint(endWorld);

                Vector2 direction = end - start;
                float distance = direction.magnitude;

                line.anchoredPosition = start;
                line.sizeDelta = new Vector2(distance, line.sizeDelta.y);
                line.pivot = new Vector2(0, 0.5f); // left center
                line.rotation = Quaternion.FromToRotation(Vector3.right, direction);
            }
        }

        // Disable all OTHER unlocked-but-uncompleted zones not in nextZones
        foreach (var zone in allZones)
        {
            if (zone != this && !zone.isCompleted && zone.isUnlocked && !nextZones.Contains(zone))
            {
                zone.isUnlocked = false;
                if (zone.button != null)
                    zone.button.interactable = false;
                zone.AssignSprite(); // refresh visuals to re-gray it out
            }
        }
    }*/

    public void TryUnlock()
    {
        if (isUnlocked) return;

        // Check if ANY prerequisite is completed
        bool anyComplete = false;
        foreach (var prereq in prerequisites)
        {
            if (prereq.isCompleted)
            {
                anyComplete = true;
                break;
            }
        }

        if (!anyComplete) return; // none completed → stay locked

        isUnlocked = true;
        AssignSprite();
    }
}