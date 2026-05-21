using System.IO;
using UnityEngine;
using cardIndex;
public class CardParent
{
    public enum type
    {
        minion,
        spell
    }

    public enum effect
    {
        none,
        deathtouch, //just works off base damage for right now, probably want to change this
        explode
    }

    public enum location
    {
        deck,
        hand,
        inPlay,
        discard
    }

    private int cost;
    private int health;
    private int damage;
    private type cardType;
    private effect cardEffect;
    private location cardLocation;

    [SerializeField] private string cardName;
    private string flavorText;
    private bool isDead = false;

    public int Cost { get { return cost; } }
    public int Health { get { return health; } set { health = value; } }
    public int Damage { get { return damage; } set { damage = value; } }
    public bool IsDead { get { return isDead; } }
    public string CardName { get { return cardName; } }
    public string FlavorText { get { return flavorText; } }
    public location CardLocation { get { return cardLocation; } set { cardLocation = value; } }
    public type CardType { get { return cardType; } }
    public effect CardEffect { get { return cardEffect; } }

    public CardParent(int cost, int health, int damage, type cardType, effect cardEffect, location cardLocation)
    {
        this.cost = cost;
        this.health = health;
        this.damage = damage;
        this.cardType = cardType;
        this.cardEffect = cardEffect;
        this.cardLocation = cardLocation;
    }

        public CardParent(int cost, int health, int damage, string name, type cardType, effect cardEffect, location cardLocation)
    {
        this.cost = cost;
        this.health = health;
        this.damage = damage;
        this.cardName = name;
        this.cardType = cardType;
        this.cardEffect = cardEffect;
        this.cardLocation = cardLocation;
    }
    /// <summary>
    /// Construct a card using only its name. It should be noted that this constructor will set any int value that's not defined as -1.
    /// </summary>
    /// <param name="name">Name of the card.</param>
    /// <param name="cardLocation">location of the card.</param>
    public CardParent(string name, location cardLocation)
    {
        Details cardDetails = cardIndex.Index.GetDetails(name);
        cost = cardDetails.cost;
        health = cardDetails.health;
        damage = cardDetails.damage;
        cardName = cardDetails.name;
        cardType = cardDetails.type;
        cardEffect = cardDetails.ability;
        flavorText = cardDetails.flavorText;
        this.cardLocation = cardLocation;
    }

    //triggered by event COULD ALSO BE HANDLED IN CARD CLICK/MANAGER
    public void OnPlay()
    {
        Debug.Log("a");
    }

    // OnPlay should change a state in the player to a state where if they click on an opponent's card, *then* it calls attack and resets player state imo - Dave

    //OnPlay is for when cards are moved from hand to battleground. I'm not sure what you mean with the state changes - Jake

    //triggered by event BUT HANDLED IN CARD CLICK CALLED IN MANAGER
    public void Attack(CardParent target)
    {
        if (target == null || isDead)
        {
            return;
        }
        if(cardEffect == effect.deathtouch)
        {
            target.TakeDamage(this, 99999999);
        }
        else
        {
            target.TakeDamage(this, Damage);
        }
    }

    public void TakeDamage(CardParent attacker, int damage)
    {
        Health -= damage;

        if (Health <= 0) 
        {
            Health = 0;
            if(cardEffect == effect.explode) { attacker.TakeDamage(this, Damage); }
            Death(); 
        }
    }

    public void Death()
    {
        isDead = true;
        cardLocation = location.discard;
    }

    //triggered by event COULD ALSO BE HANDLED IN CARD CLICK/MANAGER
    //if its handled there it might not need to be there
    //OnDisplay()
}
