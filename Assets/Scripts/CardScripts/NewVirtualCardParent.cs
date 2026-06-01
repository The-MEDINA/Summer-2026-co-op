using UnityEngine;
using cardIndex;

public abstract class NewVirtualCardParent
{
    public enum type
    {
        minion,
        spell,
        token
    }

    public enum location
    {
        deck,
        hand,
        inPlay,
        discard
    }
    private int nameIndexPosition;
    private int cost;
    private type cardType;
    private location cardLocation;
    private GameObject unityObject;
    [SerializeField] private string cardName;
    private string flavorText;

    public int NameIndexPosition { get { return nameIndexPosition; } }
    public int Cost { get { return cost; } }
    public string CardName { get { return cardName; } }
    public string FlavorText { get { return flavorText; } }
    public location CardLocation { get { return cardLocation; } set { cardLocation = value; } }
    public type CardType { get { return cardType; } }
    public GameObject UnityObject { get { return unityObject; } set { unityObject = value; } }

    /// <summary>
    /// base constructor for all card scripts
    /// </summary>
    /// <param name="cost">energy cost to play the card</param>
    /// <param name="name">name of the card</param>
    /// <param name="cardType">type of card, spell/minion/etc</param>
    /// <param name="cardLocation">always starts in the deck</param>
    public NewVirtualCardParent(int cost, string name, type cardType, location cardLocation)
    {
        this.cost = cost;
        this.cardName = name;
        this.cardType = cardType;
        this.cardLocation = cardLocation;
        nameIndexPosition = cardIndex.Index.GetDetails(name).nameIndexPosition;
    }
    /// <summary>
    /// Construct a card using only its name. It should be noted that this constructor will set any int value that's not defined as -1.
    /// </summary>
    /// <param name="name">Name of the card.</param>
    /// <param name="cardLocation">location of the card.</param>
    public NewVirtualCardParent(string name, location cardLocation)
    {
        Details cardDetails = cardIndex.Index.GetDetails(name);
        cost = cardDetails.cost;
        cardName = cardDetails.name;
        flavorText = cardDetails.flavorText;
        nameIndexPosition = cardDetails.nameIndexPosition;
        this.cardLocation = cardLocation;
    }
}