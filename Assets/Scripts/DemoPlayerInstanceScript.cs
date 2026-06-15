using UnityEngine;
using cardIndex;

public class DemoPlayerInstanceScript : MonoBehaviour
{
    //add/switch cards out of the deck here if you don't want to hardcode
    private string[] startingDeck =
    {
        "Comically Large Spoon Cat",
        "Ninja Cat",
        "Night Vision Cat",
        "Night Vision Cat",
        "Ratta-tat-Cat",
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

        p.Deck.Add(new SpellParent(SpellParent.spellEffect.equipment, SpellParent.spellTarget.enemyCards, 2, 0, 2, "Curse",
    NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));
        p.Deck.Add(new MinionParent(3, 3, 2, "Mother Cat",
NewVirtualCardParent.type.minion, MinionParent.effect.spwnTokOnPlay, NewVirtualCardParent.location.deck));
        p.Deck.Add(new MinionParent(5, 4, 6, "Vampire Cat",
        NewVirtualCardParent.type.minion, MinionParent.effect.spawnToken, NewVirtualCardParent.location.deck));
        p.Deck.Add(new TwoAttackParent(3, 2, MinionParent.effect.heal, 4, 4, 0, "Witch Cat",
    NewVirtualCardParent.type.minion, MinionParent.effect.twoAttacks, NewVirtualCardParent.location.deck));
        p.Deck.Add(new MinionParent(1, 3, 3, "AOETEst",
                NewVirtualCardParent.type.minion, MinionParent.effect.aoe, NewVirtualCardParent.location.deck));
        p.Deck.Add(new MinionParent(4, 3, 3, "Single Celled Cat",
                NewVirtualCardParent.type.minion, MinionParent.effect.duplicate, NewVirtualCardParent.location.deck));
        p.Deck.Add(cardIndex.Index.CreateCard("M16", NewVirtualCardParent.location.deck));
        p.Deck.Add(new SpellParent("Conscript", NewVirtualCardParent.location.deck));
        p.Deck.Add(new SpellParent(SpellParent.spellEffect.equipment, SpellParent.spellTarget.allyCards, 2, 2, 1, "I Hungy!!!",
            NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));
        p.Deck.Add(new TwoAttackParent(3, 1, MinionParent.effect.aoe, 4, 4, 0, "Mage Cat",
            NewVirtualCardParent.type.minion, MinionParent.effect.twoAttacks, NewVirtualCardParent.location.deck));
        for (int i = 0; i < startingDeck.Length; i++)   
        {
                p.Deck.Add(new MinionParent(startingDeck[i], NewVirtualCardParent.location.deck));
        }

        Debug.Log(gameObject.name + " deck loaded with " + p.Deck.Count + " cards.");
    }
}