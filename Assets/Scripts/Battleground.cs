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

    // network manager needs this to instantiate cards.
    public void Start()
    {
        if (p.IsPlayerTwo) Networking.P2Battleground = this;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked deck: " + gameObject.name);
        if (p.Deck.Count > 0) Networking.SendCardAdd(p.Deck[0], NewVirtualCardParent.location.hand);
        DrawCardToHand();
    }

    // network manager needs this, so i'm making it public for now.
    // If we REALLY don't want this I'll find some alternate way to do this. - Dave
    public void DrawCardToHand()
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

        drawnCard.CardLocation = NewVirtualCardParent.location.hand;

        GameObject newCard = Instantiate(cardProto);
        cardList.Add(newCard);

        CardClickHandler clickHandler = newCard.GetComponent<CardClickHandler>();

        if (clickHandler != null)
        {
            clickHandler.CardData = drawnCard;
            clickHandler.OwnerPlayer = p;
        }
        // every card instantiated needs a reference to its gameobject from now on.
        drawnCard.UnityObject = newCard;

        p.Hand.Add(drawnCard);
        p.Deck.RemoveAt(0);

        handUIManager.AddCardToHand(newCard);
        Debug.Log(p.gameObject.name + " drew card: " + drawnCard.CardName);
    }
}