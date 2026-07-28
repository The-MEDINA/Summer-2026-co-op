using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Network;

public class Battleground : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Player p;
    [SerializeField] private GameObject cardProto;
    [SerializeField] private HandUIManager handUIManager;
    [SerializeField] private CommanderCardScript commanderCard;

    [SerializeField] private Sprite mainSprite;
    [SerializeField] private Sprite onClickSprite;

    public Player P { get { return p; } }
    public GameObject CardProto { get { return cardProto; } }
    public CommanderCardScript CommanderCard { get { return commanderCard; } }

    private List<GameObject> cardList = new List<GameObject>();

    // network manager needs this to instantiate cards.
    public void Start()
    {
        if (p.IsPlayerTwo)
        {
            Networking.P2Battleground = this;
            Networking.P2HandUI = handUIManager;
        }
        p.CommanderCard = commanderCard;
    }

    /// <summary>
    /// handles clicking a button to draw cards
    /// </summary>
    /// <param name="eventData">mouse click on button</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        // Don't run if network manager is trying to resolve a desync.
        if (Networking.CurrentState == state.paused) return;
        
        //when testing locally, enable bool isLocalTesting in inspector on CardSelectionManager.Ins, when playing online, disable it - Jacob
        if (!P.IsPlayerTwo || CardSelectionManager.Instance.IsLocalTesting)
        {
            Debug.Log("Clicked deck: " + gameObject.name);
            if (p.Deck.Count > 0) Networking.SendCardAdd(p.Deck[0], NewVirtualCardParent.location.hand);
            GetComponent<SpriteRenderer>().sprite = mainSprite;
            DrawCardToHand();
        } 
        else 
        { 
            Debug.LogWarning("Drawing player 2's cards are not allowed.");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GetComponent<SpriteRenderer>().sprite = onClickSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<SpriteRenderer>().sprite = onClickSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<SpriteRenderer>().sprite = mainSprite;
    }

    // network manager needs this, so i'm making it public for now.
    // If we REALLY don't want this I'll find some alternate way to do this. - Dave
    //Of all the methods to make public, this is high up the board for being completely fine lol I think we're good - Jake
    public void DrawCardToHand()
    {
        // This is a quick band-aid fix to prevent some cards from activating after being clicked once, when they're drawn from the deck at least twice.
        // I found a bug that seems to only activate in a VERY specific situation.
        // When you only have ONE spell that targets none in your deck, the second time you add it to your hand and try to play it, it activates immediately.
        // For some reason selectedCardObject in CardSelectionManager changes itself from null to the card???
        // It doesn't seem to be a bug with our code exactly, because an additional OnClick event is being called when it shouldn't. I Have no idea how that's happening.
        // So... This is a workaround to prevent that. Please leave this in here.
        // I'm also tired and don't think I can spend any more time debugging this. It's 10:42 PM on a Monday.
        // :<
        // - Dave
        // Nevermind this caused more bugs GRRRRAHHHHHHHHH
        // CardSelectionManager.Instance.ClearSelection();

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
        CardSelectionManager.Instance.SfxManager.RegisterCard(drawnCard);
        Debug.Log(p.gameObject.name + " drew card: " + drawnCard.CardName);

        if (drawnCard is MinionParent)
        {
            MinionParent mp = (MinionParent)drawnCard;
            mp.IsDead = false;
        }
    }

    public void SpawnCardToInPlay(NewVirtualCardParent spawnCard)
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

        spawnCard.CardLocation = NewVirtualCardParent.location.inPlay;

        GameObject newCard = Instantiate(cardProto);
        cardList.Add(newCard);
        CardSelectionManager.Instance.SfxManager.RegisterCard(spawnCard);
        CardClickHandler clickHandler = newCard.GetComponent<CardClickHandler>();

        if (clickHandler != null)
        {
            clickHandler.CardData = spawnCard;
            clickHandler.OwnerPlayer = p;
        }
        // every card instantiated needs a reference to its gameobject from now on.
        spawnCard.UnityObject = newCard;

        p.InPlay.Add(spawnCard);

        if (spawnCard.CardType == NewVirtualCardParent.type.token)
        {
            CardSelectionManager.Instance.PlayCardToBattleground(clickHandler);
        }
        if (!clickHandler.InPlay) clickHandler.InPlay = true;

        MinionParent frozenCheck = (MinionParent)spawnCard;
        if (frozenCheck != null && frozenCheck.CardEffect == MinionParent.effect.frozen)
        {
            newCard.GetComponent<CardClickHandler>().SetSpeed(CardClickHandler.speed.frozen);
            newCard.GetComponent<CardClickHandler>().ResetTimer();
            newCard.GetComponent<CardUIManager>().AddProgress(5f);
        }
        Debug.Log(p.gameObject.name + " played card: " + spawnCard.CardName);
    }

    // adding this because the connection often stays active even after the game is closed.
    // This is a quick solution to close it manually and shouldn't be called very often, if at all later on. - Dave
    private void OnApplicationQuit()
    {
        Networking.CloseConnection();
    }
}