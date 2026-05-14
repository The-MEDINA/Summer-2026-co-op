using UnityEngine;

// should be abstract no? - Dave
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
        overkill,
        explode
    }

    public enum location
    {
        deck,
        hand,
        inPlay,
        discard
    }

    private int cost; //shouldn't be private - property or public?
    private int health;
    private int damage;
    private float timer;
    private type cardType;
    private effect cardEffect;
    private location cardLocation;

    [SerializeField] private string cardName;
    private bool isDead = false;

    // public int Cost { get { return cost; } } should exist imo - Dave
    public int Health { get { return health; } set { health = value;  } }
    public int Damage { get { return damage; } set { damage = value; } }
    public bool IsDead { get { return isDead; } }

    public CardParent(int cost, int health, int damage, type cardType, effect cardEffect, location cardLocation)
    {
        this.cost = cost;
        this.health = health;
        this.damage = damage;

        this.cardType = cardType;
        this.cardEffect = cardEffect;
        this.cardLocation = cardLocation;
    }  

    public string CardName
    {
        get { return cardName; }
    }  

    //MIGHT BE WORTH TO HAVE A METHOD WITH A SWITCH LEADING INTO EVENT METHODS
    //for example, click once on card, trigger method, next click on an opponent's card, that triggers Attack() with second card as the target

    //triggered by event
    //OnPlay()
    
    // OnPlay should change a state in the player to a state where if they click on an opponent's card, *then* it calls attack and resets player state imo - Dave

    //triggered by event
    public void Attack(CardParent target)
    {
        target.TakeDamage(Damage);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0) { Death(); }
    }

    public void Death()
    {
        isDead = true;
    }

    //triggered by event
    //OnDisplay()
}
