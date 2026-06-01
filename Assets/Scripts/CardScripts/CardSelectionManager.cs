using UnityEngine;
using Network;
using UnityEngine.EventSystems;
using UnityEditor;

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
        // network manager needs this.
        // It should only be temporary since I think TryAttackTarget needs to be reworked.
        // once we have attacking sorted, I should be able to get rid of this. - Dave
        set { selectedCardObject = value; }
        
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

        Player owner = cardObject.OwnerPlayer;

        if (owner != null && !owner.CanMove)
        {
            Debug.Log("Move timer active. Wait " + owner.MoveCooldownRemaining.ToString("0.0") + " seconds.");
            ClearSelection();
            if (owner.IsPlayerTwo)
            {
                Debug.LogWarning("Overriding playerTwo.CanMove to prevent desync.");
            }
            else
            {
                return;
            }
        }

        if (cardObject.CardData.CardLocation == NewVirtualCardParent.location.hand)
        {
            PlayCardToBattleground(cardObject);
        }

        ClearSelection();
    }

    // Network manager needs to be able to access this method, so I'm making it public.
    // if we *really* don't want that, let me know and i'll find some alternate way to do this. - Dave
    public void PlayCardToBattleground(CardClickHandler cardObject)
    {
        Player owner = cardObject.OwnerPlayer;

        if (owner == null)
        {
            Debug.LogWarning("This card has no owner player.");
            if (owner.IsPlayerTwo) Networking.DesyncWarning("Player two tried to play a card with no owner");
            return;
        }

        if (!owner.CanMove)
        {
            Debug.Log("Move timer active. Wait " + owner.MoveCooldownRemaining.ToString("0.0") + " seconds.");
            if (owner.IsPlayerTwo)
            {
                Debug.LogWarning("Overriding player two move timer to prevent desync.");
            }
            else
            {
                return;
            }
        }

        if (!owner.CanAfford(cardObject.CardData))
        {
            Debug.Log("Cannot play card. Not enough energy.");
            if (owner.IsPlayerTwo)
            {
                Debug.LogWarning("Overriding player two CanAfford to prevent desync.");
            }
            else 
            {
                return;
            }
        }

        if (!owner.SpendEnergy(cardObject.CardData.Cost))
        {
            return;
        }

        // check to see if this move should be sent to peer.
        if (owner == player1)
        {
            Networking.SendCardMove(cardObject.CardData, // card to move 
                NewVirtualCardParent.location.hand, // in the player's hand
                cardObject.OwnerPlayer.Hand.IndexOf(cardObject.CardData), // index of card in their hand
                NewVirtualCardParent.location.inPlay); // moved to inPlay
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

        for (int i = 0; i < owner.InPlay.Count; i++)
        {
            RefreshCardVisual(owner.InPlay[i].UnityObject.GetComponent<CardClickHandler>());
        }

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

    public void TryAttackTarget(CardClickHandler targetCard, bool wasSecondAttack)
    {
        if (selectedCardObject == null || targetCard == null)
        {
            if (selectedCardObject.OwnerPlayer.IsPlayerTwo) Networking.DesyncWarning("Player two's attacking or target card was null");
            ClearSelection();
            return;
        }

        Player attackingOwner = selectedCardObject.OwnerPlayer;

        if (attackingOwner != null && !attackingOwner.CanMove)
        {
            Debug.Log("Move timer active. Wait " + attackingOwner.MoveCooldownRemaining.ToString("0.0") + " seconds.");
            if (selectedCardObject.OwnerPlayer.IsPlayerTwo)
            {
                Debug.LogWarning("Overriding player two active timer to try to prevent desync.");
            }
            else
            {
                ClearSelection();
                return;
            }
        }

        if (selectedCardObject.CardData.CardLocation != NewVirtualCardParent.location.inPlay)
        {
            Debug.Log("Card must be in play before it can attack.");
            if (selectedCardObject.OwnerPlayer.IsPlayerTwo) Networking.DesyncWarning("Player two is attacking with card not in play");
            ClearSelection();
            return;
        }

        if (targetCard.CardData.CardLocation != NewVirtualCardParent.location.inPlay)
        {
            Debug.Log("Target card must be in play.");
            if (selectedCardObject.OwnerPlayer.IsPlayerTwo) Networking.DesyncWarning("Player two is attacking a card that's not in play");
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
            if (selectedCardObject.OwnerPlayer.IsPlayerTwo) Networking.DesyncWarning("Only minion cards can attack right now for player two");
            ClearSelection();
            return;
        }

        // send this attack if this is player 1.
        if (!selectedCardObject.OwnerPlayer.IsPlayerTwo)
        {
            Networking.SendCardAttack(attacker, target, wasSecondAttack);
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
                    RefreshCardVisual(selectedCardObject);
                    RefreshCardVisual(targetCard);
                    ClearSelection();
                    return;
                }
                else
                {
                    twoAttackMinion.CheckAttack(1, target);
                    Debug.Log("Attacked enemy card. Enemy health: " + target.Health);
                    selectedCardObject.OwnerPlayer.RegisterAction();
                    RefreshCardVisual(selectedCardObject);
                    RefreshCardVisual(targetCard);
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
                    RefreshCardVisual(selectedCardObject);
                    RefreshCardVisual(targetCard);
                    ClearSelection();
                    return;
                }
                else
                {
                    twoAttackMinion.CheckAttack(1, target);
                    Debug.Log("Attacked enemy card. Enemy health: " + target.Health);
                    selectedCardObject.OwnerPlayer.RegisterAction();
                    RefreshCardVisual(selectedCardObject);
                    RefreshCardVisual(targetCard);
                    ClearSelection();
                    return;
                }
            }
        }

        if(attacker.CardEffect == MinionParent.effect.aoe)
        {
            attacker.AOEAttack(targetCard.OwnerPlayer.InPlay, false);
            selectedCardObject.OwnerPlayer.RegisterAction();
            RefreshCardVisual(selectedCardObject);
            RefreshCardVisual(targetCard);
            ClearSelection();
            return;
        }

        attacker.Attack(target);

        if (attackingOwner != null)
        {
            attackingOwner.RegisterAction();
        }

        RefreshCardVisual(selectedCardObject);
        RefreshCardVisual(targetCard);

        Debug.Log(attacker.CardName + " attacked " + target.CardName + ". Target health: " + target.Health);
        selectedCardObject.OwnerPlayer.RegisterAction();

        ClearSelection();
    }

    // network manager needs this, so I'm making it public. - Dave
    public void TrySpellTarget(CardClickHandler targetCard)
    {
        if (selectedCardObject == null || targetCard == null)
        {
            ClearSelection();
            if (selectedCardObject.OwnerPlayer.IsPlayerTwo) Networking.DesyncWarning("Player two's attacking or target card was null");
            return;
        }

        if (selectedCardObject.CardData.CardLocation != NewVirtualCardParent.location.hand)
        {
            Debug.Log("Card must be in your hand before it can be played.");
            if (selectedCardObject.OwnerPlayer.IsPlayerTwo) Networking.DesyncWarning("Player two is playing card from hand");
            ClearSelection();
            return;
        }

        if (targetCard.CardData.CardLocation != NewVirtualCardParent.location.inPlay)
        {
            Debug.Log("Target card must be in play.");
            if (selectedCardObject.OwnerPlayer.IsPlayerTwo) Networking.DesyncWarning("Player two is targetting card not in play");
            ClearSelection();
            return;
        }

        SpellParent attacker = selectedCardObject.CardData as SpellParent;
        MinionParent target = targetCard.CardData as MinionParent;

        if (attacker == null || target == null)
        {
            Debug.Log("Only minion cards can attack right now.");
            if (selectedCardObject.OwnerPlayer.IsPlayerTwo) Networking.DesyncWarning("Only minion cards can attack right now for player two");
            ClearSelection();
            return;
        }

        attacker.OnPlay(target);

        // send this action if this is player 1.
        if (!selectedCardObject.OwnerPlayer.IsPlayerTwo)
        {
            Networking.SendCardAttack(attacker, target, false);
        }

        selectedCardObject.gameObject.SetActive(false);

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

    // This method probably needs a way to figure out if a second attack was made.
    // TryAttackTarget needs that to properly function.
    // Currently I'm setting it always false. - Dave
    public void AttackOpposingTeamButton()
    {
        if (selectedCardObject == null || selectedCardObject.CardData == null)
        {
            Debug.Log("No card selected.");
            return;
        }

        Player attackerOwner = selectedCardObject.OwnerPlayer;

        if (attackerOwner == null)
        {
            Debug.Log("Selected card has no owner.");
            ClearSelection();
            return;
        }

        if (!attackerOwner.CanMove)
        {
            Debug.Log("Move timer active. Wait " + attackerOwner.MoveCooldownRemaining.ToString("0.0") + " seconds.");
            ClearSelection();
            return;
        }

        if (selectedCardObject.CardData.CardLocation != NewVirtualCardParent.location.inPlay)
        {
            Debug.Log("Selected card must be in play before attacking.");
            ClearSelection();
            return;
        }

        Player targetPlayer = null;

        if (attackerOwner == player1)
        {
            targetPlayer = player2;
        }
        else if (attackerOwner == player2)
        {
            targetPlayer = player1;
        }

        if (targetPlayer == null || targetPlayer.InPlay.Count <= 0)
        {
            Debug.Log("Opposing team has no cards in play.");
            ClearSelection();
            return;
        }

        CardClickHandler targetCard = FindFirstVisibleCardForPlayer(targetPlayer);

        if (targetCard == null)
        {
            Debug.Log("Could not find opposing card object.");
            ClearSelection();
            return;
        }

        TryAttackTarget(targetCard, false);
    }

private CardClickHandler FindFirstVisibleCardForPlayer(Player targetPlayer)
{
    CardClickHandler[] allCards = FindObjectsByType<CardClickHandler>(FindObjectsSortMode.None);

    for (int i = 0; i < allCards.Length; i++)
    {
        if (allCards[i].OwnerPlayer == targetPlayer &&
            allCards[i].CardData != null &&
            allCards[i].CardData.CardLocation == NewVirtualCardParent.location.inPlay &&
            allCards[i].gameObject.activeInHierarchy)
        {
            return allCards[i];
        }
    }

    return null;
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