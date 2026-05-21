using UnityEngine;
using cardIndex;

public abstract class NewVirtualCardParent
{
    public enum type
    {
        minion,
        spell
    }

    public enum location
    {
        deck,
        hand,
        inPlay,
        discard
    }

    private int cost;
    private type cardType;
    private location cardLocation;

    [SerializeField] private string cardName;
    private string flavorText;

    public int Cost { get { return cost; } }
    public string CardName { get { return cardName; } }
    public string FlavorText { get { return flavorText; } }
    public location CardLocation { get { return cardLocation; } set { cardLocation = value; } }
    public type CardType { get { return cardType; } }

    public NewVirtualCardParent(int cost, string name, type cardType, location cardLocation)
    {
        this.cost = cost;
        this.cardName = name;
        this.cardType = cardType;
        this.cardLocation = cardLocation;
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
        this.cardLocation = cardLocation;
    }
    public abstract void OnPlay();
}