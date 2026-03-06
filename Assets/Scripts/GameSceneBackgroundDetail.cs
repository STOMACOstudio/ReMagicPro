using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GameSceneBackgroundDetail : MonoBehaviour
{
    private const int NoiseTextureSize = 128;
    private const int VignetteTextureSize = 512;

    [SerializeField] private Color baseTint = new Color(0.94f, 0.91f, 0.84f, 1f);
    [SerializeField] private Color pulseTint = new Color(0.9f, 0.88f, 0.83f, 1f);
    [SerializeField] private float pulseSpeed = 0.25f;
    [SerializeField] private float pulseStrength = 0.35f;

    [SerializeField] private Color noiseColor = new Color(0.15f, 0.09f, 0.04f, 0.1f);
    [SerializeField] private float noiseScrollX = 0.003f;
    [SerializeField] private float noiseScrollY = 0.002f;

    [SerializeField] private Color vignetteColor = new Color(0.09f, 0.05f, 0.02f, 0.28f);

    private Image baseImage;
    private RawImage noiseLayer;
    private RawImage vignetteLayer;

    private static Texture2D sharedNoiseTexture;
    private static Texture2D sharedVignetteTexture;

    private void Awake()
    {
        baseImage = GetComponent<Image>();
        BuildLayers();
    }

    private void Update()
    {
        float pulse = (Mathf.Sin(Time.unscaledTime * pulseSpeed * Mathf.PI * 2f) * 0.5f + 0.5f) * pulseStrength;
        baseImage.color = Color.Lerp(baseTint, pulseTint, pulse);

        if (noiseLayer != null)
        {
            Rect uv = noiseLayer.uvRect;
            uv.x += noiseScrollX * Time.unscaledDeltaTime * 60f;
            uv.y += noiseScrollY * Time.unscaledDeltaTime * 60f;
            noiseLayer.uvRect = uv;
        }
    }

    private void BuildLayers()
    {
        EnsureSharedTextures();

        noiseLayer = BuildRawLayer("AmbientNoise", sharedNoiseTexture, noiseColor);
        noiseLayer.uvRect = new Rect(0f, 0f, 5.5f, 5.5f);

        vignetteLayer = BuildRawLayer("Vignette", sharedVignetteTexture, vignetteColor);
        vignetteLayer.uvRect = new Rect(0f, 0f, 1f, 1f);
    }

    private static void EnsureSharedTextures()
    {
        if (sharedNoiseTexture == null)
        {
            sharedNoiseTexture = MakeNoiseTexture(NoiseTextureSize, NoiseTextureSize);
            sharedNoiseTexture.name = "GameSceneBackgroundDetail_Noise";
        }

        if (sharedVignetteTexture == null)
        {
            sharedVignetteTexture = MakeVignetteTexture(VignetteTextureSize, VignetteTextureSize);
            sharedVignetteTexture.name = "GameSceneBackgroundDetail_Vignette";
        }
    }

    private RawImage BuildRawLayer(string layerName, Texture2D texture, Color tint)
    {
        Transform existing = transform.Find(layerName);
        GameObject layerObject = existing != null ? existing.gameObject : new GameObject(layerName, typeof(RectTransform), typeof(RawImage));
        layerObject.transform.SetParent(transform, false);

        RectTransform rect = layerObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        RawImage rawImage = layerObject.GetComponent<RawImage>();
        rawImage.texture = texture;
        rawImage.color = tint;
        rawImage.raycastTarget = false;
        layerObject.transform.SetAsLastSibling();
        return rawImage;
    }

    private static Texture2D MakeNoiseTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Repeat,
            filterMode = FilterMode.Bilinear
        };

        Color[] pixels = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float n = Mathf.PerlinNoise(x * 0.09f, y * 0.09f);
                pixels[y * width + x] = new Color(n, n, n, n);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply(false, true);
        return texture;
    }

    private static Texture2D MakeVignetteTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        Color[] pixels = new Color[width * height];
        Vector2 center = new Vector2(0.5f, 0.5f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 uv = new Vector2((float)x / (width - 1), (float)y / (height - 1));
                float dist = Vector2.Distance(uv, center);
                float alpha = Mathf.Clamp01(Mathf.InverseLerp(0.28f, 0.72f, dist));
                pixels[y * width + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply(false, true);
        return texture;
    }
}
