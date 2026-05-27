using UnityEngine;
using cardIndex;

public class SpellParent : NewVirtualCardParent
{
    public enum spellEffect
    {
        damage,
        heal,
        unique
    }

    public enum spellTarget
    {
        enemyCards,
        allyCards,
        opponent,
        owner
    }

    private spellEffect effect;
    private spellTarget target;
    private int amount;

    public spellEffect Effect { get { return effect; } }
    public spellTarget Target { get { return target; } }
    public int Amount { get { return amount; } }

    public SpellParent(spellEffect thisSpellEffect, spellTarget thisSpellTarget, int amount, 
        int cost, string name, type cardType, location cardLocation) : base(cost, name, cardType, cardLocation)
    {
        effect = thisSpellEffect;
        target = thisSpellTarget;
        this.amount = amount;
    }

    //index constructor

    public override void OnPlay()
    {
        //currently unused, might take OnPlay out of NewVirtualCardParent
    }

    //not sure an enemy check is needed at all
    //if it is it could be pulled out but that might not be neater id- Jake
    public void OnPlay(MinionParent target)
    {
        switch (effect)
        {
            case spellEffect.damage:
                {
                    target.TakeDamage(amount);
                    break;
                }

            case spellEffect.heal:
                {
                    target.Health += amount;
                    break;
                }

            case spellEffect.unique:
            default:
                {
                    break;
                }
        }
    }

    public void OnPlay(Player target)
    {
        switch (effect)
        {
            case spellEffect.damage:
                {
                    target.TakeDamage(amount);
                    break;
                }

            case spellEffect.heal:
                {
                    target.Health += amount;
                    break;
                }

            case spellEffect.unique:
            default:
                {
                    break;
                }
        }
    }
}
