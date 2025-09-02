using UnityEngine;
using UnityEngine.UI;

public class ResolveButtonUI : MonoBehaviour
{
    [SerializeField] private Button resolveButton;

    void Awake()
    {
        if (resolveButton != null)
        {
            resolveButton.onClick.AddListener(OnResolveClick);
            resolveButton.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (resolveButton != null)
        {
            resolveButton.gameObject.SetActive(GameManager.Instance != null && GameManager.Instance.IsStackActive());
        }
    }

    private void OnResolveClick()
    {
        GameManager.Instance?.ResolveStackNow();
    }
}
