using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Battleground : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Player p;
    [SerializeField] private GameObject cardProto;
    [SerializeField] private HandUIManager handUIManager;

    private List<GameObject> cardList = new List<GameObject>();
    private CardClickHandler currentCard;

    private void Update()
    {
        if (p != null && p.canDraw == true && p.Deck.Count > 0)
        {
            DrawCardToHand();
            p.canDraw = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (p != null && p.Deck.Count > 0)
        {
            DrawCardToHand();
        }
    }

    private void DrawCardToHand()
    {
        CardParent drawnCard = p.Deck[0];

        p.Hand.Add(drawnCard);
        p.Deck.RemoveAt(0);

        drawnCard.CardLocation = CardParent.location.hand;

        GameObject newCard = Instantiate(cardProto);
        cardList.Add(newCard);

        currentCard = newCard.GetComponent<CardClickHandler>();

        if (currentCard != null)
        {
            currentCard.CardData = drawnCard;
        }

        if (handUIManager != null)
        {
            handUIManager.AddCardToHand(newCard);
        }
        else
        {
            newCard.transform.position = new Vector3(-5.75f + ((p.Hand.Count - 1) * 2f), -3.75f, -0.1f);
            Debug.LogWarning("No HandUIManager assigned. Using fallback position.");
        }

        Debug.Log("Drew card. Cards in hand: " + p.Hand.Count);
    }
}