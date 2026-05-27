using System.Collections.Generic;
using UnityEngine;

public class TwoAttackParent : MinionParent
{
    [SerializeField] private int firstDamage;
    [SerializeField] private int secondDamage;
    private effect secondAttackEffect;

    public int FirstDamage { get { return firstDamage; } set { firstDamage = value; } }
    public int SecondDamage { get { return secondDamage; } set { secondDamage = value; } }
    public effect SecondaryCardEffect { get { return secondAttackEffect; } }

    public TwoAttackParent(int firstD, int secondD, effect secondAttackE, int cost, int health, int damage, string name, 
        type cardType, effect cardEffect, location cardLocation) : base(cost, health, damage, name, cardType, cardEffect, cardLocation)
    {
        FirstDamage = firstD;
        SecondDamage = secondD;
        secondAttackEffect = secondAttackE;
    }

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
                Attack(target);
        }
    }

    public void CheckAOEAttack(int numAttack, MinionParent target, List<NewVirtualCardParent> targetList)
    {
        {
            if(secondAttackEffect == effect.aoe)
            {
                if (numAttack == 1)
                {
                    Damage = firstDamage;
                    Attack(target);
                }
                else
                {
                    Damage = secondDamage;
                    AOEAttack(targetList);
                }
            }
        }
    }
}
