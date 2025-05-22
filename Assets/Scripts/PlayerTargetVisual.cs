using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerTargetVisual : MonoBehaviour, IPointerClickHandler
{
    public bool isHuman;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.targetingSorcery != null &&
            GameManager.Instance.targetingSorcery.requiresTarget &&
            (GameManager.Instance.targetingSorcery.requiredTargetType == SorceryCard.TargetType.Player ||
             GameManager.Instance.targetingSorcery.requiredTargetType == SorceryCard.TargetType.CreatureOrPlayer))
        {
            Debug.Log($"Player target clicked: {(isHuman ? "Human" : "AI")}");

            Player target = isHuman ? GameManager.Instance.humanPlayer : GameManager.Instance.aiPlayer;

            GameManager.Instance.CompletePlayerTargetSelection(target);
        }
    }
}