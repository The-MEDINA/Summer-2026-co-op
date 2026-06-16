using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DeckInstanceDeckbuilderScript : MonoBehaviour
{
    private static DeckInstanceDeckbuilderScript instance;

    private List<NewVirtualCardParent> deck = new List<NewVirtualCardParent>();

    public List<NewVirtualCardParent> Deck { get { return this.deck; } } 

    private void Awake()
    {
        //singleton instance code
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);

        Deck.Add((new MinionParent(5, 4, 6, "Vampire Cat",
        NewVirtualCardParent.type.minion, MinionParent.effect.spawnToken, NewVirtualCardParent.location.deck)));
    }

    public void AddCard(NewVirtualCardParent card)
    {
        //deck at capacity
        //too many copies of that card
    }

    public void RemoveCard(NewVirtualCardParent card)
    {
        //no cards in list
        //no copies of that card in list
    }
}
