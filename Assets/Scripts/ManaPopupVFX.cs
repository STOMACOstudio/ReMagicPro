using UnityEngine;

public class ManaPopupVFX : MonoBehaviour
{
    public float duration = 1.0f;
    public float startScaleFactor = 0.5f;
    public float rotationSpeed = 45f;
    public bool moveUpward = true;             // 🔧 NEW: toggle upward motion
    public GameObject miniVFXPrefab;
    public int miniCount = 4;

    private float timer = 0f;
    private Vector3 startScale;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning("ManaPopupVFX: No SpriteRenderer found!");
            return;
        }

        startScale = Vector3.one * startScaleFactor;
        transform.localScale = startScale;

        if (miniVFXPrefab != null && miniCount > 0)
        {
            SpawnMiniBursts();
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duration;

        if (moveUpward)
            transform.position += Vector3.up * Time.deltaTime;

        transform.localScale = Vector3.Lerp(startScale, startScale * 1.5f, t);
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        sr.color = new Color(1f, 1f, 1f, 1f - t);

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }

    void SpawnMiniBursts()
    {
        for (int i = 0; i < miniCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized * 0.3f;
            Vector3 spawnPos = transform.position + (Vector3)offset;

            GameObject mini = Instantiate(miniVFXPrefab, spawnPos, Quaternion.identity);

            var miniSR = mini.GetComponentInChildren<SpriteRenderer>();
            if (miniSR != null && sr != null)
            {
                miniSR.sprite = sr.sprite;
            }

            Rigidbody2D rb = mini.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.velocity = offset.normalized * Random.Range(0.5f, 1.5f);
            rb.drag = 2f;
        }
    }
}