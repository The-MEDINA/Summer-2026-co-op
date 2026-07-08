using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using cardIndex;

public class DeckInstanceDeckbuilderScript : MonoBehaviour
{
    public static DeckInstanceDeckbuilderScript instance;

    [SerializeField] private GameObject prefab;
    [SerializeField] private int cardsInRow = 7;
    [SerializeField] private float cardSpacing = 2.3f;
    private float highYPos = -100;
    private float lowYPos = 100;
    private List<GameObject> cardObjects = new List<GameObject>();
    private List<NewVirtualCardParent> deck = new List<NewVirtualCardParent>();
    private string commander = "";
    private bool sentLoadout = false;
    private CommanderCardScript commanderInstance;

    public List<NewVirtualCardParent> Deck { get { return this.deck; } } 
    public List<GameObject> CardObjects { get { return cardObjects; } set { cardObjects = value; } }
    public string Commander { get { return commander; } }
    public bool SentLoadout { get { return sentLoadout; } set { sentLoadout = value; } }
    public CommanderCardScript CommanderInstance { get { return commanderInstance; } set { commanderInstance = value; } }

    private void Awake()
    {
        //singleton instance code
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        else
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // remove any lingering cards
        while (cardObjects.Count != 0)
        {
            Destroy(cardObjects[0]);
            cardObjects.RemoveAt(0);
        }
        string[] name = scene.path.Split("/");
        if (name[name.Length - 1] == "DeckbuilderScene.unity") 
        {
            highYPos = -100;
            lowYPos = 100;
            ChangeFactionCards("Cat");
            sentLoadout = false;
        }
    }

    private void Update()
    {
        float move = Input.mouseScrollDelta.y;
        if (move < 0 && highYPos > 3 || move > 0 && lowYPos < -3)
        {
            for (int i = 0; i < cardObjects.Count; i++)
            {
                Vector3 cardPosition = cardObjects[i].transform.position;
                cardPosition.y += move;
                cardObjects[i].transform.position = cardPosition;
            }
            highYPos += move;
            lowYPos += move;
        }
    }

    /// <summary>
    /// Add a card to the loadout.
    /// </summary>
    /// <param name="cardName">Card to add via its name</param>
    /// <returns>whether the addition was successful</returns>
    public bool AddCard(string cardName)
    {
        //deck at capacity
        //too many copies of that card

        NewVirtualCardParent cardToAdd = cardIndex.Index.CreateCard(cardName, NewVirtualCardParent.location.deck);
        Deck.Add(cardToAdd);
        Network.Networking.P1InitialDeck.Add(cardToAdd);
        return true;
    }

    /// <summary>
    /// Add a commander to the loadout.
    /// </summary>
    /// <param name="incomingCommander">Commander to add</param>
    /// <returns>Whether the addition was successful</returns>
    public bool AddCard(CommanderCardScript incomingCommander)
    {
        if (commander == incomingCommander.Name) return false;
        else
        {
            if (commander != "")
            {
                commander = incomingCommander.Name;
                for (int i = 0; i < cardObjects.Count; i++)
                {
                    if (cardObjects[i].gameObject.GetComponent<DeckbuilderCard>() != null)
                    {
                        DeckbuilderCard previousCommanderCard = cardObjects[i].gameObject.GetComponent<DeckbuilderCard>();
                        previousCommanderCard.UpdateUI();
                    }
                }
            }
            else
            {
                commander = incomingCommander.Name;
            }
            commanderInstance = incomingCommander;
        }
        return true;
    }

    /// <summary>
    /// Remove a card from the loadout.
    /// </summary>
    /// <param name="cardName">Card to remove via its name</param>
    /// <returns>Whether the removal was successful</returns>
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

    /// <summary>
    /// Remove a commander from the loadout
    /// </summary>
    /// <param name="outCommander">Commander to remove</param>
    /// <returns>Whether the removal was successful</returns>
    public bool RemoveCard(CommanderCardScript outCommander)
    {
        if (commander != outCommander.Name) return false;
        else commander = "";
        return true;
    }

    public int CardCountInDeck(NewVirtualCardParent cardToFind)
    {
        int count = 0;
        for (int i = 0; i < deck.Count; i++)
        {
            if (deck[i].CardName == cardToFind.CardName)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Change the cards to select by faction.
    /// </summary>
    /// <param name="faction">Faction of cards to select</param>
    private void ChangeFactionCards(string faction)
    {
        // remove existing cards
        while (cardObjects.Count != 0)
        {
            Destroy(cardObjects[0]);
            cardObjects.RemoveAt(0);
        }
        List<Details> factionCards = cardIndex.Index.GetAllFactionCards(faction);
        int index = 0;
        // Add and position all relevant cards
        for (int i = 0; i < factionCards.Count; i++)
        {
            // exclude tokens
            if (factionCards[i].type != NewVirtualCardParent.type.token)
            {
                GameObject deckCard = Instantiate(prefab);
                // funky math shifts the cards right and down
                deckCard.transform.position = new Vector3(-7.5f + ((cardSpacing * index) % (cardSpacing * cardsInRow)), 3 - (2 * (index / cardsInRow)), 0);
                if (deckCard.transform.position.y > highYPos) highYPos = deckCard.transform.position.y;
                if (deckCard.transform.position.y < lowYPos) lowYPos = deckCard.transform.position.y;
                deckCard.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);

                // setup for next part
                deckCard.GetComponent<DeckbuilderCard>().DeckInstance = this;
                cardObjects.Add(deckCard);

                // non-commander cards
                if (factionCards[i].type != NewVirtualCardParent.type.none)
                {
                    NewVirtualCardParent cardData = cardIndex.Index.CreateCard(factionCards[i].name, NewVirtualCardParent.location.deck);
                    deckCard.GetComponent<DeckbuilderCard>().CardInstance = cardData;
                }
                // commanders
                else
                {
                    cardIndex.Index.AttachCommanderCard(deckCard, factionCards[i].name, null);
                    deckCard.GetComponent<DeckbuilderCard>().UpdateUI();
                    deckCard.GetComponent<CommanderCardScript>().DeckbuilderOverride = true;
                }
                index++;
            }
        }
    }
}
