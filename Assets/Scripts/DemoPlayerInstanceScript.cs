using UnityEngine;
using cardIndex;

public class DemoPlayerInstanceScript : MonoBehaviour
{
    //add/switch cards out of the deck here if you don't want to hardcode
    private string[] startingDeck =
    {
        "Terrorize",
        "Terrorize",
        "Conscript",
        "Cat Demolition Crew",
        "Cat",
        "Magic Cat / Septimus Mrreep",
        "Smite",
        "Scaredy Cat",
        "Comically Large Spoon Cat",
        "Ratta-tat-Cat",
        "Exploding Cat",
        "Night Vision Cat",
    };

    private Player p;

    private void Start()
    {
        p = GetComponent<Player>();
        
        if (p == null)
        {
            Debug.LogWarning("DemoPlayerInstanceScript needs a Player component.");
            return;
        }

        if (p.IsPlayerTwo)
        {
            //p.Deck.Add(new MinionParent("Dr. House(Cat)", NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent("Bobby", NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent("Slasher Cat", NewVirtualCardParent.location.deck));
            p.Deck.Add(new TwoAttackParent(3, 2, MinionParent.effect.heal, 4, 4, 0, "Witch Cat",
NewVirtualCardParent.type.minion, MinionParent.effect.twoAttacks, NewVirtualCardParent.location.deck));
            //p.Deck.Add(new MinionParent("Roughly A Cat", NewVirtualCardParent.location.deck));
            //p.Deck.Add(new MinionParent("Vampire Cat", NewVirtualCardParent.location.deck));
            //p.Deck.Add(new MinionParent("Nacho Cat", NewVirtualCardParent.location.deck));
        }

        p.Deck.Add(new SpellParent(SpellParent.spellEffect.unique, SpellParent.spellTarget.allEnemies, 0, 0, 4, "Blizzard",
NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));
        p.Deck.Add(new SpellParent(SpellParent.spellEffect.unique, SpellParent.spellTarget.allyCards, 0, 0, 4, "No Thoughts, Head Empty",
NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));
        p.Deck.Add(new SpellParent(SpellParent.spellEffect.equipment, SpellParent.spellTarget.enemyCards, 1, 0, 4, "Hex",
NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));

        for (int i = 0; i < startingDeck.Length; i++)   
        {
            p.Deck.Add(cardIndex.Index.CreateCard(startingDeck[i], NewVirtualCardParent.location.deck));
        }

        Debug.Log(gameObject.name + " deck loaded with " + p.Deck.Count + " cards.");
    }
}