using UnityEngine;
using UnityEngine.UI;

public class ResolveButtonUI : MonoBehaviour
{
    [SerializeField] private Button resolveButton;
    [SerializeField] private Button pauseStackButton;

    void Awake()
    {
        if (resolveButton != null)
        {
            resolveButton.onClick.AddListener(OnResolveClick);
            resolveButton.gameObject.SetActive(false);
        }

        if (pauseStackButton != null)
        {
            pauseStackButton.onClick.AddListener(OnPauseClick);
            pauseStackButton.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        GameManager manager = GameManager.Instance;
        bool stackActive = manager != null && manager.IsStackActive();

        if (resolveButton != null)
            resolveButton.gameObject.SetActive(stackActive);

        if (pauseStackButton != null)
        {
            pauseStackButton.gameObject.SetActive(stackActive);
            pauseStackButton.interactable = stackActive && !manager.IsStackTimerPaused();
        }
    }

    private void OnResolveClick()
    {
        GameManager.Instance?.ResolveStackNow();
    }

    private void OnPauseClick()
    {
        GameManager.Instance?.PauseStackTimer();
    }
}
