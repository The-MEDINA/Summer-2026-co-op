using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using cardIndex;

public class DeckInstanceDeckbuilderScript : MonoBehaviour
{
    public static DeckInstanceDeckbuilderScript instance;

    [SerializeField] private GameObject prefab;
    private List<GameObject> cardObjects = new List<GameObject>();
    private List<NewVirtualCardParent> deck = new List<NewVirtualCardParent>();
    private CommanderCardScript commander = new CommanderCardScript();

    public List<NewVirtualCardParent> Deck { get { return this.deck; } } 
    public CommanderCardScript Commander { get { return commander; } }

    private void Awake()
    {
        //singleton instance code
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ChangeFactionCards("Cat");
    }

    public bool AddCard(string cardName)
    {
        //deck at capacity
        //too many copies of that card

        NewVirtualCardParent cardToAdd = cardIndex.Index.CreateCard(cardName, NewVirtualCardParent.location.deck);
        Deck.Add(cardToAdd);
        Network.Networking.P1InitialDeck.Add(cardToAdd);
        return true;
    }

    public bool RemoveCard(string cardName)
    {
        if(Deck.Count <= 0) { return false; }

        for (int i = 0; i < Deck.Count; i++)
        {
            if (Deck[i].CardName == cardName)
            {
                Network.Networking.P1InitialDeck.RemoveAt(i);
                Deck.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    private void ChangeFactionCards(string faction)
    {
        while (cardObjects.Count != 0)
        {
            Destroy(cardObjects[0]);
            cardObjects.RemoveAt(0);
        }
        List<Details> factionCards = cardIndex.Index.GetAllFactionCards(faction);
        for (int i = 0; i < factionCards.Count; i++)
        {
            GameObject deckCard = Instantiate(prefab);
            deckCard.transform.position = new Vector3(-7 + (2.25f * i), 3, 0);
            deckCard.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            if (factionCards[i].type != NewVirtualCardParent.type.none)
            {
                NewVirtualCardParent cardData = cardIndex.Index.CreateCard(factionCards[i].name, NewVirtualCardParent.location.deck);
                deckCard.GetComponent<DeckbuilderCard>().CardInstance = cardData;
            }
            else
            {
                cardIndex.Index.AttachCommanderCard(deckCard, factionCards[i].name, null);
                deckCard.GetComponent<DeckbuilderCard>().UpdateUI();
            }
            deckCard.GetComponent<DeckbuilderCard>().DeckInstance = this;
        }
    }
}
