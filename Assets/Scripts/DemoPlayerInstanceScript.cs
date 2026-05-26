using UnityEngine;

public class DemoPlayerInstanceScript : MonoBehaviour
{
    [SerializeField] private string[] startingDeck =
    {
        "Cat",
        "Macho Cat",
        "Cool Cat",
        "Western Cat",
        "Tank Cat"
    };

    private Player p;

    private void Start()
    {
        p = GetComponent<Player>();
        p.Deck.Add(new MinionParent("Ninja Cat", MinionParent.location.deck));
        p.Deck.Add(new MinionParent("Night Vision Cat", MinionParent.location.deck));
        p.Deck.Add(new MinionParent("Night Vision Cat", MinionParent.location.deck));
        p.Deck.Add(new MinionParent("Cool Cat", MinionParent.location.deck));
        p.Deck.Add(new MinionParent("Western Cat", MinionParent.location.deck));
        //p.Deck.Add(new CardParent(5, 7, 7, "Tank Cat", CardParent.type.minion, CardParent.effect.none, CardParent.location.deck));
        //p.Deck.Add(new CardParent(3, 4, 3, "Macho Cat", CardParent.type.minion, CardParent.effect.none, CardParent.location.deck));
        //p.Deck.Add(new CardParent(1, 1, 2, "Cat", CardParent.type.minion, CardParent.effect.none, CardParent.location.deck));
        //p.Deck.Add(new CardParent(1, 2, 1, "Cool Cat", CardParent.type.minion, CardParent.effect.none, CardParent.location.deck));
        //p.Deck.Add(new CardParent(2, 3, 2, "Western Cat", CardParent.type.minion, CardParent.effect.none, CardParent.location.deck));
    }

        if (p == null)
        {
            Debug.LogWarning("DemoPlayerInstanceScript needs a Player component.");
            return;
        }

        for (int i = 0; i < startingDeck.Length; i++)
        {
            p.Deck.Add(new MinionParent(startingDeck[i], NewVirtualCardParent.location.deck));
        }
    }
}