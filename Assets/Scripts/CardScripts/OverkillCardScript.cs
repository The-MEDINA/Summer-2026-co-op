using UnityEngine;

public class OverKillCardScript : CardParent
{
    private int onKillDamage;

    public int OnKillDamage { get { return onKillDamage; } set { onKillDamage = value; } } 

    public OverKillCardScript(int onKillDamage, int cost, int health, int damage, type cardType, effect cardEffect, location cardLocation) 
        : base(cost, health, damage, cardType, cardEffect, cardLocation)
    {
        OnKillDamage = onKillDamage;
    }

    /*
     * probably all methods in CardParent will have to be made virtual, Death() will for this subclass if we use it
     * public override void Death()
     * {
     * somehow try and figure out how to damage the attacker using OnKillDamage
     * actually maybe this should be done in TakeDamage()...
     * it could also be done entirely in CardParent using an enum check because of how simple this is
     * however this would most likely not be the case for more complicated card types
     * but we also might not make many super complicated card types...
     * but if we do we probably want all the files to be uniform, maybe not if they are really all that different
     * base Death() somewhere in here...
     * }
    */
}
