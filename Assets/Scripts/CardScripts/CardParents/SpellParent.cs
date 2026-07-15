using cardIndex;
using System.Collections.Generic;
using UnityEngine;

public class SpellParent : NewVirtualCardParent
{
    public enum spellEffect //what the spell does
    {
        damage,
        heal,
        unique,
        equipment,
        spawnTokens,
        copy
    }

    public enum spellTarget //what the target type of the spell is
    {
        enemyCards,
        allyCards,
        opponent,
        owner,
        allEnemies,
        allAllies,
        none,
        any,
        inplay
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
                    for (int i = 0; i < amount; i++)
                    {
                        UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.CommanderCard.BG.SpawnCardToInPlay(
                            cardIndex.Index.CreateCard("Kitten", location.inPlay));
                    }
                    break;
                }
            case spellEffect.heal:
                {
                    UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.Health += Amount;
                    Debug.Log($"Healed player for {Amount} health. Current health: {UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.Health}");
                    break;
                }

            default:
                {
                    switch (CardName)
                    {
                        case "Barbed Wire":
                            {
                                UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.PlayerGainThorns();
                                break;
                            }

                        case "Cat Fusion":
                            {
                                int total = 1;
                                for (int i = UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.InPlay.Count - 1; i >= 0; i--)
                                {
                                    MinionParent target = (MinionParent)UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.InPlay[i];
                                    if (target.CardName == "Kitten")
                                    {
                                        total++;
                                        target.Death();
                                    }
                                    else if (target.CardName == "Grey")
                                    {
                                        total += 2;
                                        target.Death();
                                    }
                                }
                                UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.CommanderCard.BG.SpawnCardToInPlay(new MinionParent(
                                    0, total, total, "Tiger the Cat", type.token, MinionParent.effect.none, location.inPlay));
                                break;
                            }
                    }
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
                                    if (target.HasStatsUp)
                                    {
                                        twoAttackTarget.FirstDamage += amount;
                                        twoAttackTarget.SecondDamage += amount;
                                    }
                                }
                                else
                                {
                                    target.Damage += amount;
                                    if (target.HasStatsUp)
                                    {
                                        target.Damage += amount;
                                    }
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
                                    if (target.HasStatsUp)
                                    {
                                        twoAttackTarget.FirstDamage -= amount;
                                        twoAttackTarget.SecondDamage -= amount;
                                    }

                                    if (twoAttackTarget.FirstDamage < 1) { twoAttackTarget.FirstDamage = 1; }
                                    if(twoAttackTarget.SecondDamage < 1) { twoAttackTarget.SecondDamage = 1; }
                                }
                                else
                                {
                                    target.Damage -= amount;
                                    if (target.HasStatsUp)
                                    {
                                        target.Damage -= amount;
                                    }

                                    if (target.Damage < 0) { target.Damage = 0; }
                                }

                                target.StartingHealth -= secondEquipmentAmount;
                                target.Health -= secondEquipmentAmount;
                                if (target.HasStatsUp)
                                {
                                    target.StartingHealth -= secondEquipmentAmount;
                                    target.Health -= secondEquipmentAmount;
                                }

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
                                    if (target.HasStatsUp)
                                    {
                                        twoAttackTarget.FirstDamage += amount;
                                        twoAttackTarget.SecondDamage += amount;
                                    }
                                }
                                else
                                {
                                    target.Damage += amount;
                                    if (target.HasStatsUp)
                                    {
                                        target.Damage += amount;
                                    }
                                }

                                target.StartingHealth += secondEquipmentAmount;
                                target.Health += secondEquipmentAmount;
                                if (target.HasStatsUp)
                                {
                                    target.StartingHealth += secondEquipmentAmount;
                                    target.Health += secondEquipmentAmount;
                                }

                                if (CardName == "Empower") { target.AddEquipment(MinionParent.equipment.empower); }
                                if (CardName == "Fish Treat") { target.AddEquipment(MinionParent.equipment.fishTreat); }
                                break;
                            }

                        case "Catnap":
                            {
                                target.StartingHealth += secondEquipmentAmount;
                                target.Health += secondEquipmentAmount;
                                if (target.HasStatsUp)
                                {
                                    target.StartingHealth += secondEquipmentAmount;
                                    target.Health += secondEquipmentAmount;
                                }
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
                        case "Distraction":
                            {
                                target.HasGuard = true;
                                target.AddEquipment(MinionParent.equipment.distraction);
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
                                UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.CommanderCard.BG.SpawnCardToInPlay(cardIndex.Index.CreateCard(target.CardName, location.inPlay));
                                CardSelectionManager.Instance.RepositionInPlayCards(UnityObject.GetComponent<CardClickHandler>().OwnerPlayer);
                                break;
                            }

                        case "No Thoughts, Head Empty":
                            {
                                Details cardDetails = cardIndex.Index.GetDetails(target.CardName);
                                target.Damage = cardDetails.damage;
                                target.Health = cardDetails.health;
                                if (target is TwoAttackParent)
                                {
                                    TwoAttackParent twoAttackTarget = (TwoAttackParent)target;
                                    twoAttackTarget.FirstDamage = cardDetails.damage;
                                    twoAttackTarget.SecondDamage = cardDetails.secondDamage;
                                }
                                break;
                            }

                        case "2 Cats in a Trenchcoat":
                            {
                                if (target.UnityObject.GetComponent<CardClickHandler>().CurrentSpeed == CardClickHandler.speed.frozen)
                                {
                                    target.UnityObject.GetComponent<CardClickHandler>().FindSpeed(target.CardEffect);
                                    break;
                                }
                                target.UnityObject.GetComponent<CardClickHandler>().SetSpeedToFull();
                                break;
                            }

                        case "Decipher":
                            {
                                target.IsHidden = false;
                                break;
                            }

                        case "Hide":
                            {
                                target.IsHidden = true;
                                break;
                            }

                        case "I'm Sure That Wasn't Important":
                            {
                                target.UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.Deck.Insert(0, new MinionParent(target.CardName, location.deck));
                                target.UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.CommanderCard.BG.DrawCardToHand();
                                target.TakeDamage(9999);
                                break;
                            }
                    }
                    break;
                }
        }
    }

    public void OnPlayAny(NewVirtualCardParent target)
    {
        switch(effect)
        {
            case spellEffect.copy:
                {//does not function
                    UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.Deck.Insert(0, cardIndex.Index.CreateCard(target.CardName, location.deck));
                    UnityObject.GetComponent<CardClickHandler>().OwnerPlayer.CommanderCard.BG.DrawCardToHand();
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

            case "Undeniable Proof":
                {
                    for (int i = 0; i < cards.Count; i++)
                    {
                        if (cards[i] is MinionParent)
                        {
                            MinionParent target = (MinionParent)cards[i];
                            target.IsHidden = false;
                        }
                    }
                    break;
                }

            case "Cover-Up":
                {
                    for (int i = 0; i < cards.Count; i++)
                    {
                        if (cards[i] is MinionParent)
                        {
                            MinionParent target = (MinionParent)cards[i];
                            target.IsHidden = true;
                        }
                    }
                    break;
                }

            default:
                {
                    switch(Effect)
                    {
                        case spellEffect.damage:
                            {
                                for (int i = cards.Count - 1; i >= 0; i--)
                                {
                                    if (cards[i] is MinionParent)
                                    {
                                        MinionParent target = (MinionParent)cards[i];
                                        target.TakeDamage(amount);
                                    }
                                 }
                                    break;
                            }
                    }
                    break;
                }
        }
    }
}
