using UnityEngine;
using UnityEngine.EventSystems;

public class GraveyardAreaClick : MonoBehaviour, IPointerClickHandler
{
    public bool isHumanArea = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GraveyardView.Instance == null)
            return;

        if (GameManager.Instance.isGraveyardOpen)
            return;

        var expectedPlayer = isHumanArea ? TurnSystem.PlayerType.Human : TurnSystem.PlayerType.AI;
        if (TurnSystem.Instance.currentPlayer != expectedPlayer)
            return;

        var owner = isHumanArea ? GameManager.Instance.humanPlayer : GameManager.Instance.aiPlayer;
        GraveyardView.Instance.Open(owner);
    }
}
