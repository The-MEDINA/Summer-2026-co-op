using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Network;

public class Battleground : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Player p;
    [SerializeField] private GameObject cardProto;
    [SerializeField] private HandUIManager handUIManager;

    private List<GameObject> cardList = new List<GameObject>();

    // battleground needs an update to check if network manager wants to instantiate a card.
    // If we REALLY don't want this method let me know and I'll find some other way to do this.
    // If this is fine, yall can remove this block of comments. - Dave
    public void Update()
    {
        if (Networking.RequestCardInstantiation != -1 && p.IsPlayerTwo)
        {
            DrawCardToHand();
            Networking.RequestCardInstantiation = -1;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked deck: " + gameObject.name);
        if (p.Deck.Count > 0) Networking.SendCardAdd(p.Deck[0], NewVirtualCardParent.location.hand);
        DrawCardToHand();
    }

    private void DrawCardToHand()
    {
        if (p == null)
        {
            Debug.LogWarning(gameObject.name + " has no Player assigned.");
            return;
        }

        if (cardProto == null)
        {
            Debug.LogWarning(gameObject.name + " has no Card Proto assigned.");
            return;
        }

        if (handUIManager == null)
        {
            Debug.LogWarning(gameObject.name + " has no Hand UI Manager assigned.");
            return;
        }

        if (p.Deck.Count <= 0)
        {
            Debug.LogWarning(p.gameObject.name + " deck is empty.");
            return;
        }

        NewVirtualCardParent drawnCard = p.Deck[0];

        p.Hand.Add(drawnCard);
        p.Deck.RemoveAt(0);

        drawnCard.CardLocation = NewVirtualCardParent.location.hand;

        GameObject newCard = Instantiate(cardProto);
        cardList.Add(newCard);

        CardClickHandler clickHandler = newCard.GetComponent<CardClickHandler>();

        if (clickHandler != null)
        {
            clickHandler.CardData = drawnCard;
            clickHandler.OwnerPlayer = p;
        }

        handUIManager.AddCardToHand(newCard);

        Debug.Log(p.gameObject.name + " drew card: " + drawnCard.CardName);
    }
}