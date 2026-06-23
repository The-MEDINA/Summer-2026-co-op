using UnityEngine;
using cardIndex;
using System.Collections.Generic;

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
        allEnemies,
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

    /// <summary>
    /// creates a new Spell card using its name and cardIndex
    /// </summary>
    /// <param name="name">the name of the spell</param>
    /// <param name="cardLocation">always deck</param>
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
        switch (effect)
        {
            case spellEffect.spawnTokens:
                {
                    Debug.Log("e");

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

                        case "Curse":
                        case "Terrorize":
                            {
                                if(target is TwoAttackParent)
                                {
                                    TwoAttackParent twoAttackTarget = (TwoAttackParent)target;
                                    twoAttackTarget.FirstDamage -= amount;
                                    twoAttackTarget.SecondDamage -= amount;
                                    if(twoAttackTarget.FirstDamage < 1) { twoAttackTarget.FirstDamage = 1; }
                                    if(twoAttackTarget.SecondDamage < 1) { twoAttackTarget.SecondDamage = 1; }
                                }
                                else
                                {
                                    target.Damage -= amount;
                                    if(target.Damage < 0) { target.Damage = 0; }
                                }

                                target.StartingHealth -= secondEquipmentAmount;
                                target.Health -= secondEquipmentAmount;
                                if (target.StartingHealth < 1) { target.StartingHealth = 1; }
                                if (target.Health < 1) { target.Health = 1; }

                                if(CardName == "Curse") { target.AddEquipment(MinionParent.equipment.curse); }
                                if(CardName == "Terrorize") { target.AddEquipment(MinionParent.equipment.terrorize); }
                                break;
                            }

                        case "Empower":
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

                                if(CardName == "Empower") { target.AddEquipment(MinionParent.equipment.empower); }
                                if (CardName == "Fish Treat") { target.AddEquipment(MinionParent.equipment.fishTreat); }
                                break;
                            }

                        case "Catnap":
                            {
                                target.StartingHealth += secondEquipmentAmount;
                                target.Health += secondEquipmentAmount;
                                target.AddEquipment(MinionParent.equipment.catnap);

                                break;
                            }

                        case "Hex":
                            {
                                if (target is TwoAttackParent)
                                {
                                    TwoAttackParent twoAttackTarget = (TwoAttackParent)target;
                                    twoAttackTarget.FirstDamage = 1;
                                    twoAttackTarget.SecondDamage = 1;
                                }
                                else
                                {
                                    target.Damage = 1;
                                }
                                target.AddEquipment(MinionParent.equipment.hex);
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
                    switch(CardName)
                    {
                        case "Clone":
                            {
                                UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.CommanderCard.BG.SpawnCardToInPlay(new MinionParent(0, 
                                target.Health, target.Damage, target.CardName, NewVirtualCardParent.type.token, target.CardEffect, 
                                NewVirtualCardParent.location.inPlay));
                                break;
                            }

                        case "No Thoughts, Head Empty":
                            {
                                RevertEquipment(target); //not implemented
                                break;
                            }
                    }
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

    public void OnPlayAOE(List<NewVirtualCardParent> cards)
    {
        switch(CardName)
        {
            case "Blizzard":
                {
                    for (int i = 0; i < cards.Count; i++)
                    {
                        cards[i].UnityObject.GetComponent<CardClickHandler>().SetSpeed(CardClickHandler.speed.frozen);
                        cards[i].UnityObject.GetComponent<CardClickHandler>().ResetTimer();
                        cards[i].UnityObject.GetComponent<CardUIManager>().AddProgress(5f);
                    }
                    break;
                }
        }
    }

    private void RevertEquipment(MinionParent target)//does not work for twoattackparents, and test more plz
    {
        if (target.EquipmentList.Count <= 0) { return; }

        for (int i = 0; i < target.EquipmentList.Count; i++)
        {
            switch(target.EquipmentList[i])//errors occur when health taken below 1 and then tried to revert it
            {
                case MinionParent.equipment.m16: { target.Damage -= 2; break; }
                case MinionParent.equipment.terrorize: { target.Damage++; target.Health++; target.StartingHealth++; break; }
                case MinionParent.equipment.fishTreat: { target.Damage -= 2; target.Health -= 2; target.StartingHealth -= 2; break; }
                case MinionParent.equipment.empower: { target.Damage--; target.Health--; target.StartingHealth--; break; }
                case MinionParent.equipment.curse: { target.Damage += 2; break; }
                case MinionParent.equipment.catnap: { target.Health -= 3; target.StartingHealth -= 3; break; }
            }
        }
    }
}
