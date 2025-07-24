using UnityEngine;
using UnityEngine.EventSystems;

public class GraveyardArea : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GraveyardUIManager.Instance != null && !GameManager.Instance.graveyardViewActive)
        {
            GraveyardUIManager.Instance.Open(GameManager.Instance.humanPlayer.Graveyard);
        }
    }
}
