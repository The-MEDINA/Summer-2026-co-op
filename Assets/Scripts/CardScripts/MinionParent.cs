using cardIndex;
using System.Collections.Generic;
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
        coordinate,
        twoAttacks,
        aoe,
        overkill,
        duplicate
    }

    public enum equipment
    {
        m16,
        iHungy,
        terrorize,
        fishTreat
    }

    private int health;
    private int damage;
    private effect cardEffect;
    private bool isDead = false;
    [SerializeField] private bool canAttack = false;
    private CoordinateAbilityScript coordinateAbility;
    private int startingHealth;
    private List<equipment> equipmentList;

    public int Health { get { return health; } set { health = value; } }
    public int Damage { get { return damage; } set { damage = value; } }
    public bool IsDead { get { return isDead; } }
    public effect CardEffect { get { return cardEffect; } }
    public bool CanAttack { get { return canAttack; } set { canAttack = value; } }
    public CoordinateAbilityScript CoordinateAbility { get { return coordinateAbility; } set { coordinateAbility = value; }  }
    public int StartingHealth { get { return startingHealth; } set { startingHealth = value; } }

    public MinionParent(int cost, int health, int damage, string name, type cardType, effect cardEffect, location cardLocation) 
        : base(cost, name, cardType, cardLocation)
    {
        this.health = health;
        this.startingHealth = health;
        this.damage = damage;
        this.cardEffect = cardEffect;
        if(this.cardEffect == effect.coordinate) { CoordinateAbility = new CoordinateAbilityScript(this.CardName); }
        equipmentList = new List<equipment>();
        if (CardType == NewVirtualCardParent.type.token) { CardLocation = NewVirtualCardParent.location.inPlay; }
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
        startingHealth = cardDetails.health;
        damage = cardDetails.damage;
        cardEffect = cardDetails.ability;
        equipmentList = new List<equipment>();
        if (this.cardEffect == effect.coordinate) { CoordinateAbility = new CoordinateAbilityScript(this.CardName); }
        if (CardType == NewVirtualCardParent.type.token) { CardLocation = NewVirtualCardParent.location.inPlay; }
    }

    public void OnPlay()
    {
        Debug.Log("a");
        if (CardEffect == effect.duplicate)
        {
            Debug.Log("b");
            UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.CommanderCard.BG.SpawnCardToInPlay(new MinionParent(0, Health, Damage, CardName, 
                NewVirtualCardParent.type.token, MinionParent.effect.none, NewVirtualCardParent.location.inPlay));
        }
    }

    public void Attack(MinionParent target)
    {
        if (canAttack)
        {
            Debug.Log(Damage);
            Debug.Log(target.Health);
            if (target == null || isDead || target.IsDead)
            {
                return;
            }
            else
            {
                target.TakeDamage(this, Damage);
            }
            canAttack = false;
        }
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

    public void TakeDamage(MinionParent attacker, int damage)
    {
        if(attacker.CardEffect == effect.deathtouch)
        {
            Health = 0;
        }

        Health -= damage;

        if (Health <= 0)
        {
            if (attacker.CardEffect == effect.overkill)
            {
                this.UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.TakeDamage(-1 * Health);
            }
            Health = 0;

            if (cardEffect == effect.explode) 
            {
                int explodeDamage = 0;
                switch(CardName)
                {
                    case "Mad Scientist Cat":
                        {
                            explodeDamage = 3;
                            break;
                        }

                    case "Exploding Cat":
                    default:
                        {
                            explodeDamage = 1;
                            break;
                        }
                }
                attacker.TakeDamage(this, explodeDamage); 
            }
            Death();
        }
    }

    public void Death()
    {
        isDead = true;
        if(CardType == NewVirtualCardParent.type.token) { return; }
        CardLocation = location.discard;
        if (UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.IsPlayerTwo)
        {
            Networking.SendCardDeath(UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.IsPlayerTwo, this);
        }
        UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.MoveCardToDiscard(this);
    }

    public void AOEAttack(List<NewVirtualCardParent> targetList, bool isSecond)
    {
        if(canAttack && (cardEffect == effect.aoe || isSecond))
        {
            if (targetList == null)
            {
                return;
            }

            for (int i = 0; i < targetList.Count; i++)
            {
                if (targetList[i] is MinionParent)
                {
                    MinionParent enemyTarget = (MinionParent)targetList[i];
                    enemyTarget.TakeDamage(Damage);
                }
            }
            canAttack = false;
        }
    }

    public void AddEquipment(equipment addToList)
    {
        equipmentList.Add(addToList);
    }
}
