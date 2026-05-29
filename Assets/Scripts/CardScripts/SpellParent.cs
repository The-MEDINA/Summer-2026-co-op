using UnityEngine;
using cardIndex;

public class SpellParent : NewVirtualCardParent
{
    public enum spellEffect //what the spell does
    {
        damage,
        heal,
        unique
    }

    public enum spellTarget //what the target type of the spell is
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
    public int Amount { get { return amount; } }//amount of damage done, health healed, etc

    /// <summary>
    /// creates a new Spell card object
    /// </summary>
    /// <param name="thisSpellEffect">the effect of the spell</param>
    /// <param name="thisSpellTarget">who can be targeted by the spell</param>
    /// <param name="amount">amount of damage done, health healed, etc</param>
    /// <param name="cost">how much energy it costs to play this spell</param>
    /// <param name="name">the spell's name</param>
    /// <param name="cardType">always spell</param>
    /// <param name="cardLocation">always deck</param>
    public SpellParent(spellEffect thisSpellEffect, spellTarget thisSpellTarget, int amount, 
        int cost, string name, type cardType, location cardLocation) : base(cost, name, cardType, cardLocation)
    {
        effect = thisSpellEffect;
        target = thisSpellTarget;
        this.amount = amount;
    }

    public SpellParent(string name, location cardLocation) : base(name, cardLocation)
    {
        Details spellDetails = cardIndex.Index.GetDetails(name);
        effect = spellDetails.spellEffect;
        target = spellDetails.spellTarget;
        // so... I see this amount variable and I don't exactly know where it's being used right now.
        // Since the only thing I see that has an amount in the spreadsheet has it in damage, I'm gonna pull it from there for now.
        // Let me know if I should change this. - Dave
        amount = spellDetails.damage;
    }

    /// <summary>
    /// spell usage when the target is a minion
    /// </summary>
    /// <param name="target">the minion being targeted</param>
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
                    if (target.Health > target.StartingHealth)
                    {
                        target.Health = target.StartingHealth;
                    }
                    break;
                }

            case spellEffect.unique:
            default:
                {
                    break;
                }
        }
    }

    /// <summary>
    /// spell usage when the target is a player
    /// </summary>
    /// <param name="target">the player being targeted</param>
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
