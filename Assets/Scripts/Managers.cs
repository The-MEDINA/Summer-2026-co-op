using UnityEngine;

public class CardSelectionManager : MonoBehaviour
{
    public static CardSelectionManager Instance;

    public CardParent selectedCard;

    private void Awake()
    {
        Instance = this;
    }

    public void SelectCard(CardParent card)
    {
        if (selectedCard == card)
        {
            ActivateCard(card);
            return;
        }

        selectedCard = card;

        Debug.Log(card.CardName + " selected");
    }

    public void ActivateCard(CardParent card)
    {
        Debug.Log(card.CardName + " activated");

        selectedCard = null;
    }

    public void ClearSelection()
    {
        selectedCard = null;
    }
}