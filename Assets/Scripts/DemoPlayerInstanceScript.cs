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

        p.Deck.Add(new SpellParent(SpellParent.spellEffect.equipment, SpellParent.spellTarget.allyCards, 2, 2, 1, "I Hungy!!!",
            NewVirtualCardParent.type.spell, NewVirtualCardParent.location.deck));
   //    p.InPlay.Add(new MinionParent(1, 1, 0, "tokenTest",
  //  NewVirtualCardParent.type.token, MinionParent.effect.none, NewVirtualCardParent.location.inPlay));
        p.Deck.Add(new TwoAttackParent(3, 1, MinionParent.effect.aoe, 4, 4, 0, "Mage Cat",
            NewVirtualCardParent.type.minion, MinionParent.effect.twoAttacks, NewVirtualCardParent.location.deck));
        for (int i = 0; i < startingDeck.Length; i++)   
        {
            p.Deck.Add(new MinionParent(startingDeck[i], NewVirtualCardParent.location.deck));
        }

        Debug.Log(gameObject.name + " deck loaded with " + p.Deck.Count + " cards.");
    }
}