using UnityEngine;
using cardIndex;

public class DemoPlayerInstanceScript : MonoBehaviour
{
    //add/switch cards out of the deck here if you don't want to hardcode
    private string[] startingDeck =
    {
        /*"Cat",
        "Magic Cat / Septimus Mrreep",
        "M16",
        "Conscript",
        "Smite",*/
        //"Scaredy Cat",
        //"Comically Large Spoon Cat",
        //"Ratta-tat-Cat",
        //"Cat",
        //"Exploding Cat",
        //"Night Vision Cat",
    };

    private Player p;

    private void Start() //test other colors and remember to flip these back
    {
        p = GetComponent<Player>();
        
        if (p == null)
        {
            Debug.LogWarning("DemoPlayerInstanceScript needs a Player component.");
            return;
        }

        if (p.IsPlayerTwo)
        {
            p.Deck.Add(new MinionParent(4, 3, 4, "Dr House(Cat)", NewVirtualCardParent.type.minion,
    MinionParent.effect.heal, NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent(1, 3, 3, "AOETEst",
NewVirtualCardParent.type.minion, MinionParent.effect.aoe, NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent(6, 6, 9999, "Slasher Cat", NewVirtualCardParent.type.minion,
MinionParent.effect.deathtouch, NewVirtualCardParent.location.deck));
            p.Deck.Add(new TwoAttackParent(3, 2, MinionParent.effect.heal, 4, 4, 0, "Witch Cat",
NewVirtualCardParent.type.minion, MinionParent.effect.twoAttacks, NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent("Night Vision Cat", NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent("Comically Large Spoon Cat", NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent("Scaredy Cat", NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent(1, 1, 1, "Roughly A Cat",
NewVirtualCardParent.type.minion, MinionParent.effect.guard, NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent(5, 4, 6, "Vampire Cat",
NewVirtualCardParent.type.minion, MinionParent.effect.spawnToken, NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent("Cat", NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent(2, 3, 1, "Nacho Cat", NewVirtualCardParent.type.minion,
MinionParent.effect.thorns, NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent("Exploding Cat", NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent("Ratta-tat-Cat", NewVirtualCardParent.location.deck));
        }
        else
        {
            p.Deck.Add(new MinionParent("Ratta-tat-Cat", NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent("Exploding Cat", NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent(2, 3, 1, "Nacho Cat", NewVirtualCardParent.type.minion,
        MinionParent.effect.thorns, NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent("Cat", NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent(5, 4, 6, "Vampire Cat",
    NewVirtualCardParent.type.minion, MinionParent.effect.spawnToken, NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent(1, 1, 1, "Roughly A Cat",
            NewVirtualCardParent.type.minion, MinionParent.effect.guard, NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent("Scaredy Cat", NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent("Comically Large Spoon Cat", NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent("Night Vision Cat", NewVirtualCardParent.location.deck));
            p.Deck.Add(new TwoAttackParent(3, 2, MinionParent.effect.heal, 4, 4, 0, "Witch Cat",
        NewVirtualCardParent.type.minion, MinionParent.effect.twoAttacks, NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent(6, 6, 9999, "Slasher Cat", NewVirtualCardParent.type.minion,
        MinionParent.effect.deathtouch, NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent(1, 3, 3, "AOETEst",
    NewVirtualCardParent.type.minion, MinionParent.effect.aoe, NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent(4, 3, 4, "Dr House(Cat)", NewVirtualCardParent.type.minion,
    MinionParent.effect.heal, NewVirtualCardParent.location.deck));
        }
        /*p.Deck.Add(new SpellParent(SpellParent.spellEffect.unique, SpellParent.spellTarget.allyCards, 0, 0, 4, "No Thoughts, Head Empty",
NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));*/
        /*p.Deck.Add(new SpellParent(SpellParent.spellEffect.equipment, SpellParent.spellTarget.enemyCards, 1, 0, 4, "Hex",
NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));*/
        /*p.Deck.Add(new SpellParent("M16", NewVirtualCardParent.location.deck));
        p.Deck.Add(new SpellParent("Terrorize", NewVirtualCardParent.location.deck));*/
        for (int i = 0; i < startingDeck.Length; i++)   
        {
            p.Deck.Add(cardIndex.Index.CreateCard(startingDeck[i], NewVirtualCardParent.location.deck));
        }

        Debug.Log(gameObject.name + " deck loaded with " + p.Deck.Count + " cards.");
    }
}