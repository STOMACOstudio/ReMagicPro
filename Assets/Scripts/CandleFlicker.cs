// Attach this script to the Point Light
using UnityEngine;

public class CandleFlicker : MonoBehaviour
{
    Light candleLight;
    float baseIntensity;
    float noiseOffset;
    public float noiseSpeed = 1f;
    public float intensityRange = 0.2f;

    void Start()
    {
        candleLight = GetComponent<Light>();
        baseIntensity = candleLight.intensity;
        noiseOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * noiseSpeed + noiseOffset, 0f);
        float flicker = (noise - 0.5f) * 2f * intensityRange;
        candleLight.intensity = Mathf.Max(0f, baseIntensity + flicker);
    }
}