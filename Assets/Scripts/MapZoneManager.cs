using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapZoneManager : MonoBehaviour
{
    public List<MapZone> mapZones;

    void Start()
    {
        if (mapZones == null || mapZones.Count != 10)
        {
            Debug.LogError("You must assign exactly 10 map zones!");
            return;
        }

        // Assign types based on fixed layout
        mapZones[0].zoneType = MapZone.ZoneType.Shack;
        for (int i = 1; i <= 5; i++) mapZones[i].zoneType = MapZone.ZoneType.Beginner;
        for (int i = 6; i <= 8; i++) mapZones[i].zoneType = MapZone.ZoneType.Advanced;
        mapZones[9].zoneType = MapZone.ZoneType.Boss;

        // Let each MapZone assign its sprite now
        foreach (var zone in mapZones)
        {
            zone.AssignSprite();
        }

        mapZones[0].TryUnlock(); // Start with Shack unlocked
    }
}