using UnityEngine;

public class CardSelectionManager : MonoBehaviour
{
    public static CardSelectionManager Instance;

    private CardClickHandler selectedCardObject;

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