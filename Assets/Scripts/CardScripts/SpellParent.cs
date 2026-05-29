using UnityEngine;
using cardIndex;

public class SpellParent : NewVirtualCardParent
{
    public enum spellEffect //what the spell does
    {
        damage,
        heal,
        unique,
        equipment
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
    private int secondEquipmentAmount;

    public spellEffect Effect { get { return effect; } }
    public spellTarget Target { get { return target; } }
    public int Amount { get { return amount; } }//amount of damage done, health healed, etc
    public int SecondEquipmentAmountAmount { get { return secondEquipmentAmount; } }//startingHealth change in an equipment card

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
    public SpellParent(spellEffect thisSpellEffect, spellTarget thisSpellTarget, int amount, int secondAmount,
        int cost, string name, type cardType, location cardLocation) : base(cost, name, cardType, cardLocation)
    {
        effect = thisSpellEffect;
        target = thisSpellTarget;
        this.amount = amount;
        this.secondEquipmentAmount = secondAmount;
    }

    public SpellParent(string name, location cardLocation) : base(name, cardLocation)
    {
        Details spellDetails = cardIndex.Index.GetDetails(name);
        effect = spellDetails.spellEffect;
        target = spellDetails.spellTarget;
        amount = spellDetails.damage;
        secondEquipmentAmount = spellDetails.health;
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

            case spellEffect.equipment:
                {   
                    switch(CardName)
                    {
                        case "M16":
                            target.Damage += amount;
                            {
                                target.AddEquipment(MinionParent.equipment.m16);
                                break;
                            }

                        case "I Hungy!!!":
                            {
                                
                                target.AddEquipment(MinionParent.equipment.iHungy);
                                break;
                            }

                        case "Terrorize":
                            {
                                target.Damage -= amount;
                                target.StartingHealth -= secondEquipmentAmount;
                                target.Health -= secondEquipmentAmount;
                                target.AddEquipment(MinionParent.equipment.terrorize);
                                break;
                            }

                        case "Fish Treat":
                            {
                                target.Damage += amount;
                                target.StartingHealth += secondEquipmentAmount;
                                target.Health += secondEquipmentAmount;
                                target.AddEquipment(MinionParent.equipment.fishTreat);
                                break;
                            }

                        default:
                            {
                                break;
                            }
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
