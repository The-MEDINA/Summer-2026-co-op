using UnityEngine;

public class CardSelectionManager : MonoBehaviour
{
    public static CardSelectionManager Instance;

    [SerializeField] private GameObject activeCardsRectangle;
    [SerializeField] private HandUIManager handUIManager;
    [SerializeField] private Player player;

    [SerializeField] private float battlegroundStartX = -9f;
    [SerializeField] private float battlegroundSpacing = 2f;
    private CardClickHandler selectedCardObject;
    private int position = 0;

    public CardClickHandler SelectedCardObject
    {
        get { return selectedCardObject; }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void SelectCard(CardClickHandler clickedCard)
    {
        if (clickedCard == null || clickedCard.CardData == null)
        {
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

        Debug.Log("Selected card: " + clickedCard.gameObject.name);
    }

    private void ActivateCard(CardClickHandler cardObject)
    {
        if (cardObject == null || cardObject.CardData == null)
        {
            ClearSelection();
            return;
        }

        if (cardObject.IsEnemyCard)
        {
            ClearSelection();
            return;
        }

        if (cardObject.CardData.CardLocation == CardParent.location.hand)
        {
            PlayCardToBattleground(cardObject);
        }

        ClearSelection();
    }

    private void PlayCardToBattleground(CardClickHandler cardObject)
    {
        if (player == null)
        {
            Debug.LogWarning("No Player assigned on CardSelectionManager.");
            return;
        }

        if (activeCardsRectangle == null)
        {
            Debug.LogWarning("No activeCardsRectangle assigned.");
            return;
        }

        if (!player.CanAfford(cardObject.CardData))
        {
            Debug.Log("Cannot play card. Not enough energy.");
            return;
        }

        bool paid = player.SpendEnergy(cardObject.CardData.Cost);

        if (!paid)
        {
            return;
        }

        player.MoveCardToInPlay(cardObject.CardData);

        if (handUIManager != null)
        {
            handUIManager.RemoveCardFromHand(cardObject.gameObject);
        }

        cardObject.transform.position = new Vector3(
            battlegroundStartX + (battlegroundSpacing * position),
            activeCardsRectangle.transform.position.y,
            -0.1f
        );

        position++;

        Debug.Log("Card moved to battleground. Energy left: " + player.Energy);
    }

    private void TryAttackTarget(CardClickHandler targetCard)
    {
        if (selectedCardObject == null || targetCard == null)
        {
            return;
        }

        if (selectedCardObject.IsEnemyCard)
        {
            ClearSelection();
            return;
        }

        if (!targetCard.IsEnemyCard)
        {
            ClearSelection();
            return;
        }

        if (selectedCardObject.CardData.CardLocation != CardParent.location.inPlay)
        {
            Debug.Log("Card must be in play before it can attack.");
            ClearSelection();
            return;
        }

        selectedCardObject.CardData.Attack(targetCard.CardData);

        if (player != null)
        {
            player.RegisterAction();
        }

        Debug.Log("Attacked enemy card. Enemy health: " + targetCard.CardData.Health);

        ClearSelection();
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