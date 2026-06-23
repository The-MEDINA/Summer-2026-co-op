using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Experimental.GraphView.GraphView;

/* This script is for attacking the player.
 * The way we have the button set up right now means right clicks are impossible.
 * So.. as a workaround, we need a new script that can detect right clicks.
 * It shouldn't affect anything that CardSelectionManager is doing, but it does need to grab a lot from it.
 */

public class AttackPlayerManager : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CardSelectionManager selectionManager;
    PointerEventData lastEventData;
    private void Start()
    {
        // lastEventData.button = PointerEventData.InputButton.Left;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        lastEventData = eventData;
        AttackOpposingPlayer();
    }

    public void AttackOpposingPlayer()
    {
        if (selectionManager.SelectedCardObject == null || selectionManager.SelectedCardObject.CardData == null)
        {
            Debug.Log("No card selected.");
            return;
        }

        Player attackerOwner = selectionManager.SelectedCardObject.OwnerPlayer;

        if (attackerOwner == null)
        {
            Debug.Log("Selected card has no owner.");
            selectionManager.ClearSelection();
            return;
        }

        if (selectionManager.SelectedCardObject.CardData.CardLocation != NewVirtualCardParent.location.inPlay)
        {
            Debug.Log("Selected card must be in play before attacking.");
            selectionManager.ClearSelection();
            return;
        }

        if (lastEventData.button == PointerEventData.InputButton.Right) 
            selectionManager.TryAttackPlayer(true);
        else selectionManager.TryAttackPlayer(false);
    }
}
