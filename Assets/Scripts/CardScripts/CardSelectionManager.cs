using UnityEngine;

public class CardSelectionManager : MonoBehaviour
{
    public static CardSelectionManager Instance;

    [SerializeField] private GameObject activeCardsRectangle;
    private CardClickHandler selectedCardObject;

    //test variable will be changed to index of array later
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
        if (clickedCard == null)
        {
            return;
        }

        if(clickedCard.IsEnemyCard == true)
        {
            Debug.Log("Attack");
            selectedCardObject.CardData.Attack(clickedCard.CardData);
            Debug.Log(clickedCard.CardData.Health);
            Debug.Log(clickedCard.CardData.IsDead);
        }

        if (selectedCardObject == clickedCard)
        {
            ActivateCard(clickedCard);
            return;
        }

        if (selectedCardObject != null)
        {
            selectedCardObject.SetSelectedVisual(false);
        }
        selectedCardObject = clickedCard;
        selectedCardObject.SetSelectedVisual(true);

        Debug.Log("Selected card object: " + clickedCard.gameObject.name);
    }

    private void ActivateCard(CardClickHandler cardObject)
    {
        Debug.Log("Activated card object: " + cardObject.gameObject.name);

        if (cardObject.CardData != null)
        {
            Debug.Log("Card data activated");
            cardObject.transform.position = new Vector3(-9 + (2 * position), activeCardsRectangle.transform.position.y, -0.1f);
            position++;
        }

        cardObject.SetSelectedVisual(false);
        selectedCardObject = null;
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