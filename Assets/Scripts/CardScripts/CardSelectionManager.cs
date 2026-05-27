using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelectionManager : MonoBehaviour
{
    public static CardSelectionManager Instance;

    [Header("Player 1")]
    [SerializeField] private Player player1;
    [SerializeField] private GameObject player1CardField;
    [SerializeField] private HandUIManager player1HandUI;

    [Header("Player 2")]
    [SerializeField] private Player player2;
    [SerializeField] private GameObject player2CardField;
    [SerializeField] private HandUIManager player2HandUI;

    [Header("Field Layout")]
    [SerializeField] private float player1StartX = -5f;
    [SerializeField] private float player2StartX = -5f;
    [SerializeField] private float cardSpacing = 2f;

    private CardClickHandler selectedCardObject;

    private int player1FieldPosition = 0;
    private int player2FieldPosition = 0;

    public CardClickHandler SelectedCardObject
    {
        get { return selectedCardObject; }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void SelectCard(CardClickHandler clickedCard, PointerEventData eventData)
    {
        if (clickedCard == null || clickedCard.CardData == null)
        {
            Debug.LogWarning("Clicked card has no card data.");
            return;
        }

        if (selectedCardObject != null && selectedCardObject != clickedCard)
        {
            if (selectedCardObject.CardData is MinionParent)
            {
                MinionParent minion = (MinionParent)selectedCardObject.CardData;
                if (minion.CardEffect == MinionParent.effect.twoAttacks && eventData.button == PointerEventData.InputButton.Right)
                {
                    TryAttackTarget(clickedCard, true);
                }
                else
                {
                    TryAttackTarget(clickedCard, false);
                }
            }
            else if(selectedCardObject.CardData is SpellParent) //still need to implement spelling and attacking players
            {
                TrySpellTarget(clickedCard);
            }
            return;
        }

        if (selectedCardObject == clickedCard)
        {
            if(clickedCard.CardData is MinionParent)
            {
                ActivateCard(clickedCard);
                return;
            }
        }

        selectedCardObject = clickedCard;
        selectedCardObject.SetSelectedVisual(true);

        Debug.Log("Selected card: " + clickedCard.CardData.CardName);
    }

    private void ActivateCard(CardClickHandler cardObject)
    {
        if (cardObject == null || cardObject.CardData == null)
        {
            ClearSelection();
            return;
        }

        if (cardObject.CardData.CardLocation == NewVirtualCardParent.location.hand)
        {
            PlayCardToBattleground(cardObject);
        }

        ClearSelection();
    }

    private void PlayCardToBattleground(CardClickHandler cardObject)
    {
        Player owner = cardObject.OwnerPlayer;

        if (owner == null)
        {
            Debug.LogWarning("This card has no owner player.");
            return;
        }

        if (!owner.CanAfford(cardObject.CardData))
        {
            Debug.Log("Cannot play card. Not enough energy.");
            return;
        }

        if (!owner.SpendEnergy(cardObject.CardData.Cost))
        {
            return;
        }

        owner.MoveCardToInPlay(cardObject.CardData);

        if (owner == player1)
        {
            if (player1HandUI != null)
            {
                player1HandUI.RemoveCardFromHand(cardObject.gameObject);
            }

            MoveCardToField(cardObject, player1CardField, player1StartX, player1FieldPosition);
            player1FieldPosition++;
        }
        else if (owner == player2)
        {
            if (player2HandUI != null)
            {
                player2HandUI.RemoveCardFromHand(cardObject.gameObject);
            }

            MoveCardToField(cardObject, player2CardField, player2StartX, player2FieldPosition);
            player2FieldPosition++;
        }
        else
        {
            Debug.LogWarning("Card owner does not match Player 1 or Player 2.");
        }

        RefreshCardVisual(cardObject);

        Debug.Log("Card moved to battleground. Energy left: " + owner.Energy);
    }

    private void MoveCardToField(CardClickHandler cardObject, GameObject field, float startX, int position)
    {
        if (field == null)
        {
            Debug.LogWarning("Missing card field reference.");
            return;
        }

        cardObject.transform.position = new Vector3(
            startX + (cardSpacing * position),
            field.transform.position.y,
            -0.1f
        );
    }

    private void TryAttackTarget(CardClickHandler targetCard, bool wasSecondAttack)
    {
        if (selectedCardObject == null || targetCard == null)
        {
            ClearSelection();
            return;
        }

        if (selectedCardObject.CardData.CardLocation != NewVirtualCardParent.location.inPlay)
        {
            Debug.Log("Card must be in play before it can attack.");
            ClearSelection();
            return;
        }

        if (targetCard.CardData.CardLocation != NewVirtualCardParent.location.inPlay)
        {
            Debug.Log("Target card must be in play.");
            ClearSelection();
            return;
        }

        if (selectedCardObject.OwnerPlayer == targetCard.OwnerPlayer)
        {
            Debug.Log("You cannot attack your own card.");
            ClearSelection();
            return;
        }

        MinionParent attacker = selectedCardObject.CardData as MinionParent;
        MinionParent target = targetCard.CardData as MinionParent;

        if (attacker == null || target == null)
        {
            Debug.Log("Only minion cards can attack right now.");
            ClearSelection();
            return;
        }

        if (attacker is TwoAttackParent)
        {
            TwoAttackParent twoAttackMinion = (TwoAttackParent)attacker;

            if (twoAttackMinion.SecondaryCardEffect == MinionParent.effect.aoe)
            {
                if (wasSecondAttack)
                {
                    twoAttackMinion.CheckAOEAttack(2, target, targetCard.OwnerPlayer.InPlay);
                    Debug.Log("Attacked enemy card. Enemy health: " + target.Health);
                    selectedCardObject.OwnerPlayer.RegisterAction();
                    ClearSelection();
                    return;
                }
                else
                {
                    twoAttackMinion.CheckAttack(1, target);
                    Debug.Log("Attacked enemy card. Enemy health: " + target.Health);
                    selectedCardObject.OwnerPlayer.RegisterAction();
                    ClearSelection();
                    return;
                }
            }
            else
            {
                if (wasSecondAttack)
                {
                    twoAttackMinion.CheckAttack(2, target);
                    Debug.Log("Attacked enemy card. Enemy health: " + target.Health);
                    selectedCardObject.OwnerPlayer.RegisterAction();
                    ClearSelection();
                    return;
                }
                else
                {
                    twoAttackMinion.CheckAttack(1, target);
                    Debug.Log("Attacked enemy card. Enemy health: " + target.Health);
                    selectedCardObject.OwnerPlayer.RegisterAction();
                    ClearSelection();
                    return;
                }
            }
        }

        if(attacker.CardEffect == MinionParent.effect.aoe)
        {
            attacker.AOEAttack(targetCard.OwnerPlayer.InPlay, false);
            selectedCardObject.OwnerPlayer.RegisterAction();
            ClearSelection();
            return;
        }

        attacker.Attack(target);

        RefreshCardVisual(selectedCardObject);
        RefreshCardVisual(targetCard);

        Debug.Log(attacker.CardName + " attacked " + target.CardName + ". Target health: " + target.Health);
        selectedCardObject.OwnerPlayer.RegisterAction();

        ClearSelection();
    }

    private void TrySpellTarget(CardClickHandler targetCard)
    {
        if (selectedCardObject == null || targetCard == null)
        {
            ClearSelection();
            return;
        }

        if (selectedCardObject.CardData.CardLocation != NewVirtualCardParent.location.hand)
        {
            Debug.Log("Card must be in your hand before it can be played.");
            ClearSelection();
            return;
        }

        if (targetCard.CardData.CardLocation != NewVirtualCardParent.location.inPlay)
        {
            Debug.Log("Target card must be in play.");
            ClearSelection();
            return;
        }

        SpellParent attacker = selectedCardObject.CardData as SpellParent;
        MinionParent target = targetCard.CardData as MinionParent;

        if (attacker == null || target == null)
        {
            Debug.Log("Only minion cards can attack right now.");
            ClearSelection();
            return;
        }

        attacker.OnPlay(target);

        //move spell
        //destroy spell

        RefreshCardVisual(selectedCardObject);
        RefreshCardVisual(targetCard);

        Debug.Log(attacker.CardName + " played on " + target.CardName + ". Target health: " + target.Health);
        selectedCardObject.OwnerPlayer.RegisterAction();

        ClearSelection();
    }

    private void RefreshCardVisual(CardClickHandler card)
    {
        if (card == null)
        {
            return;
        }

        CardUIManager uiManager = card.GetComponent<CardUIManager>();

        if (uiManager != null)
        {
            uiManager.RefreshCardUI();
        }
    }

    public void ClearSelection()
    {
        if (selectedCardObject != null)
        {
            selectedCardObject.SetSelectedVisual(false);
        }

        selectedCardObject = null;
    }
}