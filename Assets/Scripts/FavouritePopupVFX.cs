using UnityEngine;

public class FavouritePopupVFX : MonoBehaviour
{
    public float duration = 1.0f;
    public float maxScale = 1.0f;
    public float rotationSpeed = 90f;
    public float moveUpDistance = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip popupSound;

    private float timer;
    private Vector3 startPos;
    private Vector3 targetPos;
    private CanvasGroup canvasGroup;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * moveUpDistance;
        transform.localScale = Vector3.zero;
        canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        if (popupSound != null)
            audioSource.PlayOneShot(popupSound);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        transform.position = Vector3.Lerp(startPos, targetPos, t);
        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * maxScale, t);
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        if (canvasGroup != null)
            //canvasGroup.alpha = 1f - t;

        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}
