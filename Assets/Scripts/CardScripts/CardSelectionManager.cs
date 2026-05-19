using UnityEngine;

public class CardSelectionManager : MonoBehaviour
{
    public static CardSelectionManager Instance;

    [SerializeField] private GameObject activeCardsRectangle;
    [SerializeField] private HandUIManager handUIManager;

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

        Debug.Log("Selected card object: " + clickedCard.gameObject.name);
    }

    private void TryAttackTarget(CardClickHandler targetCard)
    {
        if (selectedCardObject == null || targetCard == null)
        {
            return;
        }

        if (selectedCardObject.IsEnemyCard == false && targetCard.IsEnemyCard == true)
        {
            if (selectedCardObject.CardData.CardLocation == CardParent.location.inPlay)
            {
                selectedCardObject.CardData.Attack(targetCard.CardData);

                Debug.Log("Attacked enemy card. Enemy health: " + targetCard.CardData.Health);

                ClearSelection();
            }
        }
    }

    private void ActivateCard(CardClickHandler cardObject)
    {
        Debug.Log("Activated card object: " + cardObject.gameObject.name);

        if (cardObject.CardData != null &&
            cardObject.IsEnemyCard == false &&
            cardObject.CardData.CardLocation != CardParent.location.inPlay)
        {
            PlayCardToBattleground(cardObject);
        }

        ClearSelection();
    }

    private void PlayCardToBattleground(CardClickHandler cardObject)
    {
        if (activeCardsRectangle == null)
        {
            Debug.LogWarning("No activeCardsRectangle assigned.");
            return;
        }

        if (handUIManager != null)
        {
            handUIManager.RemoveCardFromHand(cardObject.gameObject);
        }

        cardObject.transform.position = new Vector3(
            -9 + (2 * position),
            activeCardsRectangle.transform.position.y,
            -0.1f
        );

        cardObject.CardData.CardLocation = CardParent.location.inPlay;
        position++;

        Debug.Log("Card moved to battleground.");
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