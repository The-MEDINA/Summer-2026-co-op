using UnityEngine;

public class CardParent : MonoBehaviour
{
    public enum type
    {
        minion,
        spell
    }

    public enum effect
    {
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

    public int Health { get { return health; } set { health = value;  } }
    public int Damage { get { return damage; } set { damage = value; } }

    public CardParent(int cost, int health, int damage, type cardType, effect cardEffect, location cardLocation)
    {
        this.cost = cost;
        this.health = health;
        this.damage = damage;

        this.cardType = cardType;
        this.cardEffect = cardEffect;
        this.cardLocation = cardLocation;
    }    

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    //OnPlay()
    
    //Attack()

    //TakeDamage()

    //Death()

    //OnDisplay()
}
