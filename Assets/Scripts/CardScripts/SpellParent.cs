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
        //switch between targets and abilities
    }
}
