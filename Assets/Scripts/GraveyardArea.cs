using UnityEngine;
using UnityEngine.EventSystems;

public class GraveyardArea : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool isPlayerGraveyard = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GraveyardUIManager.Instance != null && !GameManager.Instance.graveyardViewActive)
        {
            var cards = isPlayerGraveyard ?
                GameManager.Instance.humanPlayer.Graveyard :
                GameManager.Instance.aiPlayer.Graveyard;
            GraveyardUIManager.Instance.Open(cards);
        }
    }
}
