using UnityEngine;
using cardIndex;

public class SpellParent : NewVirtualCardParent
{
    public enum spellEffect //what the spell does
    {
        damage,
        heal,
        unique,
        equipment,
        spawnTokens
    }

    public enum spellTarget //what the target type of the spell is
    {
        enemyCards,
        allyCards,
        opponent,
        owner,
        none
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
    /// spell usage when there is no target
    /// </summary>
    public void OnPlay()
    {
        switch(effect)
        {
            case spellEffect.spawnTokens:
                {
                    for (int i = 0; i < amount; i++)
                    {
                        UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.CommanderCard.BG.SpawnCardToInPlay(
                            cardIndex.Index.CreateCard("Kitten", location.inPlay));
                    }
                    break;
                }

            default:
                {
                    break;
                }
        }
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
                            { 
                                if (target is TwoAttackParent)
                                {
                                    TwoAttackParent twoAttackTarget = (TwoAttackParent)target;
                                    twoAttackTarget.FirstDamage += amount;
                                    twoAttackTarget.SecondDamage += amount;
                                }
                                else
                                {
                                    target.Damage += amount;
                                }

                                target.AddEquipment(MinionParent.equipment.m16);
                                break;
                            }

                        case "I Hungy!!!":
                            {
                                if(target.CardEffect == MinionParent.effect.coordinate)
                                {
                                    target.CoordinateAbility.NumToHit--;
                                }
                                target.AddEquipment(MinionParent.equipment.iHungy);
                                break;
                            }

                        case "Terrorize":
                            {
                                if(target is TwoAttackParent)
                                {
                                    TwoAttackParent twoAttackTarget = (TwoAttackParent)target;
                                    twoAttackTarget.FirstDamage -= amount;
                                    twoAttackTarget.SecondDamage -= amount;
                                    if(twoAttackTarget.FirstDamage < 0) { twoAttackTarget.FirstDamage = 0; }
                                    if(twoAttackTarget.SecondDamage < 0) { twoAttackTarget.SecondDamage = 0; }
                                }
                                else
                                {
                                    target.Damage -= amount;
                                    if(target.Damage < 0) { target.Damage = 0; }
                                }

                                target.StartingHealth -= secondEquipmentAmount;
                                target.Health -= secondEquipmentAmount;
                                target.AddEquipment(MinionParent.equipment.terrorize);
                                break;
                            }

                        case "Fish Treat":
                            {
                                if (target is TwoAttackParent)
                                {
                                    TwoAttackParent twoAttackTarget = (TwoAttackParent)target;
                                    twoAttackTarget.FirstDamage += amount;
                                    twoAttackTarget.SecondDamage += amount;
                                }
                                else
                                {
                                    target.Damage += amount;
                                }
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
