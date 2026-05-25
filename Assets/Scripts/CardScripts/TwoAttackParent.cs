using UnityEngine;

public class TwoAttackParent : MinionParent
{
    [SerializeField] private int firstDamage;
    [SerializeField] private int secondDamage;
    private effect secondAttackEffect;

    public int FirstDamage { get { return firstDamage; } set { firstDamage = value; } }
    public int SecondDamage { get { return secondDamage; } set { secondDamage = value; } }

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
            target.TakeDamage(this, FirstDamage);
        }
        else
        {
            if(secondAttackEffect == effect.aoe)
            {
                //implement aoe here
            }
            else
            {
                target.TakeDamage(this, SecondDamage);
            }
        }
    }
}
