using UnityEngine;
using cardIndex;
using System.Collections.Generic;

public class DemoPlayerInstanceScript : MonoBehaviour
{
    //add/switch cards out of the deck here if you don't want to hardcode
    private string[] startingDeck =
    {
        "Gold Miner Cat",
        "Cave Cat",
        "Astro Cat",
        "The Mad Catter",
        "Pspspsps!",
        "Curse"
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

        }

        //        p.Deck.Add(new SpellParent(SpellParent.spellEffect.unique, SpellParent.spellTarget.none, 0, 0, 0, "Barbed Wire",
        //NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));
        //        p.Deck.Add(new SpellParent(SpellParent.spellEffect.unique, SpellParent.spellTarget.allEnemies, 0, 0, 4, "Blizzard",
        //NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));
        //        p.Deck.Add(new SpellParent(SpellParent.spellEffect.unique, SpellParent.spellTarget.allyCards, 0, 0, 4, "No Thoughts, Head Empty",
        //NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));
        //        p.Deck.Add(new SpellParent(SpellParent.spellEffect.equipment, SpellParent.spellTarget.enemyCards, 1, 0, 4, "Hex",
        //NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));

        DeckInstanceDeckbuilderScript dBDeck = FindAnyObjectByType<DeckInstanceDeckbuilderScript>();
        if (dBDeck != null)
        {
            p.Deck = new List<NewVirtualCardParent>(dBDeck.Deck);
        }
        else
        {
            p.Deck.Add(new TwoAttackParent(1, 1, MinionParent.effect.apoptosis, 1, 1, 1, "ApopTest", NewVirtualCardParent.type.minion,
                MinionParent.effect.twoAttacks, NewVirtualCardParent.location.deck));
            p.Deck.Add(new MinionParent(1, 1, 1, "frozenTest", NewVirtualCardParent.type.minion, MinionParent.effect.frozen, NewVirtualCardParent.location.deck));

            for (int i = 0; i < startingDeck.Length; i++)
            {
                p.Deck.Add(cardIndex.Index.CreateCard(startingDeck[i], NewVirtualCardParent.location.deck));
            }
        }

        Debug.Log(gameObject.name + " deck loaded with " + p.Deck.Count + " cards.");
    }
}