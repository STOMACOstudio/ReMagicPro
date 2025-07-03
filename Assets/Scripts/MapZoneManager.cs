using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapZoneManager : MonoBehaviour
{
    public static MapZoneManager Instance;
    public List<MapZone> mapZones;
    public List<int> spriteIndices = new List<int>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (mapZones == null || mapZones.Count != 20)
        {
            Debug.LogError("You must assign exactly 20 map zones!");
            return;
        }

        // STEP 1: Set zone types
        mapZones[0].zoneType = MapZone.ZoneType.Shack;
        mapZones[mapZones.Count - 1].zoneType = MapZone.ZoneType.Boss;
        for (int i = 1; i < mapZones.Count - 1; i++)
        {
            mapZones[i].zoneType =
                Random.value < 0.5f ? MapZone.ZoneType.Beginner : MapZone.ZoneType.Advanced;
        }

        // STEP 2: Generate or load sprite layout
        string spriteIndexCSV = PlayerPrefs.GetString("MapSpriteIndices", null);
        if (string.IsNullOrEmpty(spriteIndexCSV))
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var zone in mapZones)
            {
                int index = 0;
                if (zone.zoneType == MapZone.ZoneType.Beginner && zone.beginnerSprites.Length > 0)
                    index = Random.Range(0, zone.beginnerSprites.Length);
                else if (zone.zoneType == MapZone.ZoneType.Advanced && zone.advancedSprites.Length > 0)
                    index = Random.Range(0, zone.advancedSprites.Length);

                spriteIndices.Add(index);
                sb.Append(index).Append(",");
            }
            PlayerPrefs.SetString("MapSpriteIndices", sb.ToString().TrimEnd(','));
            PlayerPrefs.Save();
        }
        else
        {
            string[] tokens = spriteIndexCSV.Split(',');
            foreach (string token in tokens)
            {
                if (int.TryParse(token, out int idx))
                    spriteIndices.Add(idx);
                else
                    spriteIndices.Add(0);
            }
        }

        // STEP 3: Assign sprites based on saved layout
        foreach (var zone in mapZones)
        {
            zone.AssignSprite();
        }

        // STEP 4: Load saved completed zones
        string completedZonesCSV = PlayerPrefs.GetString("CompletedZones", "");
        HashSet<string> completedZoneIds = new HashSet<string>(completedZonesCSV.Split(','));

        string lastZoneName = PlayerPrefs.GetString("LastCompletedZone", null);

        // Complete previously completed zones, excluding the one just completed now
        foreach (var zone in mapZones)
        {
            if (completedZoneIds.Contains(zone.zoneId) && zone.zoneId != lastZoneName)
            {
                zone.CompleteZone();
            }
        }

        // Complete the most recent zone
        if (!string.IsNullOrEmpty(lastZoneName))
        {
            MapZone completedZone = mapZones.Find(z => z.zoneId == lastZoneName);
            if (completedZone != null)
            {
                completedZone.CompleteZone();

                completedZoneIds.Add(lastZoneName);
                string newCSV = string.Join(",", completedZoneIds);
                PlayerPrefs.SetString("CompletedZones", newCSV);
                PlayerPrefs.DeleteKey("LastCompletedZone");
                PlayerPrefs.Save();
            }
        }

        // STEP 5: Ensure Shack is always unlocked
        mapZones[0].TryUnlock();
    }

    public int GetSpriteIndexForZone(MapZone zone)
    {
        int index = mapZones.IndexOf(zone);
        if (index >= 0 && index < spriteIndices.Count)
            return spriteIndices[index];
        return 0;
    }
}