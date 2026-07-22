using UnityEngine;
using cardIndex;
using System.Collections.Generic;

public class DemoPlayerInstanceScript : MonoBehaviour
{
    //add/switch cards out of the deck here if you don't want to hardcode
    private string[] startingDeck =
    {
        "Magic Cat",
        "Duplicate",
        "Clone",
        "Reptoid",
        "Nuclear Waste",
        "I'm Sure That Wasn't Important", 
        "Fish Treat",
        "Slime",
        "Conscript",
        "Extraterrestrial Invader",
        "Digger",
        "Living Planet",
        "Blizzard",
        "Gold Miner Cat",
        "Cave Cat",
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

        //        p.Deck.Add(new SpellParent(SpellParent.spellEffect.unique, SpellParent.spellTarget.allyCards, 0, 0, 4, "No Thoughts, Head Empty",
        //NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));
        //        p.Deck.Add(new SpellParent(SpellParent.spellEffect.equipment, SpellParent.spellTarget.enemyCards, 1, 0, 4, "Hex",
        //NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));

        DeckInstanceDeckbuilderScript dBDeck = FindAnyObjectByType<DeckInstanceDeckbuilderScript>();
        if (dBDeck != null && !p.IsPlayerTwo)
        {
            // DON'T COPY THE CARDS DIRECTLY, USE THEIR NAMES TO MAKE NEW ONES
            // This should avoid any cases where the cards from the deck are modified (for some reason), and those modifications are brought into the next game.
            p.Deck = new List<NewVirtualCardParent>();
            for (int i = 0; i < dBDeck.Deck.Count; i++)
            {
                p.Deck.Add(cardIndex.Index.CreateCard(dBDeck.Deck[i].CardName, NewVirtualCardParent.location.deck));
            }
        }
        else if (Network.Networking.P1InitialDeck.Count > 0 && !p.IsPlayerTwo)
        {
            p.Deck = Network.Networking.P1InitialDeck;
        }
        else if (Network.Networking.P2InitialDeck.Count > 0 && p.IsPlayerTwo)
        {
            // (Same here as in those two comments above)
            p.Deck = new List<NewVirtualCardParent>();
            for (int i = 0; i < dBDeck.Deck.Count; i++)
            {
                p.Deck.Add(cardIndex.Index.CreateCard(Network.Networking.P2InitialDeck[i].CardName, NewVirtualCardParent.location.deck));
            }
        }
        else
        {
            /*p.Deck.Add(new MinionParent(1, 5, 1, "hiddenTest", NewVirtualCardParent.type.minion, MinionParent.effect.hidden, NewVirtualCardParent.location.hand));
            p.Deck.Add(new SpellParent(SpellParent.spellEffect.unique, SpellParent.spellTarget.enemyCards, 0, 0, 2, "Decipher",
    NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));
            p.Deck.Add(new SpellParent(SpellParent.spellEffect.unique, SpellParent.spellTarget.allyCards, 0, 0, 2, "Hide",
NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));
            p.Deck.Add(new SpellParent(SpellParent.spellEffect.unique, SpellParent.spellTarget.allAllies, 0, 0, 2, "Cover-Up",
NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));
            p.Deck.Add(new SpellParent(SpellParent.spellEffect.unique, SpellParent.spellTarget.allEnemies, 0, 0, 2, "Undeniable Proof",
NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));*/
            //p.Deck.Add(new TwoAttackParent(1, 1, MinionParent.effect.apoptosis, 1, 1, 1, "ApopTest", NewVirtualCardParent.type.minion,
            //MinionParent.effect.twoAttacks, NewVirtualCardParent.location.deck));

            p.Deck.Add(new MinionParent(1, 1, 1, "Mimic", NewVirtualCardParent.type.minion, MinionParent.effect.mimic, NewVirtualCardParent.location.deck));

            p.Deck.Add(new SpellParent(SpellParent.spellEffect.unique, SpellParent.spellTarget.allEnemies, 0, 0, 4, "Unknown Virus",
               NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));

            p.Deck.Add(cardIndex.Index.CreateCard("Genetic Engineering", NewVirtualCardParent.location.deck));
            p.Deck.Add(new SpellParent(SpellParent.spellEffect.unique, SpellParent.spellTarget.owner, 0, 0, 4, "Solar Panels",
                NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));
            p.Deck.Add(new SpellParent(SpellParent.spellEffect.unique, SpellParent.spellTarget.owner, 0, 0, 4, "Solar Panels",
    NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));

            p.Deck.Add(cardIndex.Index.CreateCard("Frozen Horror", NewVirtualCardParent.location.deck));

            for (int i = 0; i < startingDeck.Length; i++)
            {
                p.Deck.Add(cardIndex.Index.CreateCard(startingDeck[i], NewVirtualCardParent.location.deck));
            }
        }

        Debug.Log(gameObject.name + " deck loaded with " + p.Deck.Count + " cards.");
    }
}