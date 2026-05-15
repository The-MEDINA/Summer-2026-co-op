using UnityEngine;

public class DemoPlayerInstanceScript : MonoBehaviour
{
    // this script exists as a replacement for what will eventually be stored decks of cards from the deckbuilder - Jake
    //(which may be iostream/text files{?})

    private Player p;
    void Start()
    {
        p = GetComponent<Player>();
        p.Deck.Add(new CardParent(1, 8, 5, CardParent.type.minion, CardParent.effect.none, CardParent.location.deck));
        p.Deck.Add(new CardParent(1, 8, 5, CardParent.type.minion, CardParent.effect.none, CardParent.location.deck));
        p.Deck.Add(new CardParent(1, 8, 5, CardParent.type.minion, CardParent.effect.none, CardParent.location.deck));
        p.Deck.Add(new CardParent(1, 8, 5, CardParent.type.minion, CardParent.effect.none, CardParent.location.deck));
        p.Deck.Add(new CardParent(1, 8, 5, CardParent.type.minion, CardParent.effect.none, CardParent.location.deck));
    }

}
