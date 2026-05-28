using UnityEngine;
using Network;

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

    public void SelectCard(CardClickHandler clickedCard)
    {
        if (clickedCard == null || clickedCard.CardData == null)
        {
            Debug.LogWarning("Clicked card has no card data.");
            return;
        }

        if (selectedCardObject != null && selectedCardObject != clickedCard)
        {
            TryAttackTarget(clickedCard);
            return;
        }

        if (selectedCardObject == clickedCard)
        {
            ActivateCard(clickedCard);
            return;
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

    // Network manager needs to be able to access this method, so I'm making it public.
    // if we *really* don't want that, let me know and i'll find some alternate way to do this. - Dave
    public void PlayCardToBattleground(CardClickHandler cardObject)
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

    // network manager needs this, so.. making it public.
    // this should be temporary, since I think this method needs to be reworked. - Dave
    public void TryAttackTarget(CardClickHandler targetCard)
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

        // send this attack if this is player 1.
        if (!selectedCardObject.OwnerPlayer.IsPlayerTwo)
        {
            Networking.SendCardAttack(attacker, target);
        }

        target.TakeDamage(attacker, attacker.Damage);

        RefreshCardVisual(selectedCardObject);
        RefreshCardVisual(targetCard);

        Debug.Log(attacker.CardName + " attacked " + target.CardName + ". Target health: " + target.Health);

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