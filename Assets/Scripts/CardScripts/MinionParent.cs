using cardIndex;
using UnityEngine;
using Network;

public class MinionParent : NewVirtualCardParent
{
    public enum effect
    {
        none,
        deathtouch, //just works off base damage for right now, probably want to change this
        explode,
        haste,
        sloth,
        coordinate
    }

    private int health;
    private int damage;
    private effect cardEffect;
    private bool isDead = false;
    [SerializeField] private bool canAttack = false;
    private CoordinateAbilityScript coordinateAbility;

    public int Health { get { return health; } set { health = value; } }
    public int Damage { get { return damage; } set { damage = value; } }
    public bool IsDead { get { return isDead; } }
    public effect CardEffect { get { return cardEffect; } }
    public bool CanAttack { get { return canAttack; } set { canAttack = value; } }
    public CoordinateAbilityScript CoordinateAbility { get { return coordinateAbility; } set { coordinateAbility = value; }  }

    public MinionParent(int cost, int health, int damage, string name, type cardType, effect cardEffect, location cardLocation) 
        : base(cost, name, cardType, cardLocation)
    {
        this.health = health;
        this.damage = damage;
        this.cardEffect = cardEffect;
        if(this.cardEffect == effect.coordinate) { CoordinateAbility = new CoordinateAbilityScript(this.CardName); }
    }

    /// <summary>
    /// Construct a card using only its name. It should be noted that this constructor will set any int value that's not defined as -1.
    /// </summary>
    /// <param name="name">Name of the card.</param>
    /// <param name="cardLocation">location of the card.</param>
    public MinionParent(string name, location cardLocation) : base(name, cardLocation)
    {
        Details cardDetails = cardIndex.Index.GetDetails(name);
        health = cardDetails.health;
        damage = cardDetails.damage;
        cardEffect = cardDetails.ability;
        if (this.cardEffect == effect.coordinate) { CoordinateAbility = new CoordinateAbilityScript(this.CardName); }
    }

    public override void OnPlay()
    {
        Debug.Log("a");
    }
    public void Attack(MinionParent target)
    {
        if (canAttack)
        {
            if (target == null || isDead)
            {
                return;
            }
            if (cardEffect == effect.deathtouch)
            {
                target.TakeDamage(this, 99999999);
            }
            else
            {
                target.TakeDamage(this, Damage);
            }
            canAttack = false;

            // send this attack if the card belongs to player 1.
            if (!UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.IsPlayerTwo)
            {
                Networking.SendCardAttack(this, target);
            }
        }
    }

    public void TakeDamage(MinionParent attacker, int damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
            if (cardEffect == effect.explode) { attacker.TakeDamage(this, Damage); }
            Death();
        }
    }

    public void Death()
    {
        isDead = true;
        CardLocation = location.discard;
    }
}
