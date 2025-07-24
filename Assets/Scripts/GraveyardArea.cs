using UnityEngine;
using UnityEngine.EventSystems;

public class GraveyardArea : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool isPlayerGraveyard = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance == null || GameManager.Instance.graveyardViewActive)
            return;

        if (isPlayerGraveyard)
            GameManager.Instance.ShowPlayerGraveyard();
        else
            GameManager.Instance.ShowOpponentGraveyard();
    }
}
