// Attach this script to the Point Light
using UnityEngine;

public class CandleFlicker : MonoBehaviour
{
    Light candleLight;
    float baseIntensity;

    void Start()
    {
        candleLight = GetComponent<Light>();
        baseIntensity = candleLight.intensity;
    }

    void Update()
    {
        candleLight.intensity = baseIntensity + Random.Range(-0.2f, 0.2f);
    }
}