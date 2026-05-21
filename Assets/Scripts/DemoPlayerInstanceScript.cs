using UnityEngine;

public class DemoPlayerInstanceScript : MonoBehaviour
{
    // this script exists as a replacement for what will eventually be stored decks of cards from the deckbuilder - Jake
    //(which may be iostream/text files{?})

    private Player p;
    void Start()
    {
        p = GetComponent<Player>();
        p.Deck.Add(new NewVirtualCardParent("Tank Cat", NewVirtualCardParent.location.deck));
        p.Deck.Add(new NewVirtualCardParent("Macho Cat", NewVirtualCardParent.location.deck));
        p.Deck.Add(new NewVirtualCardParent("Cat", NewVirtualCardParent.location.deck));
        p.Deck.Add(new NewVirtualCardParent("Cool Cat", NewVirtualCardParent.location.deck));
        p.Deck.Add(new NewVirtualCardParent("Western Cat", NewVirtualCardParent.location.deck));
        //p.Deck.Add(new CardParent(5, 7, 7, "Tank Cat", CardParent.type.minion, CardParent.effect.none, CardParent.location.deck));
        //p.Deck.Add(new CardParent(3, 4, 3, "Macho Cat", CardParent.type.minion, CardParent.effect.none, CardParent.location.deck));
        //p.Deck.Add(new CardParent(1, 1, 2, "Cat", CardParent.type.minion, CardParent.effect.none, CardParent.location.deck));
        //p.Deck.Add(new CardParent(1, 2, 1, "Cool Cat", CardParent.type.minion, CardParent.effect.none, CardParent.location.deck));
        //p.Deck.Add(new CardParent(2, 3, 2, "Western Cat", CardParent.type.minion, CardParent.effect.none, CardParent.location.deck));
    }

}
