using UnityEngine;

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
    private type cardType;
    private effect cardEffect;
    private location cardLocation;

    [SerializeField] private string cardName;
    private bool isDead = false;

    public int Cost { get { return cost; } }
    public int Health { get { return health; } set { health = value; } }
    public int Damage { get { return damage; } set { damage = value; } }
    public bool IsDead { get { return isDead; } }
    public string CardName { get { return cardName; } }
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

    public void Attack(CardParent target)
    {
        if (target == null || isDead)
        {
            return;
        }

        target.TakeDamage(Damage);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
            Death();
        }
    }

    public void Death()
    {
        isDead = true;
        cardLocation = location.discard;
    }
}