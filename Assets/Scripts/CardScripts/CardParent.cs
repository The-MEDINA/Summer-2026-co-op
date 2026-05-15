using UnityEngine;

// should be abstract no? - Dave

//will be soon. For now we have no specific cards so we're using instances of CardParent to do demo code. Considering all the methods have to work with all cards
//it makes more sense to focus on the code for now. Changing this to abstract won't be hard - Jake
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

    private int cost;
    private int health;
    private int damage;
    private float timer;
    private type cardType;
    private effect cardEffect;
    private location cardLocation;

    [SerializeField] private string cardName;
    private bool isDead = false;

    public int Cost { get { return cost; } }
    public int Health { get { return health; } set { health = value;  } }
    public int Damage { get { return damage; } set { damage = value; } }
    public bool IsDead { get { return isDead; } }
    public string CardName { get { return cardName; } }
    public location CardLocation { get { return cardLocation; } set { cardLocation = value; } }

    public CardParent(int cost, int health, int damage, type cardType, effect cardEffect, location cardLocation)
    {
        this.cost = cost;
        this.health = health;
        this.damage = damage;

        this.cardType = cardType;
        this.cardEffect = cardEffect;
        this.cardLocation = cardLocation;
    }

    //triggered by event COULD ALSO BE HANDLED IN CARD CLICK/MANAGER
    //OnPlay()

    // OnPlay should change a state in the player to a state where if they click on an opponent's card, *then* it calls attack and resets player state imo - Dave

    //OnPlay is for when cards are moved from hand to battleground. I'm not sure what you mean with the state changes - Jake

    //triggered by event BUT HANDLED IN CARD CLICK CALLED IN MANAGER
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

    //triggered by event COULD ALSO BE HANDLED IN CARD CLICK/MANAGER
    //OnDisplay()
}
