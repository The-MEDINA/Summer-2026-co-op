using System.Collections.Generic;
using cardIndex;
using UnityEngine;

public class TwoAttackParent : MinionParent
{
    [SerializeField] private int firstDamage; //damage for first attack
    [SerializeField] private int secondDamage; //damage for second attack
    private effect secondAttackEffect;
    private int startingSecondDamage;

    public int FirstDamage { get { return firstDamage; } set { firstDamage = value; } }
    public int SecondDamage { get { return secondDamage; } set { secondDamage = value; } }
    public int StartingSecondDamage { get { return startingSecondDamage; } set { startingSecondDamage = value; } }
    public effect SecondaryCardEffect { get { return secondAttackEffect; } }

    /// <summary>
    /// creates a minion with the ability to use two seperate attacks
    /// </summary>
    /// <param name="firstD">damage for first attack</param>
    /// <param name="secondD">damage for second attack</param>
    /// <param name="secondAttackE">effect, if it exists, for the second attack. First attack will likely never have an ability</param>
    /// <param name="cost">energy cost to play card</param>
    /// <param name="health">current health of the card</param>
    /// <param name="damage">damage the card deals on attack</param>
    /// <param name="name">the card's name</param>
    /// <param name="cardType">what type of card, minion/spell/etc</param>
    /// <param name="cardEffect">It's twoAttacks</param>
    /// <param name="cardLocation">deck</param>
    public TwoAttackParent(int firstD, int secondD, effect secondAttackE, int cost, int health, int damage, string name, 
        type cardType, effect cardEffect, location cardLocation) : base(cost, health, damage, name, cardType, cardEffect, cardLocation)
    {
        FirstDamage = firstD;
        SecondDamage = secondD;
        startingSecondDamage = secondD;
        secondAttackEffect = secondAttackE;
    }

    /// <summary>
    /// Construct a card using only its name. It should be noted that this constructor will set any int value that's not defined as -1.
    /// </summary>
    /// <param name="name">Name of the card.</param>
    /// <param name="cardLocation">location of the card.</param>
    public TwoAttackParent(string name, location cardLocation) : base(name, cardLocation)
    {
        Details cardDetails = cardIndex.Index.GetDetails(name);
        FirstDamage = cardDetails.damage;
        SecondDamage = cardDetails.secondDamage;
        startingSecondDamage = cardDetails.secondDamage;
        secondAttackEffect = cardDetails.secondAbility;
    }

    /// <summary>
    /// checks which attack is being used and calls Attack() with the correct damage
    /// </summary>
    /// <param name="numAttack">1st or 2nd attack</param>
    /// <param name="target">target of the attack</param>
    public void CheckAttack(int numAttack, MinionParent target)
    {
        if(numAttack == 1)
        {
            Damage = firstDamage;
            Attack(target);
        }
        else
        {
            Damage = secondDamage;

            if(secondAttackEffect == effect.heal)
            {
                CardEffect = effect.heal;
                Attack(target);
                CardEffect = effect.twoAttacks;
            }
            else if (secondAttackEffect == effect.apoptosis)
            {
                Damage = firstDamage;
                Attack(target);
                UseApoptosis();
            }
            else if (secondAttackEffect == effect.gainEnergy)
            {
                UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.Energy += secondDamage;
            }
        }
    }

    /// <summary>
    /// CheckAttack() but adapted for if the second attack happens to be an AOE. Different method because it requires a new parameter
    /// </summary>
    /// <param name="numAttack">1st or 2nd attack</param>
    /// <param name="target">target of the attack NOT REMOVING BECAUSE OF ADJACENT ATTACK POSSIBILITY</param>
    /// <param name="targetList">the entire InPlay of the opponent</param>
    public void CheckAOEAttack(int numAttack, MinionParent target, List<NewVirtualCardParent> targetList)
    {
        if (secondAttackEffect == effect.aoe)
        {
            if (numAttack == 1)
            {
                Damage = firstDamage;
                Attack(target);
            }
            else
            {
                Damage = secondDamage;
                Debug.Log(Damage);
                AOEAttack(targetList, true);
            }
        }
    }

    private void UseApoptosis()//needs to have interaction potentially changed
    {
        UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.Health += secondDamage;
        this.Death();
    }
}
