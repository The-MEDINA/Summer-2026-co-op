using UnityEngine;

public class DemoPlayerInstanceScript : MonoBehaviour
{
    private string[] startingDeck =
    {
        "Comically Large Spoon Cat",
        "Ninja Cat",
        "Night Vision Cat",
        "Night Vision Cat",
        "Magic Cat / Septimus Mrreep",
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

        p.Deck.Add(new MinionParent(1, 2, 5, "aoeCat", NewVirtualCardParent.type.minion, MinionParent.effect.aoe, NewVirtualCardParent.location.deck));
        p.Deck.Add(new TwoAttackParent(3, 1, MinionParent.effect.none, 1, 1, 1, "TwoAttack Cat",
            NewVirtualCardParent.type.minion, MinionParent.effect.twoAttacks, NewVirtualCardParent.location.deck));
        for (int i = 0; i < startingDeck.Length; i++)
        {
            p.Deck.Add(new MinionParent(startingDeck[i], NewVirtualCardParent.location.deck));
        }

        Debug.Log(gameObject.name + " deck loaded with " + p.Deck.Count + " cards.");
    }
}