using UnityEngine;
using cardIndex;

public class DemoPlayerInstanceScript : MonoBehaviour
{
    //add/switch cards out of the deck here if you don't want to hardcode
    private string[] startingDeck =
    {
        "Scaredy Cat",
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

        p.Deck.Add(new SpellParent(SpellParent.spellEffect.unique, SpellParent.spellTarget.allyCards, 0, 0, 6, "Clone",
    NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));
        p.Deck.Add(new MinionParent(1, 1, 1, "Roughly A Cat",
        NewVirtualCardParent.type.minion, MinionParent.effect.guard, NewVirtualCardParent.location.deck));
        p.Deck.Add(new SpellParent(SpellParent.spellEffect.equipment, SpellParent.spellTarget.enemyCards, 1, 0, 4, "Hex",
    NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));
        p.Deck.Add(new MinionParent(5, 4, 6, "Vampire Cat",
        NewVirtualCardParent.type.minion, MinionParent.effect.spawnToken, NewVirtualCardParent.location.deck));
        p.Deck.Add(new TwoAttackParent(3, 2, MinionParent.effect.heal, 4, 4, 0, "Witch Cat",
    NewVirtualCardParent.type.minion, MinionParent.effect.twoAttacks, NewVirtualCardParent.location.deck));
        p.Deck.Add(new MinionParent(1, 3, 3, "AOETEst",
                NewVirtualCardParent.type.minion, MinionParent.effect.aoe, NewVirtualCardParent.location.deck));
        for (int i = 0; i < startingDeck.Length; i++)   
        {
                p.Deck.Add(new MinionParent(startingDeck[i], NewVirtualCardParent.location.deck));
        }

        Debug.Log(gameObject.name + " deck loaded with " + p.Deck.Count + " cards.");
    }
}