using UnityEngine;

public class AnimateTerrainWater : MonoBehaviour
{
    public float speedX = 0.01f;
    public float speedY = 0.005f;

    Terrain terrain;
    Vector2 offset;

    void Start()
    {
        terrain = GetComponent<Terrain>();
    }

    void Update()
    {
        offset.x += speedX * Time.deltaTime;
        offset.y += speedY * Time.deltaTime;

        TerrainLayer[] layers = terrain.terrainData.terrainLayers;

        if (layers.Length > 0)
        {
            layers[0].tileOffset = offset;
        }
    }
}